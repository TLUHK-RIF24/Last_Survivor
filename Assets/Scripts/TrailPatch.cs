using UnityEngine;
using System.Collections.Generic;

public class TrailPatch : MonoBehaviour
{
    private float damage;
    private float duration;
    private float elapsed = 0f;
    private float damageCooldown = 0.5f;
    private Dictionary<GameObject, float> hitCooldowns = new Dictionary<GameObject, float>();
    private SpriteRenderer sr;

    public void Initialize(float dmg, float dur)
    {
        damage   = dmg;
        duration = dur;
        sr       = GetComponent<SpriteRenderer>();
        Destroy(gameObject, dur);
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        List<GameObject> keys = new List<GameObject>(hitCooldowns.Keys);
        foreach (var key in keys)
        {
            hitCooldowns[key] -= Time.deltaTime;
            if (hitCooldowns[key] <= 0f) hitCooldowns.Remove(key);
        }

        if (sr != null)
        {
            Color c = sr.color;
            c.a     = Mathf.Lerp(0.5f, 0f, elapsed / duration);
            sr.color = c;
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