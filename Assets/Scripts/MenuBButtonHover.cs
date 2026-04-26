using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image  buttonImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null && hoverSprite != null)
            buttonImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null && normalSprite != null)
            buttonImage.sprite = normalSprite;
    }
}