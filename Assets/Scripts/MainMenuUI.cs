using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MainMenuUI : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject helpPanel;

    [Header("Character Selection")]
    [SerializeField] private Image             characterImage;
    [SerializeField] private Image             characterNameImage;
    [SerializeField] private CharacterAnimator characterAnimator;
    [SerializeField] private Button            arrowLeft;
    [SerializeField] private Button            arrowRight;

    [Header("Archer")]
    [SerializeField] private Sprite archerAnimationFrame1;
    [SerializeField] private Sprite archerAnimationFrame2;
    [SerializeField] private Sprite archerNameImage;

    [Header("Mage")]
    [SerializeField] private Sprite mageAnimationFrame1;
    [SerializeField] private Sprite mageAnimationFrame2;
    [SerializeField] private Sprite mageNameImage;

    [Header("Knight")]
    [SerializeField] private Sprite knightNameImage;
    [SerializeField] private Sprite knightTbaImage;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button reportButton;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Scene1";

    private const string REPORT_BUG_URL = "https://github.com/TLUHK-RIF24/Last_Survivor";


    private int characterIndex = 0;
    private CharacterData[] characterData;

    private sealed class CharacterData
    {
        public Sprite[] animationFrames;
        public Sprite   nameImage;
        public Sprite   tbaImage;
    }

    private void Awake()
    {
        BuildCharacterData();

        WireButton(ref arrowLeft, "ArrowLeft", OnArrowLeft);
        WireButton(ref arrowRight, "ArrowRight", OnArrowRight);
        WireButton(ref playButton, "PlayButton", OnPlayClicked);
        WireButton(ref optionsButton, "OptionsButton", OnOptionsClicked);
        WireButton(ref helpButton, "HelpButton", OnHelpClicked);
        WireButton(ref quitButton, "QuitButton", OnQuitClicked);
        WireButton(ref reportButton, "ReportButton", OnReportBugClicked);
    }

    void Start()
    {
        ShowMain();
        UpdateCharacterDisplay();
    }


    public void ShowMain()
    {
        mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (helpPanel    != null) helpPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (helpPanel    != null) helpPanel.SetActive(false);
    }

    public void ShowHelp()
    {
        mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (helpPanel    != null) helpPanel.SetActive(true);
    }


    public void OnArrowLeft()
    {
        if (characterData == null || characterData.Length == 0) return;

        characterIndex--;
        if (characterIndex < 0) characterIndex = characterData.Length - 1;
        UpdateCharacterDisplay();
    }

    public void OnArrowRight()
    {
        if (characterData == null || characterData.Length == 0) return;

        characterIndex++;
        if (characterIndex >= characterData.Length) characterIndex = 0;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        if (characterData == null || characterData.Length == 0) return;

        CharacterData data = characterData[characterIndex];

        if (characterNameImage != null && data.nameImage != null)
            characterNameImage.sprite = data.nameImage;

        if (characterAnimator != null)
        {
            if (data.animationFrames != null && data.animationFrames.Length > 0)
            {
                characterImage.enabled = true;
                characterAnimator.SetFrames(data.animationFrames);
            }
            else if (data.tbaImage != null)
            {
                characterAnimator.Stop();
                characterImage.enabled = true;
                characterImage.sprite  = data.tbaImage;
            }
            else
            {
                characterImage.enabled = false;
            }
        }
    }

    private void BuildCharacterData()
    {
        characterData = new[]
        {
            new CharacterData
            {
                animationFrames = BuildFrames(archerAnimationFrame1, archerAnimationFrame2),
                nameImage = archerNameImage
            },
            new CharacterData
            {
                animationFrames = BuildFrames(mageAnimationFrame1, mageAnimationFrame2),
                nameImage = mageNameImage
            },
            new CharacterData
            {
                animationFrames = new Sprite[0],
                nameImage = knightNameImage,
                tbaImage = knightTbaImage
            }
        };
    }

    private Sprite[] BuildFrames(Sprite frame1, Sprite frame2)
    {
        if (frame1 != null && frame2 != null)
            return new[] { frame1, frame2 };

        if (frame1 != null)
            return new[] { frame1 };

        if (frame2 != null)
            return new[] { frame2 };

        return new Sprite[0];
    }


    public void OnPlayClicked()
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptionsClicked() => ShowOptions();
    public void OnHelpClicked()    => ShowHelp();

    public void OnReportBugClicked()
    {
        Application.OpenURL(REPORT_BUG_URL);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
