// Assets/Scripts/Configs/DisplayConfigSO.cs
using UnityEngine;

// Application.platform device types
// https://docs.unity3d.com/ScriptReference/RuntimePlatform.html
public enum DevicePlatformType
{
    Auto,      
    Windows,
    MacOS,
    Linux,
    Android,
    iOS,
    WebGL,
    Console
}

[System.Serializable]
public class DisplayConfigPars
{
    [Header("Platform mapping")]
    public DevicePlatformType platform = DevicePlatformType.Auto;

    [Header("UI scaling")]
    public Vector2Int referenceResolution = new Vector2Int(1920, 1080);
    [Range(0f, 1f)] public float uiMatchWidthOrHeight = 1f; // 0=width, 1 = height, 0.5 = both equally

    [Header("Aspect & Bars")]
    public bool useLetterboxing = true;
    public Vector2 targetAspect = new Vector2(16, 9); // 16:9 aspect ratio

    [Header("Orientation")]
    public bool forceOrientation = true;
    public ScreenOrientation orientation = ScreenOrientation.LandscapeLeft;

    [Header("Performance")]
    [Tooltip("0 = do not touch Application.targetFrameRate")]
    public int targetFrameRate = 60;
    [Tooltip("0..4; 0 = no VSync (controls targetFrameRate)")]
    public int vSyncCount = 0;

    [Header("Safe Area")]
    public bool applySafeArea = true;

    [Header("Auto-Apply")]
    [Tooltip("re-apply after new screen is loaded")]
    public bool autoApplyOnSceneLoaded = true;
}

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
