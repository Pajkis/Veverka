using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Plays random ambient sounds (birds, wind, squirrels...) at random intervals.
/// Volume is controlled by GameSettings (same level as effects).
/// </summary>
public class EnvironmentSounds : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private List<AudioClip> ambientClips;

    [Header("Settings")]
    [SerializeField] private float minDelay;
    [SerializeField] private float maxDelay;

    [Header("Audio Config")]
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private SettingEvents settingEvents;

    private float effectVolume;
    private AudioSource audioSource;

    /// <summary>
    /// Initializes the component and configures the associated <see cref="AudioSource"/> instance.
    /// </summary>
    /// <remarks>This method is called automatically by Unity when the script instance is being loaded. It
    /// adds an <see cref="AudioSource"/> component to the current <see cref="GameObject"/> and disables its  <see
    /// cref="AudioSource.playOnAwake"/> property to prevent audio playback upon initialization.</remarks>
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Audio source check
        if (ambientClips == null || ambientClips.Count == 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.General, "No ambient clips assigned to EnvironmentSounds.", this);
        }

        // AudioConfig settings
        minDelay = audioConfig.minAmbientSoundDelay;
        maxDelay = audioConfig.maxAmbientSoundDelay;
    }
       
    private void OnEnable()
    {
        settingEvents.AddListener(OnSettingEvent);
        settingEvents.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataRequest,
            Setting = GameSettingsEnum.EffectVolume
        });
        StartCoroutine(PlayAmbientLoop());
    }

    private void OnDisable()
    {
        settingEvents.RemoveListener(OnSettingEvent);
        StopAllCoroutines();
    }


    /// <summary>
    /// Plays ambient sound effects in a loop with randomized delays between each playback.
    /// </summary>
    /// <remarks>This method continuously selects a random audio clip from the available ambient sound clips
    /// and plays it  with a randomized delay between each playback. The volume of the sound is adjusted based on the
    /// current  game settings for effect volume.</remarks>
    /// <returns></returns>
    private IEnumerator PlayAmbientLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (ambientClips.Count > 0)
            {
                AudioClip clip = ambientClips[Random.Range(0, ambientClips.Count)];

                // get volume from GameSettings (same as Effects)
                float volume = effectVolume / audioConfig.soundEffectVolumeAdj;
                audioSource.PlayOneShot(clip, volume);
            }
        }
    }

    /// <summary>
    /// Setting data event handler
    /// </summary>
    /// <param name="payload"></param>
    private void OnSettingEvent(SettingEventPayload payload)
    {
        if (payload.EventType == SettingsEventType.DataBroadcast &&
            payload.Setting == GameSettingsEnum.EffectVolume)
        {
            effectVolume = payload.Value;
        }
    }
}