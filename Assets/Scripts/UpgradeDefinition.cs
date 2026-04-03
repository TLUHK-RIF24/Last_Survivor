using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic }
public enum StatType { Damage, FireRate, MoveSpeed, ProjectileSpeed }

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public Rarity rarity;

    [Header("Scaling")]
    public StatType statToUpgrade;
    public float bonusPerLevel;
}