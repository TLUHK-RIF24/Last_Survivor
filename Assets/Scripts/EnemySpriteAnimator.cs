using UnityEngine;

public class EnemySpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameInterval = 0.15f;

    private int         currentFrame = 0;
    private float       timer        = 0f;
    private Rigidbody2D rb;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (frames == null || frames.Length < 2) return;

        timer += Time.deltaTime;
        if (timer >= frameInterval)
        {
            timer        = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
        }

        if (rb != null && rb.linearVelocity.x != 0)
            spriteRenderer.flipX = rb.linearVelocity.x > 0;
    }
}