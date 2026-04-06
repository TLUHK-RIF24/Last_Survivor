using UnityEngine;

public class DashTrailAbility : MonoBehaviour
{
    private float damage = 8f;
    private float trailDuration = 1.5f;
    private float trailInterval = 0.15f;
    private float trailTimer = 0f;
    private float trailRadius = 0.8f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void LevelUp(int level)
    {
        damage = 8f + (level - 1) * 6f;
        trailDuration = 1.5f + (level - 1) * 0.3f;
        trailRadius = 0.8f + (level - 1) * 0.15f;
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
        GameObject patch = new GameObject("TrailPatch");
        patch.transform.position = transform.position;
        patch.transform.localScale = Vector3.one * trailRadius * 2f;

        SpriteRenderer sr = patch.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.CreateCircle();
        sr.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        sr.sortingOrder = 1;

        CircleCollider2D col = patch.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        TrailPatch tp = patch.AddComponent<TrailPatch>();
        tp.Initialize(damage, trailDuration);
    }
}