using UnityEngine;

// Fast and tiny, zigzags while chasing, always spawned in clusters
public class EnemyAI_Swarmer : BaseEnemy
{
    [Header("Swarmer Settings")]
    public float zigzagAmplitude = 1.2f;
    public float zigzagFrequency = 3.0f;

    private float zigzagOffset;

    protected override void OnSpawnExtra()
    {
        zigzagOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    protected override void UpdateAI()
    {
        if (player == null) return;

        Vector2 toPlayer = DirectionToPlayer();
        Vector2 perp     = new Vector2(-toPlayer.y, toPlayer.x);
        float   zigzag   = Mathf.Sin(Time.time * zigzagFrequency + zigzagOffset) * zigzagAmplitude;
        Vector2 dir      = (toPlayer + perp * zigzag).normalized;

        rb.linearVelocity = dir * moveSpeed;
    }
}