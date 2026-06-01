using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    public AudioClip sfxButtonClick;
    public AudioClip sfxCorrect;
    public AudioClip sfxWrong;
    public AudioClip sfxLevelComplete;
    public AudioClip sfxUnlock;
    public AudioClip sfxPopupOpen;
    public AudioClip sfxPopupClose;

    [Header("BGM")]
    public AudioClip bgmMenu;

    [Header("Settings")]
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float bgmVolume = 0.4f;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        ApplySoundSetting();
    }

    void Start()
    {
        if (bgmMenu != null)
            PlayBGM(bgmMenu);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButtonClick() => PlaySFX(sfxButtonClick);
    public void PlayCorrect()      => PlaySFX(sfxCorrect);
    public void PlayWrong()        => PlaySFX(sfxWrong);
    public void PlayLevelComplete()=> PlaySFX(sfxLevelComplete);
    public void PlayUnlock()       => PlaySFX(sfxUnlock);
    public void PlayPopupOpen()    => PlaySFX(sfxPopupOpen);
    public void PlayPopupClose()   => PlaySFX(sfxPopupClose);

    public void SetSoundEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("SoundEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplySoundSetting();
    }

    public bool IsSoundEnabled() => PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

    void ApplySoundSetting()
    {
        bool on = IsSoundEnabled();
        sfxSource.mute = !on;
        bgmSource.mute = !on;
    }
}
