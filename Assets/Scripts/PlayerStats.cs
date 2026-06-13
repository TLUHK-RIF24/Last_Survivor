using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Movement")]
    public float moveSpeed = 4.0f;

    [Header("Shooting")]
    public float damage          = 10f;
    public float fireRate        = 1.0f;
    public float projectileSpeed = 10f;

    [Header("Health")]
    public float maxHealth = 100f;

    void Awake()
    {
        Instance = this;
        ApplyCharacterStats();
    }

    void ApplyCharacterStats()
    {
        int character = PlayerPrefs.GetInt("SelectedCharacter", 0);

        switch (character)
        {
            case 0: // Archer
                moveSpeed       = 4.0f;
                damage          = 12f;
                fireRate        = 0.6f;
                projectileSpeed = 14f;
                maxHealth       = 100f;
                break;

            case 1: // Mage
                moveSpeed       = 3.5f;
                damage          = 18f;
                fireRate        = 1.8f;
                projectileSpeed = 7f;
                maxHealth       = 100f;
                break;

            default: // Knight
                moveSpeed       = 4.0f;
                damage          = 10f;
                fireRate        = 1.0f;
                projectileSpeed = 10f;
                maxHealth       = 150f;
                break;
        }
    }
}