using UnityEngine;

public class PiercingArrowAbility : MonoBehaviour
{
    private float damageMultiplier = 2.5f; 
    private int   pierceCount      = 3;

    public void LevelUp(int level)
    {
        damageMultiplier = 2.5f + (level - 1) * 0.5f;  // 2.5, 3.0, 3.5, 4.0...
        pierceCount      = 2 + level;                    // 3, 4, 5, 6...
    }

    public void FirePiercingArrow(Vector2 direction)
    {
        float damage = PlayerStats.Instance.damage * damageMultiplier;

        GameObject arrow = new GameObject("PiercingArrow");
        arrow.transform.position   = transform.position;
        arrow.transform.localScale = new Vector3(0.2f, 0.6f, 1f);

        SpriteRenderer sr = arrow.AddComponent<SpriteRenderer>();
        sr.sprite       = SpriteHelper.CreateCircle();
        sr.color        = new Color(0.4f, 1f, 0.4f);
        sr.sortingOrder = 2;

        Rigidbody2D rb = arrow.AddComponent<Rigidbody2D>();
        rb.gravityScale    = 0f;
        rb.linearVelocity  = direction * PlayerStats.Instance.projectileSpeed * 1.5f;

        CapsuleCollider2D col = arrow.AddComponent<CapsuleCollider2D>();
        col.isTrigger = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        PiercingArrowProjectile proj = arrow.AddComponent<PiercingArrowProjectile>();
        proj.Initialize(damage, pierceCount);
    }
}