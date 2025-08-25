using UnityEngine;

/// <summary>
/// Scriptable object storing default values for game settings.
/// </summary>
[CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "Configs/GameSettingsConfig")]
public class GameSettingsConfig : ScriptableObject
{
    [Header("Default values")]
    [SerializeField] private int activePlayer = 0;
    [SerializeField] private int animationSpeed = 1;
    [SerializeField] private int musicVolume = 5;
    [SerializeField] private int effectVolume = 5;
    [SerializeField] private int menuVolume = 5;
    [SerializeField] private int layoutStyle = 0;    

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
        GameSettingsEnum.LayoutStyle => layoutStyle,       
        _ => 0
    };
}

