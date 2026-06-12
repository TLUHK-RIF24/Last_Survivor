using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorHandler : MonoBehaviour
{
    private const string SelectedCharacterKey = "SelectedCharacter";
    private const string BaseIdleClipName = "Mage-Idle";
    private const string BaseRunClipName = "Mage-Run";
    private const string BaseCombatClipName = "Mage-Combat";

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private readonly List<KeyValuePair<AnimationClip, AnimationClip>> clipOverrides =
        new List<KeyValuePair<AnimationClip, AnimationClip>>();

    [Header("Base Animator")]
    [SerializeField] private RuntimeAnimatorController baseController;

    [Header("Characters")]
    [SerializeField] private CharacterData[] characters;

    private AnimatorOverrideController overrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null) return;
        if (baseController == null)
            baseController = animator.runtimeAnimatorController;

        if (baseController != null)
        {
            overrideController = new AnimatorOverrideController(baseController);
            animator.runtimeAnimatorController = overrideController;
        }

        LoadSelectedCharacter();
    }

    public void LoadCharacter(CharacterData data)
    {
        if (overrideController == null || data == null) return;

        clipOverrides.Clear();
        overrideController.GetOverrides(clipOverrides);

        ApplyClipOverride(BaseIdleClipName, data.idleClip);
        ApplyClipOverride(BaseRunClipName, data.runClip);
        ApplyClipOverride(BaseCombatClipName, data.combatClip);

        overrideController.ApplyOverrides(clipOverrides);

        animator.Rebind();
        animator.Update(0f);
    }

    private void LoadSelectedCharacter()
    {
        if (characters == null || characters.Length == 0) return;

        int selectedIndex = PlayerPrefs.GetInt(SelectedCharacterKey, 0);
        if (selectedIndex < 0 || selectedIndex >= characters.Length)
            selectedIndex = 0;

        LoadCharacter(characters[selectedIndex]);
    }

    private void ApplyClipOverride(string baseClipName, AnimationClip replacementClip)
    {
        if (replacementClip == null) return;

        for (int i = 0; i < clipOverrides.Count; i++)
        {
            AnimationClip originalClip = clipOverrides[i].Key;
            if (originalClip != null && originalClip.name == baseClipName)
            {
                clipOverrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, replacementClip);
                return;
            }
        }
    }

    public void SetRunning(bool isRunning)
    {
        if (animator != null)
            animator.SetBool("IsRunning", isRunning);
    }

    public void PlayAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void SetFacing(float moveDir)
    {
        if (spriteRenderer != null && moveDir != 0)
            spriteRenderer.flipX = moveDir < 0;
    }
}
