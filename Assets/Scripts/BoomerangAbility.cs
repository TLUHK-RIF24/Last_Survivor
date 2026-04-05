using UnityEngine;

public class BoomerangAbility : MonoBehaviour
{
    private int boomerangCount = 1;
    private float damage = 20f;
    private float cooldown = 3f;
    private float timer = 0f;
    private GameObject projectilePrefab;

    void Start()
    {
        projectilePrefab = GetComponent<PlayerShooter>().projectilePrefab;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            timer = 0f;
            FireBoomerangs();
        }
    }

    public void LevelUp(int level)
    {
        boomerangCount = level;
        damage = 20f + (level - 1) * 15f;
        cooldown = Mathf.Max(1f, 3f - (level - 1) * 0.2f);
    }

    void FireBoomerangs()
    {
        GameObject nearest = GetComponent<PlayerShooter>().FindNearestEnemy();
        if (nearest == null) return;

        for (int i = 0; i < boomerangCount; i++)
        {
            float angleOffset = i * (360f / boomerangCount);
            Vector2 dir = nearest.transform.position - transform.position;
            dir = RotateVector(dir.normalized, angleOffset);

            GameObject boomerang = new GameObject("Boomerang");
            boomerang.transform.position = transform.position;

            SpriteRenderer sr = boomerang.AddComponent<SpriteRenderer>();
            sr.color = new Color(1f, 0.8f, 0.2f);

            Rigidbody2D rb = boomerang.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;

            CircleCollider2D col = boomerang.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;

            BoomerangProjectile bp = boomerang.AddComponent<BoomerangProjectile>();
            bp.Initialize(transform, dir, damage, PlayerStats.Instance.projectileSpeed);
        }
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
                           v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad));
    }
}