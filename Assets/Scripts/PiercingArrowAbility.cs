using UnityEngine;

public class PiercingArrowAbility : MonoBehaviour
{
    private float damage = 30f;
    private int pierceCount = 3;

    public void LevelUp(int level)
    {
        damage = 30f + (level - 1) * 20f;
        pierceCount = 2 + level;
    }

    public void FirePiercingArrow(Vector2 direction)
    {
        GameObject arrow = new GameObject("PiercingArrow");
        arrow.transform.position = transform.position;
        arrow.transform.localScale = new Vector3(0.2f, 0.6f, 1f);

        SpriteRenderer sr = arrow.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.CreateCircle();
        sr.color = new Color(0.4f, 1f, 0.4f);
        sr.sortingOrder = 2;

        Rigidbody2D rb = arrow.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = direction * PlayerStats.Instance.projectileSpeed * 1.5f;

        CapsuleCollider2D col = arrow.AddComponent<CapsuleCollider2D>();
        col.isTrigger = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        PiercingArrowProjectile proj = arrow.AddComponent<PiercingArrowProjectile>();
        proj.Initialize(damage, pierceCount);
    }
}