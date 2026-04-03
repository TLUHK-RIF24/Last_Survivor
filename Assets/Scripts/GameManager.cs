using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int currentLevel = 1;

    void Awake()
    {
        Instance = this;
    }
    // TESTIMISEKS, VAJUTADES L, NAITAB LEVEL UP EKRAANI UI'D
    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
            TriggerLevelUp();
    }
    // DENISSI XP SUSTEEM CALLIB SELLE KUI MANGIJA SAAB LEVEL UPI
    public void TriggerLevelUp()
    {
        currentLevel++;
        LevelUpUI.Instance.Show(UpgradeManager.Instance.GetUpgradeChoices());
    }

    public int GetCurrentLevel() => currentLevel;

    
}
