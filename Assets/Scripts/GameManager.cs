using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("XP Settings")]
    [SerializeField] private float baseXPToLevel = 100f;
    [SerializeField] private float xpScalingPerLevel = 50f;

    private int   currentLevel   = 1;
    private float currentXP      = 0f;
    private float xpToNextLevel;

    void Awake()
    {
        Instance = this;
        xpToNextLevel = baseXPToLevel;
    }

    // TESTIMISEKS, VAJUTADES L, NAITAB LEVEL UP EKRAANI UI'D
    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
            TriggerLevelUp();
    }

    // ── XP ───────────────────────────────────────────────────────────────────

    public void AddXP(float amount)
    {
        currentXP += amount;
        XPBarUI.Instance?.UpdateBar(currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            currentXP     -= xpToNextLevel;
            xpToNextLevel  = baseXPToLevel + (currentLevel * xpScalingPerLevel);
            TriggerLevelUp();
        }
    }

    // ── Level up ─────────────────────────────────────────────────────────────

    // DENISSI XP SUSTEEM CALLIB SELLE KUI MANGIJA SAAB LEVEL UPI
    public void TriggerLevelUp()
    {
        currentLevel++;
        LevelUpUI.Instance.Show(UpgradeManager.Instance.GetUpgradeChoices());
    }

    // ── Death / reset ─────────────────────────────────────────────────────────

    public void ResetGame()
    {
        // Reload the current scene — resets everything
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Getters ──────────────────────────────────────────────────────────────

    public int   GetCurrentLevel()  => currentLevel;
    public float GetCurrentXP()     => currentXP;
    public float GetXPToNextLevel() => xpToNextLevel;
}