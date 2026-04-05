using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI levelText;
    public Image iconImage;
    public Image rarityBanner;

    private UpgradeDefinition currentUpgrade;

    public void Setup(UpgradeDefinition upgrade)
    {
        currentUpgrade = upgrade;
        int currentLevel = UpgradeManager.Instance.GetUpgradeLevel(upgrade);

        upgradeNameText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        rarityText.text = upgrade.rarity.ToString().ToUpper();
        levelText.text = currentLevel == 0 ? "NEW!" : $"Level {currentLevel} -> {currentLevel + 1}";

        if (upgrade.icon != null && iconImage != null)
            iconImage.sprite = upgrade.icon;

        if (rarityBanner != null)
            rarityBanner.color = GetRarityColor(upgrade.rarity);
    }

    Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:   return new Color(0.6f, 0.6f, 0.6f);
            case Rarity.Uncommon: return new Color(0.2f, 0.7f, 0.2f);
            case Rarity.Rare:     return new Color(0.2f, 0.4f, 0.9f);
            case Rarity.Epic:     return new Color(0.6f, 0.1f, 0.8f);
            default:              return Color.white;
        }
    }

    public void OnClick()
    {
        UpgradeManager.Instance.ApplyUpgrade(currentUpgrade);
        LevelUpUI.Instance.Hide();
    }
}