using UnityEngine;

public class MagnetFieldAbility : MonoBehaviour
{
    private float pullRadius = 5f;
    private float pullSpeed = 6f;

    public void LevelUp(int level)
    {
        pullRadius = 5f + (level - 1) * 3f;
        pullSpeed = 6f + (level - 1) * 2f;
    }

    void Update()
    {
        PullXPOrbs();
    }

    void PullXPOrbs()
    {
        // finds all XP orbs in radius and pulls them toward player
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