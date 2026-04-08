using UnityEngine;

// Keeps its distance and fires projectiles at the player
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
        shootTimer = 1f / fireRate;   // start ready to fire immediately
    }

    protected override void UpdateAI()
    {
        float dist = DistanceToPlayer();

        if (dist < preferredRange - 1f)
        {
            rb.linearVelocity = -DirectionToPlayer() * moveSpeed;         // back away
        }
        else if (dist > preferredRange + 1f)
        {
            rb.linearVelocity = DirectionToPlayer() * moveSpeed * 0.6f;   // close in slowly
        }
        else
        {
            // In sweet spot — strafe sideways
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