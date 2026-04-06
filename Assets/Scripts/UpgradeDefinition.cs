using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic }
public enum StatType { Damage, FireRate, MoveSpeed, ProjectileSpeed }
public enum UpgradeType { Stat, Ability }
public enum AbilityType { None, AuraField, Shotgun, Orbital, Boomerang, LightningChain, PiercingArrow, DashTrail, MagnetField, BouncingShot }

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public Rarity rarity;

    [Header("Upgrade Type")]
    public UpgradeType upgradeType = UpgradeType.Stat;

    [Header("Stat Upgrade (if type is Stat)")]
    public StatType statToUpgrade;
    public float bonusPerLevel;

    [Header("Ability Upgrade (if type is Ability)")]
    public AbilityType abilityType = AbilityType.None;
}