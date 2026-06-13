using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float timeSurvived = 0f;
    private bool  isDead       = false;

    [Header("Death Animation — Mage")]
    [SerializeField] private Sprite mageDeathFrame1;
    [SerializeField] private Sprite mageDeathFrame2;

    [Header("Death Animation — Archer")]
    [SerializeField] private Sprite archerDeathFrame1;
    [SerializeField] private Sprite archerDeathFrame2;

    private SpriteRenderer spriteRenderer;
    private int    selectedCharacter = 0;
    private Sprite killerSprite      = null;

    void Awake()
    {
        Instance          = this;
        spriteRenderer    = GetComponent<SpriteRenderer>();
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
    }

    void Start()
    {
        currentHealth = PlayerStats.Instance != null
            ? PlayerStats.Instance.maxHealth
            : maxHealth;
        UpdateUI();
    }

    void Update()
    {
        if (!isDead)
            timeSurvived += Time.deltaTime;
    }

    public void TakeDamage(float amount, Sprite sourceSprite = null)
    {
        if (isDead) return;

        if (sourceSprite != null)
            killerSprite = sourceSprite;

        currentHealth -= amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;

        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated      = false;
        }

        PlayerShooter shooter = GetComponent<PlayerShooter>();
        if (shooter != null) shooter.enabled = false;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        XPBarUI.Instance?.StopTimer();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
            e.SetActive(false);

        Transform shadow = transform.Find("Player_Shadow_0");
        if (shadow != null) shadow.gameObject.SetActive(false);

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        Sprite frame1 = null;
        Sprite frame2 = null;

        switch (selectedCharacter)
        {
            case 0: // Archer
                frame1 = archerDeathFrame1;
                frame2 = archerDeathFrame2;
                break;
            case 1: // Mage
                frame1 = mageDeathFrame1;
                frame2 = mageDeathFrame2;
                break;
        }

        if (frame1 != null && spriteRenderer != null)
            spriteRenderer.sprite = frame1;

        yield return new WaitForSeconds(1.2f);

        if (frame2 != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = frame2;
            if (selectedCharacter == 1) 
                spriteRenderer.flipX = true;
        }

        yield return new WaitForSeconds(1.5f);

        int   level = GameManager.Instance.GetCurrentLevel();
        float xp    = GameManager.Instance.GetCurrentXP();
        GameOverUI.Instance?.ShowGameOver(level, timeSurvived, xp, killerSprite);
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateUI();
    }

    void UpdateUI()
    {
        PlayerHealthUI.Instance?.UpdateBar(currentHealth, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth()     => maxHealth;
    public float GetTimeSurvived()  => timeSurvived;
}