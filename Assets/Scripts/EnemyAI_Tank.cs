using UnityEngine;

// Slow, huge HP, knocks the player back on contact
public class EnemyAI_Tank : BaseEnemy
{
    [Header("Tank Settings")]
    public float knockbackForce = 6f;

    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 dir = ((Vector2)other.transform.position - rb.position).normalized;
                playerRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}