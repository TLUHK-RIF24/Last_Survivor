using UnityEngine;

// ═════════════════════════════════════════════════════════════════════════════
//  All enemy AI types live in this one file.
//  Each is a subclass of BaseEnemy and overrides UpdateAI().
// ═════════════════════════════════════════════════════════════════════════════


// ─────────────────────────────────────────────────────────────────────────────
//  1. FODDER  —  slow straight-line chaser, the bread-and-butter crowd filler
// ─────────────────────────────────────────────────────────────────────────────
public class EnemyAI_Fodder : BaseEnemy
{
}


// ─────────────────────────────────────────────────────────────────────────────
//  2. SWARMER  —  fast and tiny, zigzags while chasing, always in clusters
// ─────────────────────────────────────────────────────────────────────────────
public class EnemyAI_Swarmer : BaseEnemy
{
    [Header("Swarmer Settings")]
    public float zigzagAmplitude = 1.2f;
    public float zigzagFrequency = 3.0f;

    private float zigzagOffset;

    protected override void OnSpawnExtra()
    {
        zigzagOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    protected override void UpdateAI()
    {
        if (player == null) return;

        Vector2 toPlayer = DirectionToPlayer();
        Vector2 perp     = new Vector2(-toPlayer.y, toPlayer.x);
        float   zigzag   = Mathf.Sin(Time.time * zigzagFrequency + zigzagOffset) * zigzagAmplitude;
        Vector2 dir      = (toPlayer + perp * zigzag).normalized;

        rb.linearVelocity = dir * moveSpeed;
    }
}


// ─────────────────────────────────────────────────────────────────────────────
//  3. TANK  —  slow, huge HP, knocks the player back on contact
// ─────────────────────────────────────────────────────────────────────────────
public class EnemyAI_Tank : BaseEnemy
{
    [Header("Tank Settings")]
    public float knockbackForce = 6f;

    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);   

        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 dir = ((Vector2)other.transform.position - rb.position).normalized;
                playerRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}


// ─────────────────────────────────────────────────────────────────────────────
//  4. CHARGER  —  telegraphs with an orange flash, then dashes at high speed
// ─────────────────────────────────────────────────────────────────────────────
public class EnemyAI_Charger : BaseEnemy
{
    [Header("Charger Settings")]
    public float telegraphDuration = 0.9f;
    public float dashSpeed         = 18f;
    public float dashDuration      = 0.25f;
    public float cooldownDuration  = 1.2f;
    public Color telegraphColor    = new Color(1f, 0.4f, 0f);

    private enum State { Approach, Telegraph, Dash, Cooldown }
    private State   state = State.Approach;
    private float   stateTimer;
    private Vector2 dashDir;
    private Color   baseColor;

    protected override void OnSpawnExtra()
    {
        state      = State.Approach;
        stateTimer = 0f;
        if (spriteRenderer != null) baseColor = spriteRenderer.color;
    }

    protected override void UpdateAI()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Approach:
                MoveTowardsPlayer();
                if (DistanceToPlayer() < 5f) EnterTelegraph();
                break;

            case State.Telegraph:
                rb.linearVelocity = Vector2.zero;
                if (spriteRenderer != null)
                {
                    float t = Mathf.PingPong(stateTimer * 6f, 1f);
                    spriteRenderer.color = Color.Lerp(baseColor, telegraphColor, t);
                }
                if (stateTimer >= telegraphDuration) EnterDash();
                break;

            case State.Dash:
                rb.linearVelocity = dashDir * dashSpeed;
                if (stateTimer >= dashDuration) EnterCooldown();
                break;

            case State.Cooldown:
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 8f);
                if (stateTimer >= cooldownDuration)
                {
                    state = State.Approach;
                    stateTimer = 0f;
                    if (spriteRenderer != null) spriteRenderer.color = baseColor;
                }
                break;
        }
    }

    private void EnterTelegraph()
    {
        state      = State.Telegraph;
        stateTimer = 0f;
        dashDir    = DirectionToPlayer();
    }

    private void EnterDash()
    {
        state      = State.Dash;
        stateTimer = 0f;
        if (spriteRenderer != null) spriteRenderer.color = baseColor;
    }

    private void EnterCooldown()
    {
        state      = State.Cooldown;
        stateTimer = 0f;
    }
}


// ─────────────────────────────────────────────────────────────────────────────
//  5. PROJECTOR  —  keeps its distance and fires projectiles at the player
// ─────────────────────────────────────────────────────────────────────────────
public class EnemyAI_Projector : BaseEnemy
{
    [Header("Projector Settings")]
    public GameObject projectilePrefab;
    public float preferredRange   = 7f;
    public float fireRate         = 1.5f;
    public float projectileSpeed  = 5f;
    public float projectileDamage = 8f;

    private float shootTimer;

    protected override void OnSpawnExtra()
    {
        shootTimer = 1f / fireRate;   
    }

    protected override void UpdateAI()
    {
        float dist = DistanceToPlayer();

        if (dist < preferredRange - 1f)
        {
            rb.linearVelocity = -DirectionToPlayer() * moveSpeed;        
        }
        else if (dist > preferredRange + 1f)
        {
            rb.linearVelocity = DirectionToPlayer() * moveSpeed * 0.6f;  
        }
        else
        {
            Vector2 perp = new Vector2(-DirectionToPlayer().y, DirectionToPlayer().x);
            rb.linearVelocity = perp * moveSpeed * 0.4f;
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= 1f / fireRate)
        {
            shootTimer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || player == null) return;

        Vector2 dir   = DirectionToPlayer();
        float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.Init(dir, projectileSpeed, projectileDamage * (damage / baseDamage));
        else
        {
            Rigidbody2D prb = proj.GetComponent<Rigidbody2D>();
            if (prb != null) prb.linearVelocity = dir * projectileSpeed;
        }
    }
}


// ─────────────────────────────────────────────────────────────────────────────
//  6. ELITE  —  mini-boss, scales up visually, drops loot signal on death
// ─────────────────────────────────────────────────────────────────────────────
public class EnemyAI_Elite : BaseEnemy
{
    [Header("Elite Settings")]
    public Color eliteColor = new Color(0.8f, 0.2f, 1f);

    protected override void OnSpawnExtra()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = eliteColor;

        transform.localScale = Vector3.one * 1.6f;
    }

    protected override void Die()
    {
        // Hook loot / XP here:
        // LootManager.Instance?.DropChest(transform.position);
        // XPManager.Instance?.SpawnGem(transform.position, xpValue * 5f);

        transform.localScale = Vector3.one; 
        base.Die();
    }
}