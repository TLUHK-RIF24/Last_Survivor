using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public List<UpgradeDefinition> allUpgrades;
    private Dictionary<UpgradeDefinition, int> playerUpgrades = new Dictionary<UpgradeDefinition, int>();

    void Awake() { Instance = this; }

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
        foreach (UpgradeDefinition u in upgrades) totalWeight += GetRarityWeight(u.rarity);
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (UpgradeDefinition u in upgrades)
        {
            cumulative += GetRarityWeight(u.rarity);
            if (roll <= cumulative) return u;
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
        if (playerUpgrades.ContainsKey(upgrade)) playerUpgrades[upgrade]++;
        else playerUpgrades[upgrade] = 1;
        int level = playerUpgrades[upgrade];

        if (upgrade.upgradeType == UpgradeType.Stat) ApplyStatUpgrade(upgrade);
        else if (upgrade.upgradeType == UpgradeType.Ability) ApplyAbilityUpgrade(upgrade, level);
    }

    void ApplyStatUpgrade(UpgradeDefinition upgrade)
    {
        PlayerStats stats = PlayerStats.Instance;
        switch (upgrade.statToUpgrade)
        {
            case StatType.Damage:          stats.damage          += upgrade.bonusPerLevel; break;
            case StatType.FireRate:        stats.fireRate        = Mathf.Max(0.05f, stats.fireRate - upgrade.bonusPerLevel); break;
            case StatType.MoveSpeed:       stats.moveSpeed       += upgrade.bonusPerLevel; break;
            case StatType.ProjectileSpeed: stats.projectileSpeed += upgrade.bonusPerLevel; break;
        }
    }

    void ApplyAbilityUpgrade(UpgradeDefinition upgrade, int level)
    {
        GameObject player = PlayerStats.Instance.gameObject;
        switch (upgrade.abilityType)
        {
            case AbilityType.AuraField:
                AuraField aura = player.GetComponent<AuraField>();
                if (aura == null) aura = player.AddComponent<AuraField>();
                aura.LevelUp(level); break;

            case AbilityType.Shotgun:
                ShotgunAbility shotgun = player.GetComponent<ShotgunAbility>();
                if (shotgun == null) shotgun = player.AddComponent<ShotgunAbility>();
                shotgun.LevelUp(level); break;

            case AbilityType.Orbital:
                OrbitalAbility orbital = player.GetComponent<OrbitalAbility>();
                if (orbital == null) orbital = player.AddComponent<OrbitalAbility>();
                orbital.LevelUp(level); break;

            case AbilityType.Boomerang:
                BoomerangAbility boomerang = player.GetComponent<BoomerangAbility>();
                if (boomerang == null) boomerang = player.AddComponent<BoomerangAbility>();
                boomerang.LevelUp(level); break;

            case AbilityType.LightningChain:
                LightningChainAbility lightning = player.GetComponent<LightningChainAbility>();
                if (lightning == null) lightning = player.AddComponent<LightningChainAbility>();
                lightning.LevelUp(level); break;

            case AbilityType.PiercingArrow:
                PiercingArrowAbility piercing = player.GetComponent<PiercingArrowAbility>();
                if (piercing == null) piercing = player.AddComponent<PiercingArrowAbility>();
                piercing.LevelUp(level); break;

            case AbilityType.DashTrail:
                DashTrailAbility dashTrail = player.GetComponent<DashTrailAbility>();
                if (dashTrail == null) dashTrail = player.AddComponent<DashTrailAbility>();
                dashTrail.LevelUp(level); break;

            case AbilityType.MagnetField:
                MagnetFieldAbility magnet = player.GetComponent<MagnetFieldAbility>();
                if (magnet == null) magnet = player.AddComponent<MagnetFieldAbility>();
                magnet.LevelUp(level); break;

            case AbilityType.BouncingShot:
                BouncingShotAbility bouncing = player.GetComponent<BouncingShotAbility>();
                if (bouncing == null) bouncing = player.AddComponent<BouncingShotAbility>();
                bouncing.LevelUp(level); break;
        }
    }

    public int GetUpgradeLevel(UpgradeDefinition upgrade)
    {
        return playerUpgrades.ContainsKey(upgrade) ? playerUpgrades[upgrade] : 0;
    }
}