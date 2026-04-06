using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Shooting")]
    public float damage = 10f;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;

    void Awake()
    {
        Instance = this;
    }
}