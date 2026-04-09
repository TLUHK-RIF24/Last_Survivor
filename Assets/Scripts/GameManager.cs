using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("XP Settings")]
    [SerializeField] private float baseXP = 100f;
    [SerializeField] private float xpScaling = 1.4f;
    [SerializeField] private float xpScalingPerLevel = 50f;

    private int currentLevel = 1;
    private float currentXP = 0f;
    private float xpToNextLevel;

    void Awake()
    {
        Instance = this;
        xpToNextLevel = baseXP;
    }

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
            AddXP(xpToNextLevel);
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            xpToNextLevel = Mathf.Round(baseXP * Mathf.Pow(xpScaling, currentLevel));
            TriggerLevelUp();
        }

        XPBarUI.Instance?.UpdateBar(currentXP, xpToNextLevel);
    }

    public void TriggerLevelUp()
    {
        currentLevel++;
        LevelUpUI.Instance.Show(UpgradeManager.Instance.GetUpgradeChoices());
    }

    public int GetCurrentLevel() => currentLevel;
    public float GetCurrentXP() => currentXP;
    public float GetXPToNextLevel() => xpToNextLevel;
}