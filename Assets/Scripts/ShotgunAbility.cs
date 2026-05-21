using UnityEngine;

public class ShotgunAbility : MonoBehaviour
{
    // Damage multiplier on top of PlayerStats.damage
    // Level 1: 0.8x per bullet (3 bullets = 2.4x total DPS)
    // Each level adds 0.1x multiplier and 2 more bullets
    private int   bulletCount     = 3;
    private float spreadAngle     = 30f;
    private float damageMultiplier = 0.8f;

    public void LevelUp(int level)
    {
        bulletCount      = 3 + (level - 1) * 2;          // 3, 5, 7, 9...
        spreadAngle      = 30f + (level - 1) * 5f;        // 30, 35, 40, 45...
        damageMultiplier = 0.8f + (level - 1) * 0.1f;     // 0.8, 0.9, 1.0, 1.1...
    }

    public void FireShotgun(Vector2 baseDirection, GameObject projectilePrefab,
                            BouncingShotAbility bouncing, PlayerShooter shooter)
    {
        float damage      = PlayerStats.Instance.damage * damageMultiplier;
        float startAngle  = -spreadAngle / 2f;
        float angleStep   = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float   angle = startAngle + angleStep * i;
            Vector2 dir   = RotateVector(baseDirection, angle);

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            shooter.ApplyProjectileVisual(bullet, dir);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = dir * PlayerStats.Instance.projectileSpeed;

            if (bouncing != null)
            {
                BouncingBullet bb = bullet.AddComponent<BouncingBullet>();
                bb.Initialize(damage, bouncing.GetBounceCount());
            }
            else
            {
                Projectile proj = bullet.GetComponent<Projectile>();
                if (proj != null) proj.SetDamage(damage);
            }
        }
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        );
    }
}