using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Image  characterImage;
    [SerializeField] private float  frameInterval = 0.5f;  

    private Sprite[] frames;
    private int      currentFrame = 0;
    private float    timer        = 0f;
    private bool     isAnimating  = false;

    void Awake()
    {
        if (characterImage == null)
            characterImage = GetComponent<Image>();
    }

    void Update()
    {
        if (!isAnimating || frames == null || frames.Length < 2) return;

        timer += Time.deltaTime;
        if (timer >= frameInterval)
        {
            timer        = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            characterImage.sprite = frames[currentFrame];
        }
    }

    public void SetFrames(Sprite[] newFrames)
    {
        frames       = newFrames;
        currentFrame = 0;
        timer        = 0f;
        isAnimating  = frames != null && frames.Length >= 2;

        if (characterImage != null && frames != null && frames.Length > 0)
            characterImage.sprite = frames[0];
    }

    public void Stop()
    {
        isAnimating = false;
    }
}