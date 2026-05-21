using UnityEngine;

public class AuraField : MonoBehaviour
{
    private float damageMultiplier = 0.5f;
    private float damageInterval   = 1.0f;
    private float radius           = 1.2f;
    private float timer            = 0f;
    private GameObject auraVisual;

    void Start() => CreateVisual();

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= damageInterval) { timer = 0f; DamageNearbyEnemies(); }
    }

    public void LevelUp(int level)
    {
        damageMultiplier = 0.5f  + (level - 1) * 0.15f;  // 0.5, 0.65, 0.8, 0.95, 1.1
        radius           = 1.2f  + (level - 1) * 0.3f;   // 1.2, 1.5, 1.8, 2.1, 2.4
        damageInterval   = Mathf.Max(0.5f, 1.0f - (level - 1) * 0.1f); // ticks faster with level
        UpdateVisual();
    }

    void DamageNearbyEnemies()
    {
        float damage = PlayerStats.Instance.damage * damageMultiplier;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }

    void CreateVisual()
    {
        auraVisual = new GameObject("AuraVisual");
        auraVisual.transform.SetParent(transform);
        auraVisual.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = auraVisual.AddComponent<SpriteRenderer>();
        sr.sprite       = SpriteHelper.CreateCircle(128);
        sr.color        = new Color(0.5f, 0f, 1f, 0.25f);
        sr.sortingOrder = 1;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (auraVisual != null)
            auraVisual.transform.localScale = Vector3.one * radius * 2f;
    }
}