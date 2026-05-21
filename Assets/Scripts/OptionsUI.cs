using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [Header("Volume - Music")]
    [SerializeField] private Slider musicSlider;

    [Header("Volume - SFX")]
    [SerializeField] private Slider sfxSlider;

    [Header("Fullscreen Toggle")]
    [SerializeField] private Image  fullscreenButtonImage;
    [SerializeField] private Sprite fullscreenOnSprite;
    [SerializeField] private Sprite fullscreenOffSprite;

    private const string PREF_MUSIC      = "MusicVolume";
    private const string PREF_SFX        = "SFXVolume";
    private const string PREF_FULLSCREEN = "Fullscreen";


    void OnEnable()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        float music = PlayerPrefs.GetFloat(PREF_MUSIC, 0.5f);
        float sfx   = PlayerPrefs.GetFloat(PREF_SFX,   0.8f);
        bool  full  = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;

        if (musicSlider != null) musicSlider.value = music;
        if (sfxSlider   != null) sfxSlider.value   = sfx;

        UpdateFullscreenDisplay(full);
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


    public void OnFullscreenToggleClicked()
    {
        bool newValue     = !Screen.fullScreen;
        Screen.fullScreen = newValue;
        PlayerPrefs.SetInt(PREF_FULLSCREEN, newValue ? 1 : 0);
        UpdateFullscreenDisplay(newValue);
    }

    private void UpdateFullscreenDisplay(bool isFullscreen)
    {
        if (fullscreenButtonImage == null) return;
        fullscreenButtonImage.sprite = isFullscreen ? fullscreenOnSprite : fullscreenOffSprite;
        fullscreenButtonImage.SetNativeSize();
    }


    public void OnBackClicked()
    {
        PlayerPrefs.Save();
        MainMenuUI menu = FindFirstObjectByType<MainMenuUI>();
        if (menu != null) menu.ShowMain();
    }
}