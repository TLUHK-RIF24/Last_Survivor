using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("Orb Settings")]
    [SerializeField] private float xpValue = 10f;
    [SerializeField] private float pickupRadius = 1.2f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float attractRadius = 4f; // Distance at which orb starts moving toward player

    private Transform player;
    private bool isAttracting = false;

    void Start()
    {
        // Find player — cache this however you reference your player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Start attracting once player is close enough
        if (dist <= attractRadius)
            isAttracting = true;

        if (isAttracting)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }

        // Pick up when close enough
        if (dist <= pickupRadius)
        {
            GameManager.Instance.AddXP(xpValue);
            Destroy(gameObject);
        }
    }

    // Call this from outside to set XP value before spawning (for varied orbs)
    public void SetXPValue(float value)
    {
        xpValue = value;
    }
}