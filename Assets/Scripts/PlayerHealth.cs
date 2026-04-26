using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float timeSurvived = 0f;
    private bool  isDead       = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        if (!isDead)
            timeSurvived += Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;

        int   level = GameManager.Instance.GetCurrentLevel();
        float xp    = GameManager.Instance.GetCurrentXP();

        GameOverUI.Instance?.ShowGameOver(level, timeSurvived, xp);
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