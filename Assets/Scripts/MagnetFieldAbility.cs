using UnityEngine;

public class MagnetFieldAbility : MonoBehaviour
{
    private float pullRadius = 5f;
    private float pullSpeed = 6f;
    private GameObject magnetVisual;

    void Start()
    {
        magnetVisual = new GameObject("MagnetVisual");
        magnetVisual.transform.SetParent(transform);
        magnetVisual.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = magnetVisual.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.CreateCircle(128);
        sr.color = new Color(1f, 1f, 0f, 0.08f);
        sr.sortingOrder = 1;

        magnetVisual.transform.localScale = Vector3.one * pullRadius * 2f;
    }

    public void LevelUp(int level)
    {
        pullRadius = 5f + (level - 1) * 3f;
        pullSpeed = 6f + (level - 1) * 2f;

        if (magnetVisual != null)
            magnetVisual.transform.localScale = Vector3.one * pullRadius * 2f;
    }

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("XPOrb"))
            {
                Vector2 direction = (transform.position - hit.transform.position).normalized;
                hit.transform.position += (Vector3)direction * pullSpeed * Time.deltaTime;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}