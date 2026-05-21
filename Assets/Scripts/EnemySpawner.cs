using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Core loop (runs every wave.spawnInterval seconds):
///   1. If total alive >= hardCap → skip
///   2. For each enemy type in the current wave:
///      - If alive count < minimum → spawn until quota filled
///      - If already at or over minimum → spawn exactly 1
///
/// HP scaling: finalHP = baseHP × playerLevel  (if hpScalesWithLevel is set on the prefab)
/// Speed/Damage: flat multipliers (default 1.0)
///
/// Also handles:
///   - Timed map events (SwarmRush, WallSpawn, ArenaRing)
///   - Teleport recycling of distant enemies
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

    public Transform PlayerTransform { get; private set; }

    private Camera cam;
    private float  gameTime      = 0f;
    private float  spawnTimer    = 0f;
    private float  teleportTimer = 0f;

    private Dictionary<string, int> alivePerType = new Dictionary<string, int>();

    private List<BaseEnemy> liveEnemies = new List<BaseEnemy>();

    private bool      swarmActive  = false;
    private float     swarmTimer   = 0f;
    private float     swarmDuration = 0f;
    private GameObject swarmPrefab  = null;
    private int       swarmEdge     = 0;


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
        else Debug.LogError("[EnemySpawner] No Player tagged 'Player' found!");

        if (waveTable == null) Debug.LogError("[EnemySpawner] WaveTable not assigned!");
    }

    void Update()
    {
        if (waveTable == null || PlayerTransform == null) return;

        gameTime      += Time.deltaTime;
        spawnTimer    += Time.deltaTime;
        teleportTimer += Time.deltaTime;

        TickMapEvents();

        WaveDefinition wave = waveTable.GetWave(gameTime);
        if (wave != null && spawnTimer >= wave.spawnInterval)
        {
            spawnTimer = 0f;
            if (!swarmActive) // pause regular spawns during a swarm rush
                RunQuotaFill(wave);
        }

        if (swarmActive)
        {
            swarmTimer += Time.deltaTime;
            if (Mathf.FloorToInt(swarmTimer / 0.25f) > Mathf.FloorToInt((swarmTimer - Time.deltaTime) / 0.25f))
                SpawnEnemy(swarmPrefab, GetEdgePosition(swarmEdge));

            if (swarmTimer >= swarmDuration)
                swarmActive = false;
        }

        if (teleportTimer >= waveTable.teleportScanInterval)
        {
            teleportTimer = 0f;
            RecycleDistantEnemies();
        }
    }


    void RunQuotaFill(WaveDefinition wave)
    {
        int totalAlive = EnemyPool.Instance != null ? EnemyPool.Instance.ActiveCount : 0;
        if (totalAlive >= waveTable.hardCap) return;

        foreach (var entry in wave.enemies)
        {
            if (entry.prefab == null) continue;

            string key   = entry.prefab.name;
            int    alive = alivePerType.ContainsKey(key) ? alivePerType[key] : 0;

            if (alive < entry.minimumAlive)
            {
                // Fill to quota
                int needed = entry.minimumAlive - alive;
                for (int i = 0; i < needed; i++)
                {
                    if (EnemyPool.Instance.ActiveCount >= waveTable.hardCap) break;
                    SpawnEnemy(entry.prefab, GetSpawnRingPosition());
                }
            }
            else
            {
                if (EnemyPool.Instance.ActiveCount < waveTable.hardCap)
                    SpawnEnemy(entry.prefab, GetSpawnRingPosition());
            }
        }
    }


    void SpawnEnemy(GameObject prefab, Vector2 position)
    {
        if (EnemyPool.Instance == null || prefab == null) return;

        int playerLevel = GameManager.Instance != null
            ? GameManager.Instance.GetCurrentLevel()
            : 1;

        BaseEnemy enemy = EnemyPool.Instance.Get(prefab, position, playerLevel);
        if (enemy != null)
        {
            liveEnemies.Add(enemy);
            string key = prefab.name;
            if (!alivePerType.ContainsKey(key)) alivePerType[key] = 0;
            alivePerType[key]++;
        }
    }

    public void OnEnemyDied(BaseEnemy enemy)
    {
        liveEnemies.Remove(enemy);
        string key = enemy.gameObject.name;
        if (alivePerType.ContainsKey(key))
            alivePerType[key] = Mathf.Max(0, alivePerType[key] - 1);
    }


    void TickMapEvents()
    {
        foreach (var ev in waveTable.mapEvents)
        {
            if (ev.triggered || gameTime < ev.triggerTime) continue;
            ev.triggered = true;
            Debug.Log($"[EnemySpawner] Map event: {ev.label} ({ev.eventType})");
            TriggerMapEvent(ev);
        }
    }

    void TriggerMapEvent(MapEvent ev)
    {
        switch (ev.eventType)
        {
            case MapEventType.SwarmRush:
                swarmActive   = true;
                swarmTimer    = 0f;
                swarmDuration = ev.duration;
                swarmPrefab   = ev.enemyPrefab;
                swarmEdge     = Random.Range(0, 4);
                break;

            case MapEventType.WallSpawn:
            case MapEventType.ArenaRing:
                SpawnRing(ev.enemyPrefab, ev.enemyCount, ev.ringRadius);
                break;
        }
    }

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