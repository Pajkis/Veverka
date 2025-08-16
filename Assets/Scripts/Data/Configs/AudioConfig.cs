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
}