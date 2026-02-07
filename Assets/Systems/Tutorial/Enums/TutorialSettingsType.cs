
/// <summary>
/// Specifies the available settings types for tutorial Player prefs
/// </summary>
/// <remarks>Use <see cref="TutorialSettingsType"/> to identify which aspect of the tutorial experience is being
/// configured or queried. The enumeration values correspond to distinct settings that may be adjusted
/// independently.</remarks>
public enum TutorialSettingsType
{
    Tutorial_Autoplay,
    Tutorial_TextSpeed,
    Tutorial_AnimSpeed
}

/// <summary>
/// Specifies the available animation speed options for tutorials.
/// </summary>
/// <remarks>Use this enumeration to select the desired speed for tutorial animations. The values represent
/// increasing speed levels, with <see cref="Normal"/> being the default, <see cref="Fast"/> for accelerated playback,
/// and <see cref="VeryFast"/> for the fastest animation.</remarks>
public enum TutorialAnimSpeedType
{
    Normal = 1,
    Fast = 3,
    VeryFast = 5
}