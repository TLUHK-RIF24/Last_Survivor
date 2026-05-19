// CharacterData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Roguelike/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;

    public RuntimeAnimatorController animatorController; // Optional if all use same

    // Sprite sheets / Animation Clips
    public AnimationClip idleClip;
    public AnimationClip runClip;
    public AnimationClip combatClip;

    // You can also put stats here later: health, speed, etc.
}
