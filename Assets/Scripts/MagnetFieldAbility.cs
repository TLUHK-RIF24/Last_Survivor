using UnityEngine;

public class MagnetFieldAbility : MonoBehaviour
{
    private float pullRadius = 5f;
    private float pullSpeed  = 6f;

    public void LevelUp(int level)
    {
        pullRadius = 5f + (level - 1) * 3f;
        pullSpeed  = 6f + (level - 1) * 2f;
    }

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (Collider2D hit in hits)
        {
            // Use GetComponent instead of tag — XPOrb is a script, not a tag
            if (hit.GetComponent<XPOrb>() == null) continue;

            Vector2 direction = (transform.position - hit.transform.position).normalized;
            hit.transform.position += (Vector3)direction * pullSpeed * Time.deltaTime;
        }
    }
}