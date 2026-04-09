using UnityEngine;

// Telegraphs with an orange flash, then dashes at high speed
public class EnemyAI_Charger : BaseEnemy
{
    [Header("Charger Settings")]
    public float telegraphDuration = 0.9f;
    public float dashSpeed         = 18f;
    public float dashDuration      = 0.25f;
    public float cooldownDuration  = 1.2f;
    public Color telegraphColor    = new Color(1f, 0.4f, 0f);

    private enum State { Approach, Telegraph, Dash, Cooldown }
    private State   state = State.Approach;
    private float   stateTimer;
    private Vector2 dashDir;
    private Color   baseColor;

    private static readonly int enemyLayer = -1;   // cached at first use

    protected override void OnSpawnExtra()
    {
        state      = State.Approach;
        stateTimer = 0f;
        if (spriteRenderer != null) baseColor = spriteRenderer.color;
    }

    protected override void UpdateAI()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Approach:
                MoveTowardsPlayer();
                if (DistanceToPlayer() < 5f) EnterTelegraph();
                break;

            case State.Telegraph:
                rb.linearVelocity = Vector2.zero;
                if (spriteRenderer != null)
                {
                    float t = Mathf.PingPong(stateTimer * 6f, 1f);
                    spriteRenderer.color = Color.Lerp(baseColor, telegraphColor, t);
                }
                if (stateTimer >= telegraphDuration) EnterDash();
                break;

            case State.Dash:
                rb.linearVelocity = dashDir * dashSpeed;
                if (stateTimer >= dashDuration) EnterCooldown();
                break;

            case State.Cooldown:
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 8f);
                if (stateTimer >= cooldownDuration)
                {
                    state      = State.Approach;
                    stateTimer = 0f;
                    if (spriteRenderer != null) spriteRenderer.color = baseColor;
                }
                break;
        }
    }

    private void EnterTelegraph()
    {
        state      = State.Telegraph;
        stateTimer = 0f;
        dashDir    = DirectionToPlayer();
    }

    private void EnterDash()
    {
        state      = State.Dash;
        stateTimer = 0f;
        if (spriteRenderer != null) spriteRenderer.color = baseColor;

        // Ignore enemy-enemy collisions during the dash so nothing blocks it
        int layer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(layer, layer, true);
    }

    private void EnterCooldown()
    {
        state      = State.Cooldown;
        stateTimer = 0f;

        // Re-enable enemy-enemy collisions after the dash
        int layer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(layer, layer, false);
    }

    // Make sure collisions are always restored if this enemy is recycled mid-dash
    public override void OnSpawn(float hpMult, float speedMult, float damageMult)
    {
        int layer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(layer, layer, false);
        base.OnSpawn(hpMult, speedMult, damageMult);
    }
}