using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.8f;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName     = "Scene1";

    private AudioSource musicSource;
    private AudioSource sfxSource;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource             = gameObject.AddComponent<AudioSource>();
        musicSource.loop        = true;
        musicSource.playOnAwake = false;
        musicSource.volume      = musicVolume;

        sfxSource             = gameObject.AddComponent<AudioSource>();
        sfxSource.loop        = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume      = sfxVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = null;

        if (sceneName == mainMenuSceneName)
            clip = mainMenuMusic;
        else if (sceneName == gameSceneName)
            clip = gameplayMusic;

        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }


    public void PlayHover()
    {
        if (hoverSound == null) return;
        sfxSource.PlayOneShot(hoverSound, sfxVolume);
    }

    public void PlayClick()
    {
        if (clickSound == null) return;
        sfxSource.PlayOneShot(clickSound, sfxVolume);
    }

    // ── Volume control (for Options screen later)

    public void SetMusicVolume(float volume)
    {
        musicVolume            = Mathf.Clamp01(volume);
        musicSource.volume     = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume          = Mathf.Clamp01(volume);
        sfxSource.volume   = sfxVolume;
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume()   => sfxVolume;
}