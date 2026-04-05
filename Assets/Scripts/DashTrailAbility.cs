using UnityEngine;

public class DashTrailAbility : MonoBehaviour
{
    private float damage = 8f;
    private float trailDuration = 1.5f;
    private float trailInterval = 0.15f;
    private float trailTimer = 0f;
    private float trailRadius = 0.8f;
    private Vector2 lastPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
    }

    public void LevelUp(int level)
    {
        damage = 8f + (level - 1) * 6f;
        trailDuration = 1.5f + (level - 1) * 0.3f;
        trailRadius = 0.8f + (level - 1) * 0.15f;
    }

    void Update()
    {
        // only spawn trail if player is moving
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
        GameObject patch = new GameObject("TrailPatch");
        patch.transform.position = transform.position;

        SpriteRenderer sr = patch.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        patch.transform.localScale = Vector3.one * trailRadius * 2f;

        CircleCollider2D col = patch.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = trailRadius;

        TrailPatch tp = patch.AddComponent<TrailPatch>();
        tp.Initialize(damage, trailDuration);
    }
}