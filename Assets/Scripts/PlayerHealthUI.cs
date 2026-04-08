using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateBar(float current, float max)
    {
        healthSlider.value = current / max;
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}