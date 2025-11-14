using UnityEngine;

/// <summary>
/// Scriptable object storing default values for game settings.
/// </summary>
[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Configs/GameSettingsConfig")]
public class GameSettingsConfig : ScriptableObject
{
    [Header("Player Settings")]
    [Tooltip("Active player index (0-based)")]
    [SerializeField] private int activePlayer = 0;

    [Header("Performance")]
    [Tooltip("Animation speed: 1=Slow, 2=Normal, 3=Fast")]
    [Range(1, 3)]
    [SerializeField] private int animationSpeed = 2; // Default to Normal

    [Header("Volume Settings (0-10)")]
    [Tooltip("Background music volume level")]
    [Range(0, 10)]
    [SerializeField] private int musicVolume = 5;
    [Tooltip("Sound effects volume level")]
    [Range(0, 10)]
    [SerializeField] private int effectVolume = 5;
    [Tooltip("UI sounds volume level")]
    [Range(0, 10)]
    [SerializeField] private int menuVolume = 5;

    [Header("UI Layout")]
    [Tooltip("Button layout style for different platforms")]
    [SerializeField] private LayoutStyleTypes layoutStyle = LayoutStyleTypes.WindowsRight;    

    /// <summary>
    /// Get default value for a given setting.
    /// </summary>
    /// <param name="key">Setting key.</param>
    /// <returns>Default value defined in config.</returns>
    public int GetDefault(GameSettingsEnum key) => key switch
    {
        GameSettingsEnum.ActivePlayer => activePlayer,
        GameSettingsEnum.AnimationSpeed => animationSpeed,
        GameSettingsEnum.MusicVolume => musicVolume,
        GameSettingsEnum.EffectVolume => effectVolume,
        GameSettingsEnum.MenuVolume => menuVolume,
        GameSettingsEnum.LayoutStyle => (int)layoutStyle, // Cast enum to int
        _ => 0
    };
}

