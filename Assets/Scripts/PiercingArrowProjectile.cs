using UnityEngine;
using System.Collections.Generic;

public class PiercingArrowProjectile : MonoBehaviour
{
    private float damage;
    private int pierceCount;
    private List<GameObject> hitEnemies = new List<GameObject>();

    public void Initialize(float dmg, int pierce)
    {
        damage = dmg;
        pierceCount = pierce;
        Destroy(gameObject, 4f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitEnemies.Contains(other.gameObject)) return;

        hitEnemies.Add(other.gameObject);

        EnemyHealth health = other.GetComponent<EnemyHealth>();
        if (health != null) health.TakeDamage(damage);

        pierceCount--;
        if (pierceCount <= 0)
            Destroy(gameObject);
    }
}