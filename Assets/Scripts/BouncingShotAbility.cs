using UnityEngine;

public class BouncingShotAbility : MonoBehaviour
{
    private int bounceCount = 2;
    private float damage = 15f;

    public void LevelUp(int level)
    {
        bounceCount = 1 + level;
        damage = 15f + (level - 1) * 10f;
    }

    public void FireBouncingShot(Vector2 direction)
    {
        GameObject bullet = new GameObject("BouncingBullet");
        bullet.transform.position = transform.position;
        bullet.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.CreateCircle();
        sr.color = new Color(0.3f, 0.8f, 1f);
        sr.sortingOrder = 2;

        Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = direction * PlayerStats.Instance.projectileSpeed;

        CircleCollider2D col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.15f;

        BouncingBullet bb = bullet.AddComponent<BouncingBullet>();
        bb.Initialize(damage, bounceCount);
    }
}