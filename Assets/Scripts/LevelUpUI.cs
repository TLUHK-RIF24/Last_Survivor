using UnityEngine;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI Instance;

    public GameObject levelUpPanel;
    public UpgradeButton[] upgradeButtons;

    void Awake()
    {
        Instance = this;
        levelUpPanel.SetActive(false);
    }

    public void Show(List<UpgradeDefinition> choices)
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                upgradeButtons[i].Setup(choices[i]);
            }
        }
    }

    public void Hide()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}