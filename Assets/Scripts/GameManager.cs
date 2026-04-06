using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int currentLevel = 1;

    [Header("XP Settings")]
    private float currentXP = 0f;
    private float xpToNextLevel = 100f;
    [SerializeField] private float baseXP = 100f;
    [SerializeField] private float xpScaling = 1.4f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
            AddXP(xpToNextLevel); // testimiseks täidab kohe XP bari
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
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