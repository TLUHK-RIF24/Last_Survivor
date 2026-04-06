using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    public static XPBarUI Instance;

    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text levelText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateBar(float currentXP, float maxXP)
    {
        xpSlider.value = currentXP / maxXP;
        levelText.text = $"Level {GameManager.Instance.GetCurrentLevel()}";
    }
}