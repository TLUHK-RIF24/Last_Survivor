using UnityEngine;

/// <summary>
/// Attach to the enemy projectile prefab.
/// Requires a Rigidbody2D and a Collider2D set to Is Trigger.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    public float lifetime = 4f;

    private float       damage;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, float dmg)
    {
        damage            = dmg;
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // ── Hook your player health system here ──
        // PlayerHealth.Instance?.TakeDamage(damage);

        Destroy(gameObject);
    }
}