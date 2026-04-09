using UnityEngine;

/// <summary>
/// Replaces both EnemyHealth and EnemyMovement.
/// Do NOT attach this directly to prefabs — attach one of the EnemyAI_X subclasses.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    [Header("Base Stats  (set on each prefab)")]
    public float baseMaxHealth = 80f;
    public float baseMoveSpeed = 2.8f;
    public float baseDamage    = 10f;
    public float baseXPValue   = 5f;

    [Header("XP Drop")]
    [SerializeField] private GameObject xpOrbPrefab;

    [Header("Hit Flash")]
    public SpriteRenderer spriteRenderer;
    public Color  damageFlashColor = Color.red;
    public float  damageFlashTime  = 0.08f;

    // ── Set by the pool on every spawn ───────────────────────────────────────
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float damage;
    [HideInInspector] public float xpValue;

    // ── Internals ────────────────────────────────────────────────────────────
    protected float       currentHealth;
    protected Transform   player;
    protected Rigidbody2D rb;

    private Color originalColor;   // set once in Awake, never overwritten
    private float flashTimer;
    private bool  isFlashing;

    private float contactDamageTimer;
    private const float CONTACT_INTERVAL = 0.5f;

    // ─────────────────────────────────────────────────────────────────────────
    //  Awake — runs ONCE when the prefab instance is first created by the pool
    //  Store originalColor here so it never gets corrupted by a flash state
    // ─────────────────────────────────────────────────────────────────────────
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the true prefab color here — this only runs once per instance
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Called by EnemyPool every time this object is pulled from the pool
    // ─────────────────────────────────────────────────────────────────────────
    public virtual void OnSpawn(float hpMult, float speedMult, float damageMult)
    {
        maxHealth  = baseMaxHealth * hpMult;
        moveSpeed  = baseMoveSpeed * speedMult;
        damage     = baseDamage    * damageMult;
        xpValue    = baseXPValue   * Mathf.Sqrt(hpMult);

        currentHealth      = maxHealth;
        contactDamageTimer = 0f;
        flashTimer         = 0f;
        isFlashing         = false;

        // Always restore to the true original color on every spawn
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        player = EnemySpawner.Instance != null
            ? EnemySpawner.Instance.PlayerTransform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        OnSpawnExtra();
    }

    /// <summary>Override in subclass for per-type spawn init.</summary>
    protected virtual void OnSpawnExtra() { }

    // ─────────────────────────────────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────────────────────────────────
    protected virtual void Update()
    {
        if (player == null) return;
        contactDamageTimer += Time.deltaTime;
        HandleFlash();
        UpdateAI();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  AI hook — override in subclasses
    // ─────────────────────────────────────────────────────────────────────────
    protected virtual void UpdateAI()
    {
        MoveTowardsPlayer();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Shared movement helpers for subclasses
    // ─────────────────────────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────────────────────────
    //  Health / damage
    // ─────────────────────────────────────────────────────────────────────────
    public void TakeDamage(float amount)
    {
        if (!gameObject.activeSelf) return;
        currentHealth -= amount;
        TriggerFlash();
        if (currentHealth <= 0f) Die();
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
        EnemyPool.Instance?.Return(this);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Contact damage to player
    // ─────────────────────────────────────────────────────────────────────────
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (contactDamageTimer < CONTACT_INTERVAL) return;
        contactDamageTimer = 0f;

        // Hook your player health system here:
        // PlayerHealth.Instance?.TakeDamage(damage);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Damage flash
    // ─────────────────────────────────────────────────────────────────────────
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