using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fadeDuration = 1.2f;
    public float lifeTime = 1.8f;

    private TextMeshPro tmp;
    private Vector3 startPosition;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();

        // Force it to top
        tmp.sortingOrder = 200;
    }

    public void Setup(string text, Color color)
    {
        tmp.text = text;
        tmp.color = color;
        startPosition = transform.position;

        transform.localScale = Vector3.one * 1.4f;

        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;

            transform.position = startPosition + new Vector3(0, elapsed * floatSpeed, 0);

            float scale = Mathf.Lerp(1.4f, 1f, elapsed / lifeTime);
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
