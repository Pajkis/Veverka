using UnityEngine;

/// <summary>
/// Aaudio configuration settings for the game.
/// </summary>
[CreateAssetMenu(fileName = "AudioConfig", menuName = "Systems/Config/Audio")]
public class AudioConfig : ScriptableObject
{
    // volume adjustmenst are used to adjust the volume of different audio channels
    [Header("Volume Adjustmenst")]
    public float soundEffectVolumeAdj = 25f;
    public float soundMusicVolumeAdj = 50f;
    public float soundUIVolumeAdj = 25f;

    [Header("Environment Sounds")]
    public float minAmbientSoundDelay = 7f; // minimum delay between ambient sounds
    public float maxAmbientSoundDelay = 15f; // maximum delay between ambient sounds

    [Header("Effect Sounds")]
    public float minPitch = 0.8f; // minimum pitch for sound effects
    public float normalPitch = 1f; // normal pitch for sound effects
    public float maxPitch = 1.2f; // maximum pitch for sound effects

    [Header("Audio Transitions")]
    [Tooltip("Fade out time when stopping current music track")]
    public float musicFadeOutTime = 0.5f;
    [Tooltip("Fade in time when starting new music track")]
    public float musicFadeInTime = 0.5f;
    [Tooltip("Cross-fade time between music tracks (fade out + fade in)")]
    public float musicCrossFadeTime = 1.0f;

    [Header("Audio Timing")]
    [Tooltip("Delay before music starts after scene load")]
    public float musicStartDelay = 0.2f;
    [Tooltip("Interval for checking if music track ended")]
    public float musicUpdateInterval = 0.1f;
}