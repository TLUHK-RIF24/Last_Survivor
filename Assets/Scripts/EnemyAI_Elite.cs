using UnityEngine;

// Mini-boss — scales up visually on spawn, drops loot signal on death
public class EnemyAI_Elite : BaseEnemy
{
    [Header("Elite Settings")]
    public Color eliteColor = new Color(0.8f, 0.2f, 1f);

    protected override void OnSpawnExtra()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = eliteColor;

        transform.localScale = Vector3.one * 1.6f;
    }

    protected override void Die()
    {
        transform.localScale = Vector3.one;   // reset scale before returning to pool
        base.Die();                           // base.Die() handles XP drop + pool return
    }
}