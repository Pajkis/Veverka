using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Singleton 
    /// </summary>
    public static AudioManager Instance { get; private set; }

    #region sound input
    //UI sounds definition
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip slider;

    // Music sounds definition
    [SerializeField] private AudioClip menuBackground;
    [SerializeField] private AudioClip gameBackground;

    // Sound effect definition
    [SerializeField] private AudioClip veverkaMove;
    [SerializeField] private AudioClip veverkaRotate;
    [SerializeField] private AudioClip nutMove;
    [SerializeField] private AudioClip goalReached;
    [SerializeField] private AudioClip levelFinished;
    #endregion

    #region fields
    private int volumeEffects = 0;
    private int volumeMusic = 0;
    private int volumeUI = 0;

    private AudioSource effectsSource;
    private AudioSource musicSource;
    private AudioSource uiSource;
    #endregion

    /// <summary>
    /// Awake is called before start
    /// </summary>
    void Awake()
    {
        // Init singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        effectsSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        uiSource = gameObject.AddComponent<AudioSource>();

    }

    /// <summary>
    /// Init volume of sounds, play background music
    /// </summary>
    public void Initialize()
    {
        volumeEffects = GameSettings.Instance.Get(GameSettingsEnum.EffectVolume);
        volumeMusic = GameSettings.Instance.Get(GameSettingsEnum.MusicVolume);
        volumeUI = GameSettings.Instance.Get(GameSettingsEnum.EffectVolume);

        effectsSource.volume = volumeEffects;
        musicSource.volume = volumeMusic;
        uiSource.volume = volumeUI;

        musicSource.loop = true;
    }

    /// <summary>
    /// Play effect 
    /// </summary>
    /// <param name="soundEffect"></param>
    public void PlayEffect(SoundEffectEnum soundEffect)
    {
        AudioClip clip = soundEffect switch
        {
            SoundEffectEnum.VeverkaMove => veverkaMove,
            SoundEffectEnum.NutMove => nutMove,
            SoundEffectEnum.GoalReached => goalReached,
            SoundEffectEnum.LevelFinished => levelFinished,
            SoundEffectEnum.VeverkaRotate => veverkaRotate,
            _ => null
        };

        if (clip != null)
        {
            effectsSource.PlayOneShot(clip, volumeEffects);
        }
    }

}
