using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    public static XPBarUI Instance;

    [SerializeField] private Image    xpFillImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timerText;

    private float elapsedTime = 0f;
    private bool  running     = true;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!running) return;
        if (Time.timescale == 0f) return;
        elapsedTime += Time.deltaTime;
        UpdateTimer();
    }

    public void UpdateBar(float currentXP, float maxXP)
    {
        if (xpFillImage != null)
            xpFillImage.fillAmount = Mathf.Clamp01(currentXP / maxXP);

        if (levelText != null && GameManager.Instance != null)
            levelText.text = $"LV {GameManager.Instance.GetCurrentLevel()}";
    }

    private void UpdateTimer()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer() => running = false;
}