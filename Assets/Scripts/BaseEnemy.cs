using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    [Header("Base Stats  (set on each prefab)")]
    public float baseMaxHealth = 10f;
    public float baseMoveSpeed = 2.8f;
    public float baseDamage    = 10f;
    public float baseXPValue   = 1f;

    [Header("HP x Level")]
    [Tooltip("If true, finalHP = baseHP × playerLevel at spawn time (VS mechanic).")]
    public bool hpScalesWithLevel = false;

    [Header("XP Drop")]
    [SerializeField] private GameObject xpOrbPrefab;

    [Header("Hit Flash")]
    public SpriteRenderer spriteRenderer;
    public Color  damageFlashColor = Color.red;
    public float  damageFlashTime  = 0.08f;

    [HideInInspector] public float maxHealth;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float damage;
    [HideInInspector] public float xpValue;

    protected float       currentHealth;
    protected Transform   player;
    protected Rigidbody2D rb;

    private Color originalColor;
    private float flashTimer;
    private bool  isFlashing;

    private float contactDamageTimer;
    private const float CONTACT_INTERVAL = 0.5f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public virtual void OnSpawn(int playerLevel, float speedMult = 1f, float damageMult = 1f)
    {
        float hpLevel = hpScalesWithLevel ? Mathf.Max(1, playerLevel) : 1f;
        maxHealth  = baseMaxHealth * hpLevel;
        moveSpeed  = baseMoveSpeed * speedMult;
        damage     = baseDamage    * damageMult;
        xpValue    = baseXPValue;

        currentHealth      = maxHealth;
        contactDamageTimer = 0f;
        flashTimer         = 0f;
        isFlashing         = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        player = EnemySpawner.Instance != null
            ? EnemySpawner.Instance.PlayerTransform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        rb.simulated = true;
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols) c.enabled = true;

        OnSpawnExtra();
    }

    protected virtual void OnSpawnExtra() { }

    protected virtual void Update()
    {
        if (player == null) return;
        contactDamageTimer += Time.deltaTime;
        HandleFlash();
        UpdateAI();
    }

    protected virtual void UpdateAI()
    {
        Separate();
        MoveTowardsPlayer();
    }

    protected void MoveTowardsPlayer()
    {
        if (player == null) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    protected Vector2 DirectionToPlayer()
    {
        if (player == null) return Vector2.zero;
        return ((Vector2)player.position - rb.position).normalized;
    }

    protected float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(rb.position, player.position);
    }

    protected virtual void Separate()
    {
        Collider2D[] neighbours = Physics2D.OverlapCircleAll(
            rb.position, 0.5f, LayerMask.GetMask("Enemy"));
        foreach (Collider2D n in neighbours)
        {
            if (n.gameObject == gameObject) continue;
            Vector2 push = rb.position - (Vector2)n.transform.position;
            if (push.magnitude < 0.01f) push = Random.insideUnitCircle;
            rb.position += push.normalized * 0.02f;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!gameObject.activeSelf) return;

        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols) c.enabled = false;

        currentHealth -= amount;
        TriggerFlash();
        if (currentHealth <= 0f) Die();
        else
        {
            // Re-enable if not dead
            rb.simulated = true;
            foreach (var c in cols) c.enabled = true;
        }
    }

    protected virtual void Die()
    {
        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.OnEnemyDied(this);

        if (xpOrbPrefab != null)
        {
            GameObject orb = Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
            orb.GetComponent<XPOrb>()?.SetXPValue(xpValue);
        }

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols) c.enabled = false;
        EnemyPool.Instance?.Return(this);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (contactDamageTimer < CONTACT_INTERVAL) return;
        contactDamageTimer = 0f;
        PlayerHealth.Instance?.TakeDamage(damage);
    }

    private void TriggerFlash()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = damageFlashColor;
        flashTimer = damageFlashTime;
        isFlashing = true;
    }

    private void HandleFlash()
    {
        if (!isFlashing) return;
        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f)
        {
            spriteRenderer.color = originalColor;
            isFlashing = false;
        }
    }
}