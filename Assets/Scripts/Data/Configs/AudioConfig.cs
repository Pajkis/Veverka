using UnityEngine;

/// <summary>
/// Aaudio configuration settings for the game.
/// </summary>
[CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/AudioConfig")]
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
}