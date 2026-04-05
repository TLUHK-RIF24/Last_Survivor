using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;

    private float fireTimer = 0f;
    private ShotgunAbility shotgun;

    void Start()
    {
        shotgun = GetComponent<ShotgunAbility>();
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= PlayerStats.Instance.fireRate)
        {
            fireTimer = 0f;
            TryShoot();
        }

        if (shotgun == null)
            shotgun = GetComponent<ShotgunAbility>();
    }

    void TryShoot()
    {
        GameObject nearest = FindNearestEnemy();
        if (nearest == null) return;

        Vector2 direction = (nearest.transform.position - transform.position).normalized;

        if (shotgun != null)
        {
            shotgun.FireShotgun(direction, projectilePrefab);
        }
        else
        {
            FireSingleBullet(direction);
        }
    }

    void FireSingleBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * PlayerStats.Instance.projectileSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}