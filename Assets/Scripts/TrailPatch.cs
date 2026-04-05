using UnityEngine;
using System.Collections.Generic;

public class TrailPatch : MonoBehaviour
{
    private float damage;
    private float duration;
    private float damageCooldown = 0.5f;
    private Dictionary<GameObject, float> hitCooldowns = new Dictionary<GameObject, float>();

    public void Initialize(float dmg, float dur)
    {
        damage = dmg;
        duration = dur;
        Destroy(gameObject, dur);
    }

    void Update()
    {
        var keys = new System.Collections.Generic.List<GameObject>(hitCooldowns.Keys);
        foreach (var key in keys)
        {
            hitCooldowns[key] -= Time.deltaTime;
            if (hitCooldowns[key] <= 0f) hitCooldowns.Remove(key);
        }

        // fade out over time
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(0.5f, 0f, 1f - (duration / duration));
            sr.color = c;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitCooldowns.ContainsKey(other.gameObject)) return;

        EnemyHealth health = other.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            hitCooldowns[other.gameObject] = damageCooldown;
        }
    }
}