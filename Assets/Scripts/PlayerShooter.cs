using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject            projectilePrefab;
    public PlayerAnimatorHandler animatorHandler;

    [Header("Character Projectile Sprites")]
    [Tooltip("Knight uses the default projectile prefab sprite — leave empty")]
    public Sprite knightProjectileSprite;
    [Tooltip("Mage fireball frame 1")]
    public Sprite mageFireball1;
    [Tooltip("Mage fireball frame 2")]
    public Sprite mageFireball2;
    [Tooltip("Archer arrow sprite")]
    public Sprite archerArrow;

    private float                fireTimer = 0f;
    private ShotgunAbility       shotgun;
    private PiercingArrowAbility piercing;
    private BouncingShotAbility  bouncing;

    private int      selectedCharacter = 0;
    private Sprite[] mageFrames;


    void Start()
    {
        shotgun  = GetComponent<ShotgunAbility>();
        piercing = GetComponent<PiercingArrowAbility>();
        bouncing = GetComponent<BouncingShotAbility>();

        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        if (mageFireball1 != null && mageFireball2 != null)
            mageFrames = new Sprite[] { mageFireball1, mageFireball2 };
        else if (mageFireball1 != null)
            mageFrames = new Sprite[] { mageFireball1 };
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= PlayerStats.Instance.fireRate)
        {
            fireTimer = 0f;
            TryShoot();
        }

        if (shotgun  == null) shotgun  = GetComponent<ShotgunAbility>();
        if (piercing == null) piercing = GetComponent<PiercingArrowAbility>();
        if (bouncing == null) bouncing = GetComponent<BouncingShotAbility>();
    }

    void TryShoot()
    {
        GameObject nearest = FindNearestEnemy();
        if (nearest == null) return;

        if (animatorHandler != null)
            animatorHandler.PlayAttackAnimation();

        Vector2 direction = (nearest.transform.position - transform.position).normalized;

        if (shotgun != null)
            shotgun.FireShotgun(direction, projectilePrefab, bouncing, this);
        else if (piercing != null)
            piercing.FirePiercingArrow(direction);
        else if (bouncing != null)
            bouncing.FireBouncingShot(direction, projectilePrefab, this);
        else
            FireSingleBullet(direction);
    }

    void FireSingleBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        ApplyProjectileVisual(bullet, direction);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * PlayerStats.Instance.projectileSpeed;
    }

    public void ApplyProjectileVisual(GameObject bullet, Vector2 direction)
    {
        SpriteRenderer     sr   = bullet.GetComponentInChildren<SpriteRenderer>();
        ProjectileAnimator anim = bullet.GetComponentInChildren<ProjectileAnimator>();

        switch (selectedCharacter)
        {
            case 0: // Archer
                if (sr != null && archerArrow != null)
                    sr.sprite = archerArrow;
                float arrowAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
                break;

            case 1: // Mage
                if (anim != null && mageFrames != null)
                    anim.SetFrames(mageFrames);
                else if (sr != null && mageFireball1 != null)
                    sr.sprite = mageFireball1;
                float fireAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                bullet.transform.rotation = Quaternion.AngleAxis(fireAngle, Vector3.forward);
                break;

            default: // Knight
                if (knightProjectileSprite != null && sr != null)
                    sr.sprite = knightProjectileSprite;
                float knightAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                bullet.transform.rotation = Quaternion.AngleAxis(knightAngle, Vector3.forward);
                break;
        }
    }

    public GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject   nearest = null;
        float        minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist) { minDist = dist; nearest = enemy; }
        }
        return nearest;
    }
}