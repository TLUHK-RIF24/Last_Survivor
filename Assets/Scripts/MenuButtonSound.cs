using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayClick();
    }
}