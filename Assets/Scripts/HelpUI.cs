using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Awake()
    {
        WireButton(ref backButton, "BackButton", OnBackClicked);
    }

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

    private void WireButton(ref Button button, string objectName, UnityAction action)
    {
        if (button == null)
            button = FindButton(objectName);

        if (button == null)
            return;

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private Button FindButton(string objectName)
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name == objectName)
                return button;
        }

        return null;
    }
}
