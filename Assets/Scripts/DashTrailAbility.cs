using UnityEngine;

public class DashTrailAbility : MonoBehaviour
{
    private float damageMultiplier = 0.3f; 
    private float trailDuration    = 1.2f;
    private float trailInterval    = 0.2f;
    private float trailRadius      = 0.6f;
    private float trailTimer       = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void LevelUp(int level)
    {
        damageMultiplier = 0.3f + (level - 1) * 0.2f;   // 0.3, 0.4, 0.5, 0.6...
        trailDuration    = 1.2f + (level - 1) * 0.2f;
        trailRadius      = 0.6f + (level - 1) * 0.1f;
    }

    void Update()
    {
        if (rb == null || rb.linearVelocity.magnitude < 0.5f) return;
        trailTimer += Time.deltaTime;
        if (trailTimer >= trailInterval)
        {
            trailTimer = 0f;
            SpawnTrailPatch();
        }
    }

    void SpawnTrailPatch()
    {
        float damage = PlayerStats.Instance.damage * damageMultiplier;

        GameObject patch = new GameObject("TrailPatch");
        patch.transform.position   = transform.position;
        patch.transform.localScale = Vector3.one * trailRadius * 2f;

        SpriteRenderer sr = patch.AddComponent<SpriteRenderer>();
        sr.sprite       = SpriteHelper.CreateCircle();
        sr.color        = new Color(0.8f, 0.2f, 0.2f, 0.4f);
        sr.sortingOrder = 1;

        CircleCollider2D col = patch.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        TrailPatch tp = patch.AddComponent<TrailPatch>();
        tp.Initialize(damage, trailDuration);
    }
}