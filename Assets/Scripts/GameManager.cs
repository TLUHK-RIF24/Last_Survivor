using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int   currentLevel    = 1;
    private float currentXP       = 0f;
    private float xpToNextLevel   = 5f;   

    private float growthMultiplier = 1f;


    void Awake()
    {
        Instance = this;
        xpToNextLevel = GetXPRequired(currentLevel);
    }

    //  XP — three-phase VS curve
    //
    //  Phase 1  L1–20:  starts at 5, +10 per level    (5, 15, 25 … 195)
    //  Phase 2  L21–40: starts at 205, +13 per level  (208, 221 … 455)
    //  Phase 3  L41+:   starts at 471, +16 per level  (471, 487 …)

    public static float GetXPRequired(int level)
    {
        if (level <= 20)
            return 5f + (level - 1) * 10f;
        else if (level <= 40)
            return 195f + (level - 20) * 13f;
        else
            return 455f + (level - 40) * 16f;
    }

    public void AddXP(float amount)
    {
        currentXP += amount * growthMultiplier;
        XPBarUI.Instance?.UpdateBar(currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            currentXP     -= xpToNextLevel;
            TriggerLevelUp();
            xpToNextLevel  = GetXPRequired(currentLevel);
        }

        XPBarUI.Instance?.UpdateBar(currentXP, xpToNextLevel);
    }

    public void TriggerLevelUp()
    {
        currentLevel++;
        LevelUpUI.Instance?.Show(UpgradeManager.Instance?.GetUpgradeChoices());
    }


    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public int   GetCurrentLevel()  => currentLevel;
    public float GetCurrentXP()     => currentXP;
    public float GetXPToNextLevel() => xpToNextLevel;
    public float GetGrowthMultiplier() => growthMultiplier;

    public void SetGrowthMultiplier(float mult)
    {
        growthMultiplier = Mathf.Max(0.1f, mult);
    }
}