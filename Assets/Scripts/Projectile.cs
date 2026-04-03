using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    private float damage;

    void Start()
    {
        damage = PlayerStats.Instance.damage;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}