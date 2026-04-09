using UnityEngine;

public class AuraField : MonoBehaviour
{
    private float damage = 10f;
    private float damageInterval = 1f;
    private float radius = 1.2f;
    private float timer = 0f;
    private GameObject auraVisual;

    void Start()
    {
        CreateVisual();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= damageInterval)
        {
            timer = 0f;
            DamageNearbyEnemies();
        }
    }

    public void LevelUp(int level)
    {
        damage = 10f + (level - 1) * 8f;
        radius = 1.2f + (level - 1) * 0.25f;
        UpdateVisual();
    }

    void DamageNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
                        BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                        if (enemy != null) enemy.TakeDamage(damage);
            }
        }
    }

    void CreateVisual()
    {
        auraVisual = new GameObject("AuraVisual");
        auraVisual.transform.SetParent(transform);
        auraVisual.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = auraVisual.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.CreateCircle(128);
        sr.color = new Color(0.5f, 0f, 1f, 0.25f);
        sr.sortingOrder = 1;

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (auraVisual != null)
            auraVisual.transform.localScale = Vector3.one * radius * 2f;
    }