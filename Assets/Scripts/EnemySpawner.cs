using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The invisible director of the entire experience.
///
/// Reads the WaveTable ScriptableObject to know:
///   — which enemies to spawn each minute
///   — how fast to pulse spawns
///   — when to fire special events (SwarmRush, ArenaRing, Stalker, WallSpawn)
///
/// Uses EnemyPool instead of Instantiate/Destroy.
/// Teleports distant enemies back to the spawn ring to recycle them.
/// Watches the player's kill rate and fires a Wall Spawn if they kill too fast.
///
/// Scene requirements:
///   • An EnemyPool GameObject (with all prefabs registered)
///   • This WaveTable ScriptableObject assigned below
///   • Player GameObject tagged "Player"
///   • Camera.main (orthographic)
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("References")]
    public WaveTable waveTable;

    [Header("Spawn Ring")]
    [Tooltip("Extra distance beyond the camera edge where enemies appear")]
    public float spawnRingPadding   = 1.8f;
    [Tooltip("Enemies beyond despawnDistance + this get teleported back")]
    public float extraDespawnBuffer = 2f;

    // ── Exposed to BaseEnemy so it can find the player quickly ──────────────
    public Transform PlayerTransform { get; private set; }

    // ── Private runtime ──────────────────────────────────────────────────────
    private Camera cam;
    private float  gameTime      = 0f;
    private float  spawnTimer    = 0f;
    private float  teleportTimer = 0f;

    // Kill-rate tracking
    private int   recentKills    = 0;
    private float killRateTimer  = 0f;
    private float wallCooldown   = 0f;
    private const float KILL_WINDOW = 3f;

    // Special events
    private bool         eventActive = false;
    private float        eventTimer  = 0f;
    private SpecialEvent activeEvent;
    private int          swarmEdge   = 0;

    // Live enemy list for teleport scanning
    private List<BaseEnemy> liveEnemies = new List<BaseEnemy>();

    // ─────────────────────────────────────────────────────────────────────────
    //  Unity
    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) PlayerTransform = p.transform;
        else Debug.LogError("[EnemySpawner] No GameObject tagged 'Player' found!");

        if (waveTable == null)
            Debug.LogError("[EnemySpawner] WaveTable not assigned!");
    }

    void Update()
    {
        if (waveTable == null || PlayerTransform == null) return;

        gameTime      += Time.deltaTime;
        spawnTimer    += Time.deltaTime;
        teleportTimer += Time.deltaTime;
        killRateTimer += Time.deltaTime;
        if (wallCooldown > 0f) wallCooldown -= Time.deltaTime;

        // ── Kill-rate check ──────────────────────────────────────────────────
        if (killRateTimer >= KILL_WINDOW)
        {
            float rate    = recentKills / KILL_WINDOW;
            recentKills   = 0;
            killRateTimer = 0f;

            if (rate >= waveTable.wallTriggerKillRate && wallCooldown <= 0f)
                TriggerWallSpawn();
        }

        // ── Special events ───────────────────────────────────────────────────
        TickSpecialEvents();

        // ── Main spawn pulse  (paused during a SwarmRush) ───────────────────
        float interval = waveTable.GetSpawnInterval(gameTime);
        if (spawnTimer >= interval)
        {
            spawnTimer = 0f;
            if (!IsEventActive(SpecialEventType.SwarmRush))
                SpawnPulse();
        }

        // ── Teleport distant enemies back to the spawn ring ──────────────────
        if (teleportTimer >= waveTable.teleportScanInterval)
        {
            teleportTimer = 0f;
            RecycleDistantEnemies();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Main spawn pulse
    // ─────────────────────────────────────────────────────────────────────────

    void SpawnPulse()
    {
        int cap    = waveTable.GetEnemyCap(gameTime);
        int active = EnemyPool.Instance != null ? EnemyPool.Instance.ActiveCount : 0;
        if (active >= cap) return;

        MinuteWindow window = waveTable.GetActiveWindow(gameTime);
        if (window == null || window.rules.Count == 0) return;

        SpawnRule rule = PickRule(window.rules);
        if (rule == null || rule.enemyPrefab == null) return;

        Vector2 origin = GetSpawnRingPosition();

        if (rule.spawnInClusters)
        {
            int count = Mathf.Min(
                Random.Range(rule.minClusterSize, rule.maxClusterSize + 1),
                cap - active   // never exceed cap in one pulse
            );
            for (int i = 0; i < count; i++)
                SpawnEnemy(rule.enemyPrefab, origin + Random.insideUnitCircle * rule.clusterSpread);
        }
        else
        {
            SpawnEnemy(rule.enemyPrefab, origin);
        }
    }

    void SpawnEnemy(GameObject prefab, Vector2 position)
    {
        if (EnemyPool.Instance == null) return;

        float hpMult     = waveTable.GetHPMultiplier(gameTime);
        float speedMult  = waveTable.GetSpeedMultiplier(gameTime);
        float damageMult = waveTable.GetDamageMultiplier(gameTime);

        BaseEnemy enemy = EnemyPool.Instance.Get(prefab, position, hpMult, speedMult, damageMult);
        if (enemy != null)
            liveEnemies.Add(enemy);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Called by BaseEnemy.Die()
    // ─────────────────────────────────────────────────────────────────────────

    public void OnEnemyDied(BaseEnemy enemy)
    {
        recentKills++;
        liveEnemies.Remove(enemy);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Teleport recycling
    // ─────────────────────────────────────────────────────────────────────────

    void RecycleDistantEnemies()
    {
        float maxDist = waveTable.despawnDistance + extraDespawnBuffer;

        for (int i = liveEnemies.Count - 1; i >= 0; i--)
        {
            BaseEnemy e = liveEnemies[i];
            if (e == null || !e.gameObject.activeSelf)
            {
                liveEnemies.RemoveAt(i);
                continue;
            }

            if (Vector2.Distance(e.transform.position, PlayerTransform.position) > maxDist)
                e.transform.position = GetSpawnRingPosition();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Wall Spawn  (DPS counter-measure)
    // ─────────────────────────────────────────────────────────────────────────

    void TriggerWallSpawn()
    {
        wallCooldown = waveTable.wallSpawnCooldown;
        Debug.Log("[EnemySpawner] Wall spawn triggered!");

        MinuteWindow window = waveTable.GetActiveWindow(gameTime);
        if (window == null || window.rules.Count == 0) return;

        // Use the heaviest-weight rule as the wall material (usually the tankiest)
        SpawnRule wallRule = window.rules[0];
        foreach (var r in window.rules)
            if (r.spawnWeight > wallRule.spawnWeight) wallRule = r;

        if (wallRule.enemyPrefab == null) return;

        int   count  = 16;
        float radius = CamHH() + spawnRingPadding + 1f;

        for (int i = 0; i < count; i++)
        {
            float   angle = (360f / count) * i * Mathf.Deg2Rad;
            Vector2 pos   = (Vector2)PlayerTransform.position
                            + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            SpawnEnemy(wallRule.enemyPrefab, pos);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Special events
    // ─────────────────────────────────────────────────────────────────────────

    void TickSpecialEvents()
    {
        foreach (var ev in waveTable.specialEvents)
        {
            if (!ev.triggered && gameTime >= ev.triggerTime)
            {
                ev.triggered = true;
                activeEvent  = ev;
                eventActive  = true;
                eventTimer   = 0f;
                swarmEdge    = Random.Range(0, 4);
                Debug.Log($"[EnemySpawner] Event started: {ev.eventType}");
                OnEventStart(ev);
            }
        }

        if (!eventActive) return;

        eventTimer += Time.deltaTime;
        OnEventTick(activeEvent, eventTimer);

        if (eventTimer >= activeEvent.duration)
        {
            Debug.Log($"[EnemySpawner] Event ended: {activeEvent.eventType}");
            eventActive = false;
            activeEvent = null;
        }
    }

    void OnEventStart(SpecialEvent ev)
    {
        switch (ev.eventType)
        {
            case SpecialEventType.ArenaRing:
            case SpecialEventType.WallSpawn:
                SpawnRing(ev.enemyPrefab, ev.enemyCount, ev.ringRadius);
                break;

            case SpecialEventType.Stalker:
                if (ev.enemyPrefab != null)
                    SpawnEnemy(ev.enemyPrefab, GetSpawnRingPosition());
                break;

            // SwarmRush spawns gradually in OnEventTick
        }
    }

    void OnEventTick(SpecialEvent ev, float elapsed)
    {
        if (ev.eventType != SpecialEventType.SwarmRush) return;

        // Every 0.25 s, fire a batch of swarmers from one screen edge
        bool newTick = Mathf.FloorToInt(elapsed / 0.25f)
                     > Mathf.FloorToInt((elapsed - Time.deltaTime) / 0.25f);
        if (!newTick || ev.enemyPrefab == null) return;

        int totalTicks = Mathf.Max(1, Mathf.RoundToInt(ev.duration / 0.25f));
        int batch      = Mathf.Max(1, ev.enemyCount / totalTicks);

        for (int i = 0; i < batch; i++)
            SpawnEnemy(ev.enemyPrefab, GetEdgePosition(swarmEdge));
    }

    bool IsEventActive(SpecialEventType type)
        => eventActive && activeEvent != null && activeEvent.eventType == type;

    void SpawnRing(GameObject prefab, int count, float radius)
    {
        if (prefab == null) return;
        for (int i = 0; i < count; i++)
        {
            float   angle = (360f / count) * i * Mathf.Deg2Rad;
            Vector2 pos   = (Vector2)PlayerTransform.position
                            + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            SpawnEnemy(prefab, pos);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Weighted rule picker
    // ─────────────────────────────────────────────────────────────────────────

    SpawnRule PickRule(List<SpawnRule> rules)
    {
        float total = 0f;
        foreach (var r in rules) total += r.spawnWeight;

        float roll       = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var r in rules)
        {
            cumulative += r.spawnWeight;
            if (roll <= cumulative) return r;
        }
        return rules[rules.Count - 1];
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Position helpers
    // ─────────────────────────────────────────────────────────────────────────

    Vector2 GetSpawnRingPosition() => GetEdgePosition(Random.Range(0, 4));

    Vector2 GetEdgePosition(int edge)
    {
        float hw = CamHW() + spawnRingPadding;
        float hh = CamHH() + spawnRingPadding;
        Vector2 p = PlayerTransform.position;

        return edge switch
        {
            0 => new Vector2(p.x + Random.Range(-hw, hw), p.y + hh),
            1 => new Vector2(p.x + Random.Range(-hw, hw), p.y - hh),
            2 => new Vector2(p.x - hw, p.y + Random.Range(-hh, hh)),
            _ => new Vector2(p.x + hw, p.y + Random.Range(-hh, hh)),
        };
    }

    float CamHH() => cam != null ? cam.orthographicSize : 5f;
    float CamHW() => cam != null ? cam.orthographicSize * cam.aspect : 8f;
}