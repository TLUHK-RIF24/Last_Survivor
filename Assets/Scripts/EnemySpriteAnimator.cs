using UnityEngine;

public class EnemySpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Simple Animation (uses flipX for direction — e.g. bat)")]
    [SerializeField] private Sprite[] frames;

    [Header("Directional Animation (separate left and right frames)")]
    [SerializeField] private Sprite[] leftFrames;
    [SerializeField] private Sprite[] rightFrames;

    [SerializeField] private float frameInterval = 0.15f;

    private int         currentFrame   = 0;
    private float       timer          = 0f;
    private bool        usingDirectional = false;
    private Rigidbody2D rb;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        usingDirectional = leftFrames != null && leftFrames.Length > 0
                        && rightFrames != null && rightFrames.Length > 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameInterval)
        {
            timer = 0f;
            currentFrame++;
        }

        if (usingDirectional)
            UpdateDirectional();
        else
            UpdateSimple();
    }

    void UpdateDirectional()
    {
        bool movingRight = rb != null && rb.linearVelocity.x > 0.1f;
        Sprite[] activeFrames = movingRight ? rightFrames : leftFrames;

        if (activeFrames == null || activeFrames.Length == 0) return;

        currentFrame = currentFrame % activeFrames.Length;
        spriteRenderer.sprite = activeFrames[currentFrame];
        spriteRenderer.flipX  = false;
    }

    void UpdateSimple()
    {
        if (frames == null || frames.Length == 0) return;

        currentFrame = currentFrame % frames.Length;
        spriteRenderer.sprite = frames[currentFrame];

        if (rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            spriteRenderer.flipX = rb.linearVelocity.x > 0;
    }
}