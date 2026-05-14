using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fadeDuration = 1.2f;
    public float lifeTime = 1.8f;
    public float scaleUpAmount = 1.3f;

    private TextMeshPro tmp;
    private Vector3 startPosition;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    public void Setup(string text, Color color)
    {
        tmp.text = text;
        tmp.color = color;
        startPosition = transform.position;

        // Optional: Start a bit bigger then shrink
        transform.localScale = Vector3.one * scaleUpAmount;

        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;

            // Float upward
            transform.position = startPosition + new Vector3(0, elapsed * floatSpeed, 0);

            // Gentle scale down
            float scale = Mathf.Lerp(scaleUpAmount, 1f, elapsed / lifeTime);
            transform.localScale = Vector3.one * scale;

            // Fade out
            if (elapsed > lifeTime - fadeDuration)
            {
                float alpha = 1f - (elapsed - (lifeTime - fadeDuration)) / fadeDuration;
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
