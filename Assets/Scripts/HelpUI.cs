using UnityEngine;

public class HelpUI : MonoBehaviour
{
    public void OnBackClicked()
    {
        MainMenuUI menu = FindFirstObjectByType<MainMenuUI>();
        if (menu != null) menu.ShowMain();
    }

    public void OnContinueClicked()
    {
        MainMenuUI menu = FindFirstObjectByType<MainMenuUI>();
        if (menu != null) menu.ShowMain();
    }
}