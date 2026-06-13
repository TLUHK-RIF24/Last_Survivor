using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    public float lifetime = 4f;

    private float       damage;
    private Rigidbody2D rb;
    private Sprite      damageSourceSprite;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, float dmg, Sprite sourceSprite = null)
    {
        damage             = dmg;
        damageSourceSprite = sourceSprite != null ? sourceSprite : GetComponent<SpriteRenderer>()?.sprite;
        rb.linearVelocity  = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerHealth.Instance?.TakeDamage(damage, damageSourceSprite);
        Destroy(gameObject);
    }
}
