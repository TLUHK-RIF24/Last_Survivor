using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("XP Settings")]
    [SerializeField] private float baseXPToLevel = 100f;
    [SerializeField] private float xpScalingPerLevel = 50f;  // each level needs 50 more XP than the last

    private int   currentLevel = 1;
    private float currentXP   = 0f;
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

        // Update the XP bar UI
        XPBarUI.Instance?.UpdateBar(currentXP, xpToNextLevel);

        // Level up if threshold reached
        while (currentXP >= xpToNextLevel)
        {
            currentXP    -= xpToNextLevel;
            xpToNextLevel = baseXPToLevel + (currentLevel * xpScalingPerLevel);
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

    // ── Getters ──────────────────────────────────────────────────────────────

    public int   GetCurrentLevel()  => currentLevel;
    public float GetCurrentXP()     => currentXP;
    public float GetXPToNextLevel() => xpToNextLevel;
}