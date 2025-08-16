using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// audio manager singleton instance
    /// </summary>
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioConfig audioConfig;

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
    private float volumeEffects;
    private float volumeMusic;
    private float volumeMenu;

    // audio sources and channels
    private Dictionary<SoundChannel, AudioSource> sources = new(); // Sfx and menu sounds
    private Dictionary<MusicEnum, List<AudioClip>> musicPlaylists; // background music soundds
    private MusicEnum currentMusicType;
    private Dictionary<Enum, AudioClip> clips = new();
    #endregion

    #region Init methods
    /// <summary>
    /// Creates the singleton instance and persists it across scenes.
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
    /// Checks if the current background track finished and starts a new one.
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
        if (audioConfig == null)
        {
            Debug.LogError("AudioConfig is not assigned in AudioManager!");
            return;
        }
        volumeEffects = GameSettings.Instance.Get(GameSettingsEnum.EffectVolume) / audioConfig.soundEffectVolumeAdj;
        volumeMusic = GameSettings.Instance.Get(GameSettingsEnum.MusicVolume) / audioConfig.soundMusicVolumeAdj;
        volumeMenu = GameSettings.Instance.Get(GameSettingsEnum.MenuVolume) / audioConfig.soundUIVolumeAdj;

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
                if (soundChannel == SoundChannel.SoundMusic)
                {
                    // if music channel, update volume of background music
                    audioSource.volume = value / audioConfig.soundMusicVolumeAdj; // music volume is half of the sound effect volume
                }
                else if (soundChannel == SoundChannel.SoundUI)
                {
                    // if UI channel, update volume of UI sounds
                    audioSource.volume = value / audioConfig.soundUIVolumeAdj; // UI volume is same as sound effect volume
                }
                else if (soundChannel == SoundChannel.SoundEffect)
                {
                    // if sound effect channel, update volume of sound effects
                    audioSource.volume = value / audioConfig.soundEffectVolumeAdj; // sound effect volume is same as sound effect volume
                }
            }
            else 
            {
                Debug.LogWarning($"AudioSource for {soundChannel} not found!");
            }            
        }
    }

    #endregion
}
