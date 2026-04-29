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
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (confirmPopup.activeSelf)
            {
                OnConfirmNo();
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
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        confirmPopup.SetActive(false);
    }

    // ── Button callbacks ─────────────────────────────────────────────────────

    public void OnContinueClicked()
    {
        Resume();
    }

    public void OnOptionsClicked()
    {
        // Options screen not implemented yet — placeholder
        Debug.Log("[PauseMenu] Options clicked — not implemented yet");
    }

    public void OnHelpClicked()
    {
        // Help screen not implemented yet — placeholder
        Debug.Log("[PauseMenu] Help clicked — not implemented yet");
    }

    public void OnLeaveGameClicked()
    {
        // Show confirmation popup
        pausePanel.SetActive(false);
        confirmPopup.SetActive(true);

        if (confirmText != null)
            confirmText.text = "Progress will be lost. Are you sure?";
    }

    public void OnConfirmYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnConfirmNo()
    {
        confirmPopup.SetActive(false);
        pausePanel.SetActive(true);
    }


    public bool IsPaused => isPaused;
}