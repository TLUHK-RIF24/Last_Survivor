using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public static PlayerHealthUI Instance;

    [SerializeField] private Image hpFillImage;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateBar(float current, float max)
    {
        hpFillImage.fillAmount = current / max;
    }
}