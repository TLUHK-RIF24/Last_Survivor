using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFallingParticles : MonoBehaviour
{
    [SerializeField] private Sprite[] particleSprites;
    [SerializeField] private int particleCount = 76;
    [SerializeField] private Vector2 speedRange = new Vector2(85f, 165f);
    [SerializeField] private Vector2 driftRange = new Vector2(-26f, 26f);
    [SerializeField] private Vector2 scaleRange = new Vector2(1f, 1f);
    [SerializeField] private Vector2 alphaRange = new Vector2(0.85f, 1f);
    [SerializeField] private bool fillOnStart = true;

    private readonly List<FallingParticle> particles = new List<FallingParticle>();
    private readonly List<Sprite> usableSprites = new List<Sprite>();
    private RectTransform layerRect;
    private Sprite fallbackSprite;
    private bool initialized;

    private sealed class FallingParticle
    {
        public RectTransform rect;
        public Image image;
        public float speed;
        public float drift;
        public float rotationSpeed;
    }

    private void Awake()
    {
        layerRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EnsureInitialized();
    }

    private void Update()
    {
        EnsureInitialized();

        if (!initialized || layerRect == null)
            return;

        Rect layerBounds = GetLayerBounds();
        float bottom = -layerBounds.height * 0.5f - 80f;
        float deltaTime = Time.unscaledDeltaTime;

        foreach (FallingParticle particle in particles)
        {
            Vector2 position = particle.rect.anchoredPosition;
            position.x += particle.drift * deltaTime;
            position.y -= particle.speed * deltaTime;
            particle.rect.anchoredPosition = position;
            particle.rect.Rotate(0f, 0f, particle.rotationSpeed * deltaTime);

            if (position.y < bottom)
                ResetParticle(particle, false);
        }
    }

    private FallingParticle CreateParticle(int index)
    {
        GameObject particleObject = new GameObject($"Particle_{index:00}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        particleObject.transform.SetParent(transform, false);

        RectTransform rect = particleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localScale = Vector3.one;

        Image image = particleObject.GetComponent<Image>();
        image.raycastTarget = false;
        image.preserveAspect = true;

        return new FallingParticle
        {
            rect = rect,
            image = image
        };
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        if (layerRect == null)
            layerRect = GetComponent<RectTransform>();

        if (layerRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        CacheUsableSprites();

        for (int i = 0; i < particleCount; i++)
        {
            FallingParticle particle = CreateParticle(i);
            ResetParticle(particle, fillOnStart);
            particles.Add(particle);
        }

        initialized = true;
    }

    private void CacheUsableSprites()
    {
        usableSprites.Clear();

        if (particleSprites != null)
        {
            foreach (Sprite sprite in particleSprites)
            {
                if (sprite != null)
                    usableSprites.Add(sprite);
            }
        }

        if (usableSprites.Count == 0)
            usableSprites.Add(GetFallbackSprite());
    }

    private void ResetParticle(FallingParticle particle, bool randomizeHeight)
    {
        Rect rect = GetLayerBounds();
        Sprite sprite = usableSprites[Random.Range(0, usableSprites.Count)];
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        float halfWidth = rect.width * 0.5f;
        float halfHeight = rect.height * 0.5f;

        particle.image.sprite = sprite;
        particle.image.color = new Color(1f, 1f, 1f, Random.Range(alphaRange.x, alphaRange.y));
        particle.rect.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height) * scale;
        particle.rect.anchoredPosition = new Vector2(
            Random.Range(-halfWidth, halfWidth),
            randomizeHeight ? Random.Range(-halfHeight, halfHeight) : halfHeight + Random.Range(20f, 140f)
        );
        particle.rect.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
        particle.speed = Random.Range(speedRange.x, speedRange.y);
        particle.drift = Random.Range(driftRange.x, driftRange.y);
        particle.rotationSpeed = Random.Range(-35f, 35f);
    }

    private Rect GetLayerBounds()
    {
        Rect rect = layerRect.rect;
        if (rect.width > 1f && rect.height > 1f)
            return rect;

        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas != null ? canvas.GetComponent<RectTransform>() : null;
        if (canvasRect != null && canvasRect.rect.width > 1f && canvasRect.rect.height > 1f)
            return canvasRect.rect;

        return new Rect(0f, 0f, 1920f, 1080f);
    }

    private Sprite GetFallbackSprite()
    {
        if (fallbackSprite != null)
            return fallbackSprite;

        Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false)
        {
            name = "FallbackMenuParticle",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color dark = new Color(0f, 0f, 0f, 1f);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(7.5f, 7.5f));
                texture.SetPixel(x, y, distance <= 3.5f ? dark : clear);
            }
        }

        texture.Apply();
        fallbackSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        return fallbackSprite;
    }
}
