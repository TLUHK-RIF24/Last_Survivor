using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    private float damage;
    private bool  damageSet = false;

    void Start()
    {
        if (!damageSet)
            damage = PlayerStats.Instance.damage;
        Destroy(gameObject, lifetime);
    }

    public void SetDamage(float value)
    {
        damage    = value;
        damageSet = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null) enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}