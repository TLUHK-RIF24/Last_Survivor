using UnityEngine;

public class ShotgunAbility : MonoBehaviour
{
    private int bulletCount = 3;
    private float spreadAngle = 30f;
    private PlayerShooter shooter;

    void Start()
    {
        shooter = GetComponent<PlayerShooter>();
    }

    public void LevelUp(int level)
    {
        bulletCount = 3 + (level - 1) * 2;
        spreadAngle = 30f + (level - 1) * 5f;
    }

    public void FireShotgun(Vector2 baseDirection, GameObject projectilePrefab)
    {
        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = RotateVector(baseDirection, angle);

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * PlayerStats.Instance.projectileSpeed;

            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            bullet.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
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