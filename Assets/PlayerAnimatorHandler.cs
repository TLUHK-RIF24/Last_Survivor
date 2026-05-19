using UnityEngine;

public class PlayerAnimatorHandler : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Base Animator")]
    public RuntimeAnimatorController baseController;

    private AnimatorOverrideController overrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (baseController != null)
        {
            overrideController = new AnimatorOverrideController(baseController);
            animator.runtimeAnimatorController = overrideController;
        }
    }

    public void LoadCharacter(CharacterData data)
    {
        if (overrideController == null || data == null) return;

        overrideController["Idle"] = data.idleClip;
        overrideController["Run"] = data.runClip;
        overrideController["Combat"] = data.combatClip;   // or "Attack"

        animator.Update(0f);
    }

    public void SetRunning(bool isRunning)
    {
        animator.SetBool("IsRunning", isRunning);
    }

    // 🔥 This is the important one for you
    public void PlayAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void SetFacing(float moveDir)
    {
        if (moveDir != 0)
            spriteRenderer.flipX = moveDir < 0;
    }
}
