using UnityEngine;

public class BoomerangAbility : MonoBehaviour
{
    private int   boomerangCount   = 1;
    private float damageMultiplier = 1.5f; 
    private float cooldown         = 3f;
    private float timer            = 0f;

    void Start()
    {
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown) { timer = 0f; FireBoomerangs(); }
    }

    public void LevelUp(int level)
    {
        boomerangCount   = level;                          // 1, 2, 3, 4...
        damageMultiplier = 1.5f + (level - 1) * 0.3f;    // 1.5, 1.8, 2.1, 2.4...
        cooldown         = Mathf.Max(1f, 3f - (level - 1) * 0.25f);
    }

    void FireBoomerangs()
    {
        PlayerShooter shooter = GetComponent<PlayerShooter>();
        GameObject nearest = shooter != null ? shooter.FindNearestEnemy() : null;
        if (nearest == null) return;

        float damage = PlayerStats.Instance.damage * damageMultiplier;

        for (int i = 0; i < boomerangCount; i++)
        {
            float angleOffset = i * (360f / boomerangCount);
            Vector2 dir = nearest.transform.position - transform.position;
            dir = RotateVector(dir.normalized, angleOffset);

            GameObject boomerang = new GameObject("Boomerang");
            boomerang.transform.position   = transform.position;
            boomerang.transform.localScale  = new Vector3(0.4f, 0.4f, 1f);

            SpriteRenderer sr = boomerang.AddComponent<SpriteRenderer>();
            sr.sprite       = SpriteHelper.CreateCircle();
            sr.color        = new Color(1f, 0.8f, 0.2f);
            sr.sortingOrder = 2;

            Rigidbody2D rb = boomerang.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;

            CircleCollider2D col = boomerang.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 0.3f;

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