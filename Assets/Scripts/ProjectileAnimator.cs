using UnityEngine;

public class ProjectileAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[]       frames;
    [SerializeField] private float          frameInterval = 0.08f;

    private int   currentFrame = 0;
    private float timer        = 0f;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (frames == null || frames.Length < 2) return;

        timer += Time.deltaTime;
        if (timer >= frameInterval)
        {
            timer        = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            if (spriteRenderer != null)
                spriteRenderer.sprite = frames[currentFrame];
        }
    }

    public void SetFrames(Sprite[] newFrames)
    {
        frames       = newFrames;
        currentFrame = 0;
        timer        = 0f;
        if (spriteRenderer != null && frames != null && frames.Length > 0)
            spriteRenderer.sprite = frames[0];
    }
}