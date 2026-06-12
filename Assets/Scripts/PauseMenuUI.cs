using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    [Header("Pause Screen Root")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Main Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseFrame;
    [SerializeField] private GameObject pauseTitle;

    [Header("Help Panel")]
    [SerializeField] private GameObject helpPanel;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Confirmation Popup")]
    [SerializeField] private GameObject confirmPopup;
    [SerializeField] private TMP_Text   confirmText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button leaveGameButton;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pauseScreen.SetActive(false);
        confirmPopup.SetActive(false);

        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (confirmPopup.activeSelf)
            {
                OnConfirmNo();
            }
            else if (helpPanel != null && helpPanel.activeSelf)
            {
                OnHelpBackClicked();
            }
            else if (optionsPanel != null && optionsPanel.activeSelf)
            {
                OnOptionsBackClicked();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // ── Pause / Resume ────────────────────────────────────────────────────────

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        pausePanel.SetActive(true);
        confirmPopup.SetActive(false);
        SetMainPauseViewVisible(true);

        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        confirmPopup.SetActive(false);
        SetMainPauseViewVisible(true);

        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // ── Button callbacks ─────────────────────────────────────────────────────

    private void SetMainPauseButtonsVisible(bool visible)
    {
        if (continueButton != null)
            continueButton.gameObject.SetActive(visible);

        if (optionsButton != null)
            optionsButton.gameObject.SetActive(visible);

        if (helpButton != null)
            helpButton.gameObject.SetActive(visible);

        if (leaveGameButton != null)
            leaveGameButton.gameObject.SetActive(visible);
    }

    private void SetMainPauseViewVisible(bool visible)
    {
        if (pauseTitle != null)
            pauseTitle.SetActive(visible);

        if (pauseFrame != null)
            pauseFrame.SetActive(visible);

        SetMainPauseButtonsVisible(visible);
    }

    public void OnContinueClicked()
    {
        Resume();
    }

    public void OnOptionsClicked()
    {
        if (optionsPanel == null)
            return;

        pausePanel.SetActive(true);
        confirmPopup.SetActive(false);
        SetMainPauseViewVisible(false);

        if (helpPanel != null)
            helpPanel.SetActive(false);

        optionsPanel.SetActive(true);
    }

    public void OnOptionsBackClicked()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (helpPanel != null)
            helpPanel.SetActive(false);

        confirmPopup.SetActive(false);
        pausePanel.SetActive(true);
        SetMainPauseViewVisible(true);
    }

    public void OnHelpClicked()
    {
        if (helpPanel == null)
            return;

        pausePanel.SetActive(true);
        confirmPopup.SetActive(false);
        SetMainPauseViewVisible(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        helpPanel.SetActive(true);
    }

    public void OnHelpBackClicked()
    {
        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        confirmPopup.SetActive(false);
        pausePanel.SetActive(true);
        SetMainPauseViewVisible(true);
    }

    public void OnLeaveGameClicked()
    {
        // Show confirmation popup
        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        SetMainPauseViewVisible(true);
        pausePanel.SetActive(false);
        confirmPopup.SetActive(true);

        if (confirmText != null)
            confirmText.text = "Progress will be lost. Are you sure?";
    }

    public void OnConfirmYes()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnConfirmNo()
    {
        confirmPopup.SetActive(false);
        pausePanel.SetActive(true);
        SetMainPauseViewVisible(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }


    public bool IsPaused => isPaused;
}
