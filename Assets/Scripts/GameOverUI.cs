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
    [SerializeField] private Image     gameOverTitleImage;
    [SerializeField] private Button    nextButton;
    [SerializeField] private Image     nextButtonImage;

    [Header("Results Panel")]
    [SerializeField] private Image     gameTitleImage;
    [SerializeField] private Image     resultsBgImage;
    [SerializeField] private Image     resultsHeaderImage;
    [SerializeField] private Button    quitButton;

    [Header("Stat Rows  (label + value pairs)")]
    [SerializeField] private TMP_Text  levelLabel;
    [SerializeField] private TMP_Text  levelValue;
    [SerializeField] private TMP_Text  timeLabel;
    [SerializeField] private TMP_Text  timeValue;
    [SerializeField] private TMP_Text  xpLabel;
    [SerializeField] private TMP_Text  xpValue;

    [Header("Colors")]
    [SerializeField] private Color normalStatColor  = Color.white;
    [SerializeField] private Color recordStatColor  = new Color(1f, 0.85f, 0.1f); // gold

    [Header("Timing")]
    [SerializeField] private float statFadeDelay    = 0.35f;  // seconds between each stat appearing
    [SerializeField] private float statFadeDuration = 0.4f;   // how long each fade takes

    [Header("Ambient Particles")]
    [SerializeField] private ParticleSystem ambientParticles;

    // ── PlayerStats are passed in from PlayerHealth on death ─────────────────
    private int   finalLevel;
    private float finalTime;
    private float finalXP;

    // ── Records (saved in PlayerPrefs) ───────────────────────────────────────
    private const string PREF_BEST_LEVEL = "BestLevel";
    private const string PREF_BEST_TIME  = "BestTime";
    private const string PREF_BEST_XP    = "BestXP";

    void Awake()
    {
        Instance = this;
        gameOverScreen.SetActive(false);
    }

    // ── Called by PlayerHealth when the player dies ───────────────────────────

    public void ShowGameOver(int level, float timeSurvived, float xp)
    {
        finalLevel = level;
        finalTime  = timeSurvived;
        finalXP    = xp;

        Time.timeScale = 0f;

        gameOverScreen.SetActive(true);
        gameOverPanel.SetActive(true);
        resultsPanel.SetActive(false);

        // Fade in the game over title and button
        StartCoroutine(FadeInGameOverPanel());

        // Start ambient particles
        if (ambientParticles != null)
            ambientParticles.Play();
    }

    // ── NEXT button ───────────────────────────────────────────────────────────

    public void OnNextClicked()
    {
        gameOverPanel.SetActive(false);
        resultsPanel.SetActive(true);

        // Hide all stat rows immediately — they fade in one by one
        SetStatAlpha(levelLabel,  0f); SetStatAlpha(levelValue,  0f);
        SetStatAlpha(timeLabel,   0f); SetStatAlpha(timeValue,   0f);
        SetStatAlpha(xpLabel,     0f); SetStatAlpha(xpValue,     0f);

        StartCoroutine(FadeInResults());
    }

    // ── QUIT button ───────────────────────────────────────────────────────────

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Fade in game over panel ───────────────────────────────────────────────

    private IEnumerator FadeInGameOverPanel()
    {
        // Fade in uses unscaled time because timeScale is 0
        SetImageAlpha(gameOverTitleImage, 0f);
        SetImageAlpha(nextButtonImage,    0f);

        yield return FadeImageUnscaled(gameOverTitleImage, 0f, 1f, 0.6f);
        yield return new WaitForSecondsRealtime(0.2f);
        yield return FadeImageUnscaled(nextButtonImage, 0f, 1f, 0.4f);
    }

    // ── Fade in results one by one ────────────────────────────────────────────

    private IEnumerator FadeInResults()
    {
        // Check records
        int   bestLevel = PlayerPrefs.GetInt(PREF_BEST_LEVEL, 0);
        float bestTime  = PlayerPrefs.GetFloat(PREF_BEST_TIME, 0f);
        float bestXP    = PlayerPrefs.GetFloat(PREF_BEST_XP, 0f);

        bool newBestLevel = finalLevel > bestLevel;
        bool newBestTime  = finalTime  > bestTime;
        bool newBestXP    = finalXP    > bestXP;

        // Save new records
        if (newBestLevel) PlayerPrefs.SetInt(PREF_BEST_LEVEL, finalLevel);
        if (newBestTime)  PlayerPrefs.SetFloat(PREF_BEST_TIME, finalTime);
        if (newBestXP)    PlayerPrefs.SetFloat(PREF_BEST_XP, finalXP);
        PlayerPrefs.Save();

        // Set text content
        levelValue.text = finalLevel.ToString();
        timeValue.text  = FormatTime(finalTime);
        xpValue.text    = Mathf.RoundToInt(finalXP).ToString();

        // Apply record colors
        Color lc = newBestLevel ? recordStatColor : normalStatColor;
        Color tc = newBestTime  ? recordStatColor : normalStatColor;
        Color xc = newBestXP   ? recordStatColor : normalStatColor;

        levelLabel.color = lc; levelValue.color = lc;
        timeLabel.color  = tc; timeValue.color  = tc;
        xpLabel.color    = xc; xpValue.color    = xc;

        // Fade in each row with a delay between them
        yield return FadeStatRowUnscaled(levelLabel, levelValue, statFadeDuration);
        yield return new WaitForSecondsRealtime(statFadeDelay);

        yield return FadeStatRowUnscaled(timeLabel, timeValue, statFadeDuration);
        yield return new WaitForSecondsRealtime(statFadeDelay);

        yield return FadeStatRowUnscaled(xpLabel, xpValue, statFadeDuration);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private IEnumerator FadeStatRowUnscaled(TMP_Text label, TMP_Text value, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetStatAlpha(label, t);
            SetStatAlpha(value, t);
            yield return null;
        }
        SetStatAlpha(label, 1f);
        SetStatAlpha(value, 1f);
    }

    private IEnumerator FadeImageUnscaled(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetImageAlpha(img, Mathf.Lerp(from, to, t));
            yield return null;
        }
        SetImageAlpha(img, to);
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