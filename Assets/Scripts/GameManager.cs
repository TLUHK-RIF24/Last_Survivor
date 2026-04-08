using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            AddXP(xpToNextLevel);

        // TESTIMISEKS
        if (Keyboard.current.hKey.wasPressedThisFrame)
            PlayerHealth.Instance?.TakeDamage(25f);

        if (Keyboard.current.kKey.wasPressedThisFrame)
            PlayerHealth.Instance?.TakeDamage(999f);
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

    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetCurrentLevel() => currentLevel;
    public float GetCurrentXP() => currentXP;
    public float GetXPToNextLevel() => xpToNextLevel;
}