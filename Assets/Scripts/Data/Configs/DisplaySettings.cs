using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loads and applies display configuration.
/// </summary>
public static class DisplaySettings
{
    /// <summary>
    /// Loaded configuration instance.
    /// </summary>
    public static DisplayConfigSO Config { get; private set; }

    /// <summary>
    /// Loads configuration from Resources and applies screen settings.
    /// </summary>
    public static void Init()
    {
        if (Config != null) return;

        Config = Resources.Load<DisplayConfigSO>("Configs/DisplayConfig");
        if (Config == null)
        {
            Debug.LogWarning("[DisplaySettings] DisplayConfig not found at Resources/Configs/DisplayConfig.");
            return;
        }

        Screen.SetResolution(Config.referenceResolution.x, Config.referenceResolution.y, !Config.isMobile);

        var scaler = Object.FindObjectOfType<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = Config.canvasScaleFactor;
        }
    }
}
