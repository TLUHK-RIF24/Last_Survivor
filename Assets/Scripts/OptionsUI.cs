using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    [Header("Volume - Music")]
    [SerializeField] private Slider   musicSlider;

    [Header("Volume - SFX")]
    [SerializeField] private Slider   sfxSlider;

    [Header("Fullscreen Toggle")]
    [SerializeField] private TMP_Text fullscreenValueText;

    [Header("Language")]
    [SerializeField] private TMP_Text languageText;
    private string[] languages     = { "English", "Estonian" };
    private int      languageIndex = 0;

    private const string PREF_MUSIC      = "MusicVolume";
    private const string PREF_SFX        = "SFXVolume";
    private const string PREF_FULLSCREEN = "Fullscreen";
    private const string PREF_LANGUAGE   = "Language";

    void OnEnable()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        float music   = PlayerPrefs.GetFloat(PREF_MUSIC, 0.5f);
        float sfx     = PlayerPrefs.GetFloat(PREF_SFX,   0.8f);
        bool  full    = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;
        languageIndex = PlayerPrefs.GetInt(PREF_LANGUAGE, 0);

        if (musicSlider != null) musicSlider.value = music;
        if (sfxSlider   != null) sfxSlider.value   = sfx;

        UpdateFullscreenDisplay(full);
        UpdateLanguageDisplay();
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        PlayerPrefs.SetFloat(PREF_MUSIC, value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        PlayerPrefs.SetFloat(PREF_SFX, value);
    }

    public void OnFullscreenLeftArrow()  => CycleFullscreen();
    public void OnFullscreenRightArrow() => CycleFullscreen();

    private void CycleFullscreen()
    {
        bool newValue     = !Screen.fullScreen;
        Screen.fullScreen = newValue;
        PlayerPrefs.SetInt(PREF_FULLSCREEN, newValue ? 1 : 0);
        UpdateFullscreenDisplay(newValue);
    }

    private void UpdateFullscreenDisplay(bool isFullscreen)
    {
        if (fullscreenValueText != null)
            fullscreenValueText.text = isFullscreen ? "ON" : "OFF";
    }

    public void OnLanguageLeftArrow()
    {
        languageIndex--;
        if (languageIndex < 0) languageIndex = languages.Length - 1;
        UpdateLanguageDisplay();
        PlayerPrefs.SetInt(PREF_LANGUAGE, languageIndex);
    }

    public void OnLanguageRightArrow()
    {
        languageIndex++;
        if (languageIndex >= languages.Length) languageIndex = 0;
        UpdateLanguageDisplay();
        PlayerPrefs.SetInt(PREF_LANGUAGE, languageIndex);
    }

    private void UpdateLanguageDisplay()
    {
        if (languageText != null)
            languageText.text = languages[languageIndex];
    }

    public void OnBackClicked()
    {
        PlayerPrefs.Save();
        MainMenuUI menu = FindFirstObjectByType<MainMenuUI>();
        if (menu != null) menu.ShowMain();
    }
}