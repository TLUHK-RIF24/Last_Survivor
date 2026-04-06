using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private GameObject xpOrbPrefab;
    [SerializeField] private float xpReward = 10f;

    // Call this when the enemy dies
    public void Die()
    {
        if (xpOrbPrefab != null)
        {
            GameObject orb = Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
            orb.GetComponent<XPOrb>()?.SetXPValue(xpReward);
        }

        Destroy(gameObject);
    }
}