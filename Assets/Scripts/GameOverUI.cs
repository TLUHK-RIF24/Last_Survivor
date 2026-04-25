using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("Screen Roots")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject resultsPanel;

    [Header("Game Over Panel")]
    [SerializeField] private Image  gameOverTitleImage;
    [SerializeField] private Image  nextButtonImage;

    [Header("Results Panel")]
    [SerializeField] private Image  gameTitleImage;
    [SerializeField] private Image  resultsBgImage;
    [SerializeField] private Image  resultsHeaderImage;

    [Header("Stat Rows  (label + value pairs)")]
    [SerializeField] private TMP_Text levelLabel;
    [SerializeField] private TMP_Text levelValue;
    [SerializeField] private TMP_Text timeLabel;
    [SerializeField] private TMP_Text timeValue;
    [SerializeField] private TMP_Text xpLabel;
    [SerializeField] private TMP_Text xpValue;

    [Header("Colors")]
    [SerializeField] private Color normalStatColor = Color.white;
    [SerializeField] private Color recordStatColor = new Color(1f, 0.85f, 0.1f);

    [Header("Timing")]
    [SerializeField] private float statFadeDelay    = 0.35f;
    [SerializeField] private float statFadeDuration = 0.4f;

    [Header("Ambient Particles")]
    [SerializeField] private ParticleSystem ambientParticles;

    // ── Runtime data ─────────────────────────────────────────────────────────
    private int   finalLevel;
    private float finalTime;
    private float finalXP;

    // ── PlayerPrefs keys ─────────────────────────────────────────────────────
    private const string PREF_BEST_LEVEL = "BestLevel";
    private const string PREF_BEST_TIME  = "BestTime";
    private const string PREF_BEST_XP    = "BestXP";

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        Instance = this;
        gameOverScreen.SetActive(false);
    }

    // ── Called by PlayerHealth.Die() ─────────────────────────────────────────

    public void ShowGameOver(int level, float timeSurvived, float xp)
    {
        finalLevel = level;
        finalTime  = timeSurvived;
        finalXP    = xp;

        Time.timeScale = 0f;

        gameOverScreen.SetActive(true);
        gameOverPanel.SetActive(true);
        resultsPanel.SetActive(false);

        if (ambientParticles != null)
            ambientParticles.Play();

        StartCoroutine(FadeInGameOverPanel());
    }

    // ── Button callbacks ─────────────────────────────────────────────────────

    public void OnNextClicked()
    {
        gameOverPanel.SetActive(false);
        resultsPanel.SetActive(true);

        // Deactivate all stat rows — they activate and fade in one by one
        SetRowActive(levelLabel, levelValue, false);
        SetRowActive(timeLabel,  timeValue,  false);
        SetRowActive(xpLabel,    xpValue,    false);

        StartCoroutine(FadeInResults());
    }

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Coroutines ────────────────────────────────────────────────────────────

    private IEnumerator FadeInGameOverPanel()
    {
        SetImageAlpha(gameOverTitleImage, 0f);
        SetImageAlpha(nextButtonImage,    0f);

        yield return FadeImageUnscaled(gameOverTitleImage, 0f, 1f, 0.6f);
        yield return new WaitForSecondsRealtime(0.2f);
        yield return FadeImageUnscaled(nextButtonImage, 0f, 1f, 0.4f);
    }

    private IEnumerator FadeInResults()
    {
        // Load previous records
        int   bestLevel = PlayerPrefs.GetInt(PREF_BEST_LEVEL, 0);
        float bestTime  = PlayerPrefs.GetFloat(PREF_BEST_TIME, 0f);
        float bestXP    = PlayerPrefs.GetFloat(PREF_BEST_XP,   0f);

        bool newBestLevel = finalLevel > bestLevel;
        bool newBestTime  = finalTime  > bestTime;
        bool newBestXP    = finalXP    > bestXP;

        // Save new records
        if (newBestLevel) PlayerPrefs.SetInt(PREF_BEST_LEVEL, finalLevel);
        if (newBestTime)  PlayerPrefs.SetFloat(PREF_BEST_TIME, finalTime);
        if (newBestXP)    PlayerPrefs.SetFloat(PREF_BEST_XP,   finalXP);
        PlayerPrefs.Save();

        // Set text content
        levelValue.text = finalLevel.ToString();
        timeValue.text  = FormatTime(finalTime);
        xpValue.text    = Mathf.RoundToInt(finalXP).ToString();

        // Apply record highlight colors
        ApplyRowColor(levelLabel, levelValue, newBestLevel ? recordStatColor : normalStatColor);
        ApplyRowColor(timeLabel,  timeValue,  newBestTime  ? recordStatColor : normalStatColor);
        ApplyRowColor(xpLabel,    xpValue,    newBestXP    ? recordStatColor : normalStatColor);

        // Fade each row in one at a time
        yield return FadeStatRowUnscaled(levelLabel, levelValue, statFadeDuration);
        yield return new WaitForSecondsRealtime(statFadeDelay);

        yield return FadeStatRowUnscaled(timeLabel, timeValue, statFadeDuration);
        yield return new WaitForSecondsRealtime(statFadeDelay);

        yield return FadeStatRowUnscaled(xpLabel, xpValue, statFadeDuration);
    }

    // ── Per-row fade ─────────────────────────────────────────────────────────

    private IEnumerator FadeStatRowUnscaled(TMP_Text label, TMP_Text value, float duration)
    {
        // Activate then immediately set invisible before first render
        SetRowActive(label, value, true);
        SetStatAlpha(label, 0f);
        SetStatAlpha(value, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetStatAlpha(label, t);
            SetStatAlpha(value, t);
            yield return new WaitForEndOfFrame();
        }

        SetStatAlpha(label, 1f);
        SetStatAlpha(value, 1f);
    }

    // ── Image fade ────────────────────────────────────────────────────────────

    private IEnumerator FadeImageUnscaled(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetImageAlpha(img, Mathf.Lerp(from, to, t));
            yield return new WaitForEndOfFrame();
        }
        SetImageAlpha(img, to);
    }

    // ── Small helpers ─────────────────────────────────────────────────────────

    private void SetRowActive(TMP_Text label, TMP_Text value, bool active)
    {
        if (label != null) label.gameObject.SetActive(active);
        if (value != null) value.gameObject.SetActive(active);
    }

    private void ApplyRowColor(TMP_Text label, TMP_Text value, Color color)
    {
        if (label != null) label.color = color;
        if (value != null) value.color = color;
    }

    private void SetImageAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color; c.a = a; img.color = c;
    }

    private void SetStatAlpha(TMP_Text t, float a)
    {
        if (t == null) return;
        Color c = t.color; c.a = a; t.color = c;
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}