using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioConfig audioConfig;

    #region events
    [Header("Events")]
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private SettingEvents settingEvents;
    #endregion

    #region sound input
    [Header("Effects")]
    [SerializeField] private AudioClip veverkaMove;
    [SerializeField] private AudioClip nutMove;
    [SerializeField] private AudioClip goalReached;
    [SerializeField] private AudioClip levelFinished;
    [SerializeField] private AudioClip veverkaRotate;
    [SerializeField] private AudioClip Undo;
    [SerializeField] private AudioClip WaterSplash;
    [SerializeField] private AudioClip WaterFill;
    [SerializeField] private AudioClip NutInWater;
    [SerializeField] private AudioClip NutInHole;
    [SerializeField] private AudioClip Oops;
    [SerializeField] private AudioClip Build;

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

    // audio sources and channels
    private Dictionary<SoundChannel, AudioSource> sources = new(); // Sfx and menu sounds
    private Dictionary<MusicType, List<AudioClip>> musicPlaylists; // background music soundds
    private MusicType currentMusicType;
    private Dictionary<Enum, AudioClip> clips = new();

    float effectVolume;
    float musicVolume;
    float uiVolume;
    float pitchAdjust;

    // Audio timing
    private float musicUpdateTimer = 0f;
    #endregion

    #region Init methods
    /// <summary>
    /// Persist audio manager across scenes.
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(gameObject);        
    }

    /// <summary>
    /// Subscribe to audio events.
    /// </summary>
    private void OnEnable()
    {
        audioEvents?.AddListener(OnAudioEvent);
        settingEvents?.AddListener(OnSettingsEvent);
    }

    /// <summary>
    /// Unsubscribe from audio events.
    /// </summary>
    private void OnDisable()
    {
        audioEvents?.RemoveListener(OnAudioEvent);
        settingEvents?.RemoveListener(OnSettingsEvent);
    }

    /// <summary>
    /// Checks if the current background track finished and starts a new one.
    /// Uses configurable update interval to reduce CPU usage.
    /// </summary>
    void Update()
    {
        // Update music check timer
        musicUpdateTimer += Time.deltaTime;

        // Check music status only at configured intervals
        if (musicUpdateTimer >= audioConfig.musicUpdateInterval)
        {
            musicUpdateTimer = 0f;

            // play next song after it ends
            if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
            {
                PlayRandomMusic(currentMusicType);
            }
        }
    }

    /// <summary>
    /// Handles all audio related events and routes them to appropriate methods.
    /// </summary>
    private void OnAudioEvent(AudioEventPayload payload)
    {
        switch (payload.EventType)
        {
            case AudioEventType.Init:
                Initialize();
                break;
            case AudioEventType.PlayMusic:
                PlayRandomMusic(payload.Music);
                break;
            case AudioEventType.PlaySfx:
                PlaySfx(payload.Sfx);
                break;
            case AudioEventType.PlayUi:
                PlayUi(payload.Ui);
                break;
        }
    }

    /// <summary>
    /// Init volume of sounds, play background music
    /// </summary>
    private void Initialize()
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
        clips[SfxType.VeverkaMove] = veverkaMove;
        clips[SfxType.NutMove] = nutMove;
        clips[SfxType.GoalReached] = goalReached;
        clips[SfxType.LevelFinished] = levelFinished;
        clips[SfxType.VeverkaRotate] = veverkaRotate;
        clips[SfxType.Undo] = Undo;
        clips[SfxType.WaterSplash] = WaterSplash;
        clips[SfxType.WaterFill] = WaterFill;
        clips[SfxType.NutInWater] = NutInWater;
        clips[SfxType.NutInHole] = NutInHole;
        clips[SfxType.Oops] = Oops;
        clips[SfxType.Build] = Build;

        clips[UiType.ButtonClick] = buttonClick;
        clips[UiType.ButtonHover] = buttonHover;
        clips[UiType.Slider] = sliderMove;

        //Map music
        musicPlaylists = new Dictionary<MusicType, List<AudioClip>>
        {
          { MusicType.Menu, menuSongs },
          { MusicType.Game, gameSongs }
        };

        // load volume from game settings
        if (audioConfig == null)
        {
            DebugLogger.LogError(DebugLogCategory.General, "AudioManager: AudioConfig is not assigned!");
            return;
        }
      
        // Request volume values at init
        settingEvents.Raise(new SettingEventPayload { EventType = SettingsEventType.DataRequest, Setting = GameSettingsEnum.EffectVolume });
        settingEvents.Raise(new SettingEventPayload { EventType = SettingsEventType.DataRequest, Setting = GameSettingsEnum.MusicVolume });
        settingEvents.Raise(new SettingEventPayload { EventType = SettingsEventType.DataRequest, Setting = GameSettingsEnum.MenuVolume });
        settingEvents.Raise(new SettingEventPayload { EventType = SettingsEventType.DataRequest, Setting = GameSettingsEnum.AnimationSpeed }); // to set pitch for sound effects

        // other init settings
        // sources[SoundChannel.SoundMusic].loop = true;

        // play background sound with delay
        currentMusicType = MusicType.Menu;
        StartCoroutine(PlayMusicAfterDelay(MusicType.Menu, audioConfig.musicStartDelay));

    }

    #endregion

    #region play sounds methods
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
            // check if clip and source are not null
            if (clip == null)
            {
                DebugLogger.LogWarning(DebugLogCategory.General, $"AudioManager: Clip for {clipKey} is null.");               
            }
            if (source == null)
            {
                DebugLogger.LogWarning(DebugLogCategory.General, $"AudioManager: AudioSource for {channel} is null.");
            }

            // adjust pitch for sound effects based on animation speed setting
            if (channel == SoundChannel.SoundEffect)
            {
                float pitch = (int)pitchAdjust switch
                {
                    1 => audioConfig.minPitch,
                    2 => audioConfig.normalPitch,
                    3 => audioConfig.maxPitch,
                    _ => audioConfig.normalPitch
                };                
                source.pitch = pitch;
            }
            // reset pitch for other channels
            else
            {
                source.pitch = audioConfig.normalPitch; 
            }

            source.PlayOneShot(clip);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.General, $"AudioManager: Missing clip or source for {clipKey}");
        }
    }

    /// <summary>
    /// Play sound effect clip on the effects channel.
    /// </summary>
    public void PlaySfx(SfxType clipKey) => PlaySound(SoundChannel.SoundEffect, clipKey);

    /// <summary>
    /// Play UI sound clip on the UI channel.
    /// </summary>
    public void PlayUi(UiType clipKey) => PlaySound(SoundChannel.SoundUI, clipKey);

    /// <summary>
    /// play random background music
    /// </summary>
    /// <param name="musicType"></param>
    public void PlayRandomMusic(MusicType musicType)
    {
        if (!musicPlaylists.TryGetValue(musicType, out List<AudioClip> playlist) || playlist == null || playlist.Count == 0)
            return;

        if (backgroundMusicSource == null)
            return;

        AudioClip oldClip = backgroundMusicSource.clip;
        AudioClip newClip;

        // only one song in playlist
        if (playlist.Count == 1)
        {
            newClip = playlist[0];
        }
        // play random song avoiding immediate repeats
        else
        {
            int oldIndex = playlist.IndexOf(oldClip);
            int newIndex = UnityEngine.Random.Range(0, playlist.Count - (oldIndex != -1 ? 1 : 0));
            if (oldIndex != -1 && newIndex >= oldIndex)
            {
                newIndex++;
            }
            newClip = playlist[newIndex];
        }

        // If we're switching tracks and have fade time configured, use cross-fade
        if (oldClip != null && oldClip != newClip && audioConfig.musicCrossFadeTime > 0)
        {
            StartCoroutine(CrossFadeMusic(newClip, musicType));
        }
        else
        {
            backgroundMusicSource.clip = newClip;
            currentMusicType = musicType;
            backgroundMusicSource.Play();
        }
    }

    /// <summary>
    /// Cross-fade between current and new music track
    /// </summary>
    private System.Collections.IEnumerator CrossFadeMusic(AudioClip newClip, MusicType musicType)
    {
        float originalVolume = backgroundMusicSource.volume;
        float fadeOutTime = audioConfig.musicFadeOutTime;
        float fadeInTime = audioConfig.musicFadeInTime;

        // Fade out current track
        float timer = 0f;
        while (timer < fadeOutTime && backgroundMusicSource.isPlaying)
        {
            timer += Time.deltaTime;
            backgroundMusicSource.volume = originalVolume * (1f - timer / fadeOutTime);
            yield return null;
        }

        // Switch to new track
        backgroundMusicSource.clip = newClip;
        currentMusicType = musicType;
        backgroundMusicSource.volume = 0f;
        backgroundMusicSource.Play();

        // Fade in new track
        timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            backgroundMusicSource.volume = originalVolume * (timer / fadeInTime);
            yield return null;
        }

        // Ensure final volume is set
        backgroundMusicSource.volume = originalVolume;
    }

    /// <summary>
    /// Play music after configured delay
    /// </summary>
    private System.Collections.IEnumerator PlayMusicAfterDelay(MusicType musicType, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomMusic(musicType);
    }

    /// <summary>
    /// Play Ui click sound event handle
    /// </summary>
    public void PlayUIClickSound()
    {
        PlayUi(UiType.ButtonClick);
    }

    #endregion

    #region volume methods

    /// <summary>
    /// Update volume value from game settings
    /// </summary>
    /// <param name="source">music source to be updated</param>
    /// <param name="value">new volume value</param>
    public void UpdateVolume(GameSettingsEnum source, int volumeValue)
    {
        // check if audioConfig is assigned
        SoundChannel soundChannel = source switch
        {
            GameSettingsEnum.EffectVolume => SoundChannel.SoundEffect,
            GameSettingsEnum.MusicVolume => SoundChannel.SoundMusic,
            GameSettingsEnum.MenuVolume => SoundChannel.SoundUI,
            _ => SoundChannel.NoSound
        };

        if (soundChannel == SoundChannel.NoSound)
            return;

        // check if the sound channel exists in sources
        if (!sources.TryGetValue(soundChannel, out AudioSource audioSource))
        {
            DebugLogger.LogWarning(DebugLogCategory.General, $"AudioManager: AudioSource for {soundChannel} not found!");
            return;
        }

        // calculate volume adjustment based on sound channel
        float volumeAdj = soundChannel switch
        {
            SoundChannel.SoundMusic => audioConfig.soundMusicVolumeAdj,
            SoundChannel.SoundUI => audioConfig.soundUIVolumeAdj,
            SoundChannel.SoundEffect => audioConfig.soundEffectVolumeAdj,
            _ => 0f
        };

        audioSource.volume = volumeValue / volumeAdj;
    }

    #endregion

    /// <summary>
    /// Change volume or pitch based on requested change
    /// </summary>
    /// <param name="payload"></param>
    private void OnSettingsEvent(SettingEventPayload payload)
    {
        if (payload.EventType != SettingsEventType.DataBroadcast)
            return;

        switch (payload.Setting)
        {
            case GameSettingsEnum.EffectVolume:
                effectVolume = payload.Value / audioConfig.soundEffectVolumeAdj;
                UpdateVolume(GameSettingsEnum.EffectVolume, (int)payload.Value);
                break;
            case GameSettingsEnum.MusicVolume:
                musicVolume = payload.Value / audioConfig.soundMusicVolumeAdj;
                UpdateVolume(GameSettingsEnum.MusicVolume, (int)payload.Value);
                break;
            case GameSettingsEnum.MenuVolume:
                uiVolume = payload.Value /  audioConfig.soundUIVolumeAdj;
                UpdateVolume(GameSettingsEnum.MenuVolume, (int)payload.Value);
                break;
            case GameSettingsEnum.AnimationSpeed:
                pitchAdjust = payload.Value;
                break;
        }
    }
}
