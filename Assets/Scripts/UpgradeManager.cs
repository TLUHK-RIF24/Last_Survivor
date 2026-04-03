using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeDefinition> allUpgrades;

    private Dictionary<UpgradeDefinition, int> playerUpgrades = new Dictionary<UpgradeDefinition, int>();

    void Awake()
    {
        Instance = this;
    }

    public List<UpgradeDefinition> GetUpgradeChoices()
    {
        List<UpgradeDefinition> available = new List<UpgradeDefinition>(allUpgrades);
        List<UpgradeDefinition> choices = new List<UpgradeDefinition>();

        for (int i = 0; i < 3 && available.Count > 0; i++)
        {
            UpgradeDefinition pick = WeightedPick(available);
            choices.Add(pick);
            available.Remove(pick);
        }

        return choices;
    }

    UpgradeDefinition WeightedPick(List<UpgradeDefinition> upgrades)
    {
        float totalWeight = 0f;
        foreach (UpgradeDefinition u in upgrades)
            totalWeight += GetRarityWeight(u.rarity);

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (UpgradeDefinition u in upgrades)
        {
            cumulative += GetRarityWeight(u.rarity);
            if (roll <= cumulative)
                return u;
        }

        return upgrades[upgrades.Count - 1];
    }

    float GetRarityWeight(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:   return 100f;
            case Rarity.Uncommon: return 50f;
            case Rarity.Rare:     return 20f;
            case Rarity.Epic:     return 5f;
            default:              return 100f;
        }
    }

    public void ApplyUpgrade(UpgradeDefinition upgrade)
    {
        if (playerUpgrades.ContainsKey(upgrade))
            playerUpgrades[upgrade]++;
        else
            playerUpgrades[upgrade] = 1;

        PlayerStats stats = PlayerStats.Instance;
        switch (upgrade.statToUpgrade)
        {
            case StatType.Damage:          stats.damage          += upgrade.bonusPerLevel; break;
            case StatType.FireRate:        stats.fireRate        =  Mathf.Max(0.05f, stats.fireRate - upgrade.bonusPerLevel); break;
            case StatType.MoveSpeed:       stats.moveSpeed       += upgrade.bonusPerLevel; break;
            case StatType.ProjectileSpeed: stats.projectileSpeed += upgrade.bonusPerLevel; break;
        }
    }

    public int GetUpgradeLevel(UpgradeDefinition upgrade)
    {
        return playerUpgrades.ContainsKey(upgrade) ? playerUpgrades[upgrade] : 0;
    }
}