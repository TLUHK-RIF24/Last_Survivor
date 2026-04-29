using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Character Selection")]
    [SerializeField] private Image   characterImage;     
    [SerializeField] private Image   characterNameImage;  
    [SerializeField] private Button  arrowLeft;
    [SerializeField] private Button  arrowRight;

    [Header("Character Data")]
    [SerializeField] private Sprite[] characterSprites;   
    [SerializeField] private Sprite[] characterNameImages; 

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button quitButton;

    [Header("Scene To Load")]
    [SerializeField] private string gameSceneName = "Scene1";

    private int characterIndex = 0;
    private int characterCount = 3; 


    void Start()
    {
        UpdateCharacterDisplay();
    }


    public void OnArrowLeft()
    {
        characterIndex--;
        if (characterIndex < 0) characterIndex = characterCount - 1;
        UpdateCharacterDisplay();
    }

    public void OnArrowRight()
    {
        characterIndex++;
        if (characterIndex >= characterCount) characterIndex = 0;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        // Update character sprite (Siis kui sprited olemas)
        if (characterImage != null && characterSprites != null
            && characterIndex < characterSprites.Length
            && characterSprites[characterIndex] != null)
        {
            characterImage.sprite  = characterSprites[characterIndex];
            characterImage.enabled = true;
        }
        else if (characterImage != null)
        {
            characterImage.enabled = false; 
        }

        // Update name + shadow image
        if (characterNameImage != null && characterNameImages != null
            && characterIndex < characterNameImages.Length)
        {
            characterNameImage.sprite = characterNameImages[characterIndex];
        }
    }


    public void OnPlayClicked()
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptionsClicked()
    {
        Debug.Log("[MainMenu] Options — not implemented yet");
    }

    public void OnHelpClicked()
    {
        Debug.Log("[MainMenu] Help — not implemented yet");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}