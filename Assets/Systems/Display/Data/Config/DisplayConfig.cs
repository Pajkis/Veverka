using UnityEngine;

/// <summary>
/// Represents a configuration for display settings, including platform-specific profiles.
/// </summary>
/// <remarks>This class is a ScriptableObject that holds an array of display configuration profiles. It provides
/// functionality to resolve the appropriate profile based on the current runtime platform.</remarks>
[CreateAssetMenu(menuName = "Configs/DisplayConfig")]
public class DisplayConfig : ScriptableObject
{
    public DisplayConfigPars[] profiles;

    /// <summary>
    /// return profile with a current platform.
       /// </summary>
    public DisplayConfigPars ResolveProfile()
    {
        if (profiles == null || profiles.Length == 0) return null;

        DevicePlatformType current = FromApplicationPlatform(Application.platform);

        // 1) trz exact match
        foreach (var p in profiles)
        {
            if (p.platform == current) return p;
        }

        // 2) try to find Auto match
        foreach (var p in profiles)
        {
            if (p.platform == DevicePlatformType.Auto) return p;
        }

        // 3) fallback first match
        return profiles[0];
    }

    public static DevicePlatformType FromApplicationPlatform(RuntimePlatform rp)
    {
        switch (rp)
        {
            case RuntimePlatform.Android: return DevicePlatformType.Android;
            case RuntimePlatform.IPhonePlayer: return DevicePlatformType.iOS;
            case RuntimePlatform.WebGLPlayer: return DevicePlatformType.WebGL;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor: return DevicePlatformType.Windows;
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor: return DevicePlatformType.MacOS;
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor: return DevicePlatformType.Linux;
            default: return DevicePlatformType.Auto;
        }
    }
}
