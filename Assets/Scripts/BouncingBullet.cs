using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BouncingBullet : MonoBehaviour
{
    private float            damage;
    private int              bouncesLeft;
    private float            bounceRange  = 6f;
    private List<GameObject> hitEnemies   = new List<GameObject>();
    private Rigidbody2D      rb;
    private Collider2D       myCollider;

    public void Initialize(float dmg, int bounces)
    {
        damage       = dmg;
        bouncesLeft  = bounces;
        rb           = GetComponent<Rigidbody2D>();
        myCollider   = GetComponent<Collider2D>();
        Destroy(gameObject, 6f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (hitEnemies.Contains(other.gameObject)) return;

        hitEnemies.Add(other.gameObject);

        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null) enemy.TakeDamage(damage);

        if (bouncesLeft > 0)
        {
            bouncesLeft--;
            if (myCollider != null)
                Physics2D.IgnoreCollision(myCollider, other, true);

            BounceToNextEnemy(other.gameObject);

            StartCoroutine(ReEnableCollision(other));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void BounceToNextEnemy(GameObject justHit)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject   next    = null;
        float        minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            if (hitEnemies.Contains(e)) continue;
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < bounceRange && d < minDist) { minDist = d; next = e; }
        }

        if (next != null)
        {
            Vector2 newDir = (next.transform.position - transform.position).normalized;
            rb.linearVelocity = newDir * PlayerStats.Instance.projectileSpeed;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ReEnableCollision(Collider2D other)
    {
        yield return new WaitForSeconds(0.3f);
        if (myCollider != null && other != null)
            Physics2D.IgnoreCollision(myCollider, other, false);
    }
}