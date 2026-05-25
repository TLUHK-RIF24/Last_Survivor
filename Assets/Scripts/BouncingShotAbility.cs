using UnityEngine;

public class BouncingShotAbility : MonoBehaviour
{
    private int   bounceCount      = 2;
    private float damageMultiplier = 1.2f;

    public void LevelUp(int level)
    {
        bounceCount      = 1 + level;                   // 2, 3, 4, 5...
        damageMultiplier = 1.2f + (level - 1) * 0.15f;  // 1.2, 1.35, 1.5, 1.65...
    }

    public int GetBounceCount() => bounceCount;

    public void FireBouncingShot(Vector2 direction, GameObject projectilePrefab, PlayerShooter shooter)
    {
        float damage = PlayerStats.Instance.damage * damageMultiplier;

        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        shooter.ApplyProjectileVisual(bullet, direction);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * PlayerStats.Instance.projectileSpeed;

        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null) Destroy(proj);

        BouncingBullet bb = bullet.AddComponent<BouncingBullet>();
        bb.Initialize(damage, bounceCount);
    }
}