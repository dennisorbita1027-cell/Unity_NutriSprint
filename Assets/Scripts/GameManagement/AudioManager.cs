using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmGameplay;
    public AudioClip bgmMainMenu;

    [Header("SFX Clips")]
    public AudioClip collectGood;
    public AudioClip collectJunk;
    public AudioClip gameOver;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    private float masterVolume = 1f;
    private bool sfxEnabled = true;
    private bool bgmEnabled = true;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string SFX_ENABLED_KEY = "SFXEnabled";
    private const string BGM_ENABLED_KEY = "BGMEnabled";

    void Awake()
    {
        if (InitializeSingleton())
        {
            SetupAudioComponents();
            LoadAudioSettings();
            ApplyVolume();
        }
    }

    private void Start()
    {
        DetermineInitialBGM();
    }

    private bool InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return true;
        }

        Destroy(gameObject);
        return false;
    }

    private void SetupAudioComponents()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        bgmSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
    }

    private void DetermineInitialBGM()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // Default to gameplay, switch to menu if on index 0
        bgmSource.clip = (currentScene.buildIndex == 0) ? bgmMainMenu : bgmGameplay;

        if (bgmEnabled && bgmSource.clip != null)
        {
            bgmSource.Play();
        }
    }

    //Track Switching
    public void PlayMainMenuBGM() => SwitchBGMTrack(bgmMainMenu);
    public void PlayGameplayBGM() => SwitchBGMTrack(bgmGameplay);

    private void SwitchBGMTrack(AudioClip targetTrack)
    {
        if (!bgmEnabled || targetTrack == null) return;

        if (bgmSource.clip != targetTrack)
        {
            bgmSource.clip = targetTrack;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    //PUBLIC SFX
    public void PlayGoodCollect() => PlaySFX(collectGood);
    public void PlayJunkCollect() => PlaySFX(collectJunk);
    public void PlayGameOver() => PlaySFX(gameOver);

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    //PUBLIC BGM CONTROL
    public void PlayBGM()
    {
        if (bgmEnabled && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    //SYSTEM & SETTINGS MANAGEMENT
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolume();
        SaveAudioSettings();
    }

    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        SaveAudioSettings();
    }

    public void SetBGMEnabled(bool enabled)
    {
        bgmEnabled = enabled;

        if (!bgmEnabled) StopBGM();
        else PlayBGM();

        SaveAudioSettings();
    }

    private void ApplyVolume()
    {
        bgmSource.volume = masterVolume;
        sfxSource.volume = masterVolume;
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, sfxEnabled ? 1 : 0);
        PlayerPrefs.SetInt(BGM_ENABLED_KEY, bgmEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
        bgmEnabled = PlayerPrefs.GetInt(BGM_ENABLED_KEY, 1) == 1;
    }
}