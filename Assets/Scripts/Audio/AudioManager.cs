using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Singleton 
    /// </summary>
    public static AudioManager Instance { get; private set; }

    #region sound input
    [Header("Effects")]
    [SerializeField] private AudioClip veverkaMove;
    [SerializeField] private AudioClip nutMove;
    [SerializeField] private AudioClip goalReached;
    [SerializeField] private AudioClip levelFinished;
    [SerializeField] private AudioClip veverkaRotate;

    [Header("UI")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip sliderMove;

    [Header("Music")]  
    [SerializeField] private List<AudioClip> menuSongs;
    [SerializeField] private List<AudioClip> gameSongs;
   
    #endregion

    #region fields

    [Header("Audio Sources")]
    private AudioSource effectsSource;
    private AudioSource backgroundMusicSource;
    private AudioSource uiSource;

    [Header("Audio Volume")]
    private float volumeEffects = 0;
    private float volumeMusic = 0;
    private float volumeMenu = 0;

    // audio sources and channels
    private Dictionary<SoundChannel, AudioSource> sources = new(); // Sfx and menu sounds
    private Dictionary<MusicEnum, List<AudioClip>> musicPlaylists; // background music soundds
    private MusicEnum currentMusicType;
    private Dictionary<Enum, AudioClip> clips = new();
    #endregion

    #region Init methods
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
    }

    /// <summary>
    /// Update rutine
    /// </summary>
     void Update()
    {
        // play next song after it ends
        if (!backgroundMusicSource.isPlaying)
        {
            PlayRandomMusic(currentMusicType);
        }
    }

    /// <summary>
    /// Init volume of sounds, play background music
    /// </summary>
    public void Initialize()
    {
        // Add separate audio sources
        effectsSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        uiSource = gameObject.AddComponent<AudioSource>();

        // map audio sources
        sources[SoundChannel.SoundEffect] = effectsSource;
        sources[SoundChannel.SoundMusic] = backgroundMusicSource;
        sources[SoundChannel.SoundUI] = uiSource;

        // Map clips
        clips[SfxEnum.VeverkaMove] = veverkaMove;
        clips[SfxEnum.NutMove] = nutMove;
        clips[SfxEnum.GoalReached] = goalReached;
        clips[SfxEnum.LevelFinished] = levelFinished;
        clips[SfxEnum.VeverkaRotate] = veverkaRotate;

        clips[UiEnum.ButtonClick] = buttonClick;
        clips[UiEnum.ButtonHover] = buttonHover;
        clips[UiEnum.Slider] = sliderMove;

        //Map music
        musicPlaylists = new Dictionary<MusicEnum, List<AudioClip>>
        {
          { MusicEnum.Menu, menuSongs },
          { MusicEnum.Game, gameSongs }
        };

        // load volume
        volumeEffects = GameSettings.Instance.Get(GameSettingsEnum.EffectVolume) / 10f;
        volumeMusic = GameSettings.Instance.Get(GameSettingsEnum.MusicVolume) / 10f;
        volumeMenu = GameSettings.Instance.Get(GameSettingsEnum.MenuVolume) / 10f;

        sources[SoundChannel.SoundEffect].volume = volumeEffects;
        sources[SoundChannel.SoundMusic].volume = volumeMusic;
        sources[SoundChannel.SoundUI].volume = volumeMenu;

        // other init settings
        // sources[SoundChannel.SoundMusic].loop = true;

        // play background sound
        currentMusicType = MusicEnum.Menu;
        PlayRandomMusic(MusicEnum.Menu);

    }

    #endregion

    #region methods
    /// <summary>
    /// play sound 
    /// </summary>
    /// <typeparam name="T"> enumeration of sound clip types (Music, Sfx, UI)</typeparam>
    /// <param name="channel"> sound channel (Music, Sfx, UI) </param>
    /// <param name="clipKey">clip name key in T enum </param>
    public void PlaySound<T>(SoundChannel channel, T clipKey) where T : Enum
    {
        if (clips.TryGetValue(clipKey, out var clip) && sources.TryGetValue(channel, out var source))
        {
            source.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Missing clip or source for {clipKey}");
        }
    }

    /// <summary>
    /// play random background music
    /// </summary>
    /// <param name="musicType"></param>
    public void PlayRandomMusic(MusicEnum musicType)
    {
        if (!musicPlaylists.TryGetValue(musicType, out List<AudioClip> playlist) || playlist == null || playlist.Count == 0)
            return;

        if (backgroundMusicSource == null)
            return;

        AudioClip oldClip = backgroundMusicSource.clip;
        AudioClip newclip = null;
        
        // only one song in playlist
        if (playlist.Count == 1)
        { 
            newclip = playlist[0];
        }
        // play random song
        else
        {
            int attempts = 0;
            do 
            { 
             newclip = playlist[UnityEngine.Random.Range(0, playlist.Count)];
             attempts++;
            }
            while (attempts <= 10 && newclip == oldClip);            
        }
        
        backgroundMusicSource.clip = newclip;
        currentMusicType = musicType;
        backgroundMusicSource.Play();
    }


    /// <summary>
    /// Play Ui click sound event handle
    /// </summary>
    public void PlayUIClickSound()
    {
        PlaySound(SoundChannel.SoundUI, UiEnum.ButtonClick);
    }
    /// <summary>
    /// Update volume value from game settings
    /// </summary>
    /// <param name="source">music source to be updated</param>
    /// <param name="value">new volume value</param>
    public void UpdateVolume(GameSettingsEnum source, int value)
    {
        // map gamesettings to Soundchannel
        SoundChannel soundChannel = source switch
        {
            GameSettingsEnum.EffectVolume => SoundChannel.SoundEffect,
            GameSettingsEnum.MusicVolume => SoundChannel.SoundMusic,
            GameSettingsEnum.MenuVolume => SoundChannel.SoundUI,
            _ => SoundChannel.NoSound
        };
                
        if (soundChannel != SoundChannel.NoSound)
        {
            //Change volume of channel
            if (sources.TryGetValue(soundChannel, out AudioSource audioSource))
            {
                audioSource.volume = value / 10f;
            }
            else 
            {
                Debug.LogWarning($"AudioSource for {soundChannel} not found!");
            }            
        }
    }

    #endregion
}
