using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float timeSurvived = 0f;
    private bool isDead = false;
    public GameObject floatingTextPrefab;   // Assign the prefab here
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
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;

        int level = GameManager.Instance.GetCurrentLevel();
        float xp = GameManager.Instance.GetCurrentXP();

        GameOverUI.Instance?.ShowGameOver(level, timeSurvived, xp);
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateUI();

    if (floatingTextPrefab != null)
            {
                Vector3 spawnPos = transform.position + new Vector3(0, 1.2f, 0); // Adjust height here

                GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

                FloatingText ft = textObj.GetComponent<FloatingText>();
                if (ft != null)
                {
                    ft.Setup("+" + amount, Color.green);
                }
            }

        Debug.Log($"Healed +{amount} HP!");
    }

    void UpdateUI()
    {
        PlayerHealthUI.Instance?.UpdateBar(currentHealth, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetTimeSurvived() => timeSurvived;
}
