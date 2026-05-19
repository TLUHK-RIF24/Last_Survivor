using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [System.Serializable]
    public class CharacterData
    {
        public string   name;
        public Sprite[] animationFrames;   
        public Sprite   nameImage;         
        public Sprite   tbaImage;          
    }

    [Header("Characters")]
    [SerializeField] private CharacterData[] characters;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button quitButton;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Scene1";

    
    private int characterIndex = 0;


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
        characterIndex--;
        if (characterIndex < 0) characterIndex = characters.Length - 1;
        UpdateCharacterDisplay();
    }

    public void OnArrowRight()
    {
        characterIndex++;
        if (characterIndex >= characters.Length) characterIndex = 0;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        if (characters == null || characters.Length == 0) return;

        CharacterData data = characters[characterIndex];

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


    public void OnPlayClicked()
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptionsClicked() => ShowOptions();
    public void OnHelpClicked()    => ShowHelp();

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}