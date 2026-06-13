using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("Card Background (rarity PNG)")]
    public Image cardBackground;
    public Sprite commonCard;
    public Sprite uncommonCard;
    public Sprite rareCard;
    public Sprite epicCard;

    [Header("Text Fields")]
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statChangeText; 

    [Header("Icon")]
    public Image iconImage;

    public Image rarityBanner;

    private UpgradeDefinition currentUpgrade;

    public void Setup(UpgradeDefinition upgrade)
    {
        currentUpgrade = upgrade;
        int currentLevel = UpgradeManager.Instance.GetUpgradeLevel(upgrade);

        // Set card background PNG based on rarity
        if (cardBackground != null)
        {
            Sprite card = GetCardSprite(upgrade.rarity);
            if (card != null) cardBackground.sprite = card;
        }

        // Text fields
        if (upgradeNameText != null)
            upgradeNameText.text = upgrade.upgradeName;

        if (descriptionText != null)
            descriptionText.text = upgrade.description;

        if (rarityText != null)
            rarityText.text = upgrade.rarity.ToString().ToUpper();

        if (levelText != null)
            levelText.text = currentLevel == 0 ? "" : $"Level {currentLevel} → {currentLevel + 1}";

        // Stat change text
        if (statChangeText != null)
            statChangeText.text = GetStatChangeText(upgrade);

        // Icon
        if (upgrade.icon != null && iconImage != null)
            iconImage.sprite = upgrade.icon;

        // Rarity banner color
        if (rarityBanner != null)
            rarityBanner.color = GetRarityColor(upgrade.rarity);
    }


    Sprite GetCardSprite(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:   return commonCard;
            case Rarity.Uncommon: return uncommonCard;
            case Rarity.Rare:     return rareCard;
            case Rarity.Epic:     return epicCard;
            default:              return commonCard;
        }
    }


    string GetStatChangeText(UpgradeDefinition upgrade)
    {
        if (upgrade.upgradeType != UpgradeType.Stat) return string.Empty;

        PlayerStats stats = PlayerStats.Instance;
        if (stats == null) return string.Empty;

        float current = 0f;
        float next    = 0f;
        string label  = string.Empty;

        switch (upgrade.statToUpgrade)
        {
            case StatType.Damage:
                current = stats.damage;
                next    = stats.damage + upgrade.bonusPerLevel;
                label   = "Damage";
                return $"{Mathf.RoundToInt(current)} → {Mathf.RoundToInt(next)} {label}";

            case StatType.FireRate:
                current = stats.fireRate;
                next    = Mathf.Max(0.15f, stats.fireRate - upgrade.bonusPerLevel);
                label   = "Fire Rate";
                return $"{current:F2}s → {next:F2}s {label}";

            case StatType.MoveSpeed:
                current = stats.moveSpeed;
                next    = stats.moveSpeed + upgrade.bonusPerLevel;
                label   = "Speed";
                return $"{current:F1} → {next:F1} {label}";

            case StatType.ProjectileSpeed:
                current = stats.projectileSpeed;
                next    = stats.projectileSpeed + upgrade.bonusPerLevel;
                label   = "Proj. Speed";
                return $"{Mathf.RoundToInt(current)} → {Mathf.RoundToInt(next)} {label}";

            default:
                return string.Empty;
        }
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