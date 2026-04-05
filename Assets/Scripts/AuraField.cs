using UnityEngine;

public class AuraField : MonoBehaviour
{
    private float damage = 10f;
    private float damageInterval = 1f;
    private float radius = 2f;
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
        radius = 2f + (level - 1) * 0.3f;
        UpdateVisual();
    }

    void DamageNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth health = hit.GetComponent<EnemyHealth>();
                if (health != null)
                    health.TakeDamage(damage);
            }
        }
    }

    void CreateVisual()
    {
        auraVisual = new GameObject("AuraVisual");
        auraVisual.transform.SetParent(transform);
        auraVisual.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = auraVisual.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = new Color(0.5f, 0f, 1f, 0.2f); 
        sr.sortingOrder = -1;

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (auraVisual != null)
            auraVisual.transform.localScale = Vector3.one * radius * 2f;
    }

    Sprite CreateCircleSprite()
    {
        
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        float r = size / 2;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, dist <= r ? Color.white : Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}