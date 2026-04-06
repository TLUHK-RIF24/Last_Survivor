using UnityEngine;

/// Do NOT attach this directly to prefabs — attach one of the EnemyAI_X subclasses.

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    [Header("Base Stats  (set on each prefab)")]
    public float baseMaxHealth = 80f;
    public float baseMoveSpeed = 2.8f;
    public float baseDamage    = 10f;
    public float baseXPValue   = 5f;

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

        if (spriteRenderer != null)
        {
            originalColor        = spriteRenderer.color;
            spriteRenderer.color = originalColor;
        }

        player = EnemySpawner.Instance != null
            ? EnemySpawner.Instance.PlayerTransform
            : GameObject.FindGameObjectWithTag("Player")?.transform;

        OnSpawnExtra();
    }

    protected virtual void OnSpawnExtra() { }


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
        contactDamageTimer += Time.deltaTime;
        HandleFlash();
        UpdateAI();
    }

    protected virtual void UpdateAI()
    {
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

        // ── Hook your XP / loot system here ──
        // XPManager.Instance?.SpawnGem(transform.position, xpValue);

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;
        EnemyPool.Instance?.Return(this);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (contactDamageTimer < CONTACT_INTERVAL) return;
        contactDamageTimer = 0f;

        // ── Hook your player health system here ──
        // PlayerHealth.Instance?.TakeDamage(damage);
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