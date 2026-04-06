using UnityEngine;
using System.Collections.Generic;

public class BoomerangProjectile : MonoBehaviour
{
    private Transform player;
    private Vector2 direction;
    private float damage;
    private float speed;
    private bool returning = false;
    private float maxDistance = 8f;
    private Vector3 startPos;
    private List<GameObject> hitEnemies = new List<GameObject>();

    public void Initialize(Transform playerTransform, Vector2 dir, float dmg, float spd)
    {
        player = playerTransform;
        direction = dir;
        damage = dmg;
        speed = spd;
        startPos = transform.position;
    }

    void Update()
    {
        if (!returning)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            transform.Rotate(0, 0, 360 * Time.deltaTime);

            if (Vector2.Distance(transform.position, startPos) >= maxDistance)
            {
                returning = true;
                hitEnemies.Clear();
            }
        }
        else
        {
            Vector2 toPlayer = (player.position - transform.position).normalized;
            transform.position += (Vector3)toPlayer * speed * Time.deltaTime;
            transform.Rotate(0, 0, -360 * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.position) < 0.5f)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitEnemies.Contains(other.gameObject)) return;

        hitEnemies.Add(other.gameObject);

        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null) enemy.TakeDamage(damage);
    }
}