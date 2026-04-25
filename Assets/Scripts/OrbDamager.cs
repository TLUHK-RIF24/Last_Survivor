using UnityEngine;
using System.Collections.Generic;

public class OrbDamager : MonoBehaviour
{
    public float damage         = 15f;
    public float damageCooldown = 0.5f;
    private Dictionary<GameObject, float> hitCooldowns = new Dictionary<GameObject, float>();

    void Update()
    {
        List<GameObject> keys = new List<GameObject>(hitCooldowns.Keys);
        foreach (GameObject key in keys)
        {
            hitCooldowns[key] -= Time.deltaTime;
            if (hitCooldowns[key] <= 0f) hitCooldowns.Remove(key);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitCooldowns.ContainsKey(other.gameObject)) return;

        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            hitCooldowns[other.gameObject] = damageCooldown;
        }
    }
}