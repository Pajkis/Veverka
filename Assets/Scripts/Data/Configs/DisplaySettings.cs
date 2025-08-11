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

        if (Config.isMobile)
        {
            Screen.orientation = ScreenOrientation.Landscape;

            var cam = Camera.main;
            if (cam != null)
            {
                float targetAspect = Config.aspectRatio;
                float screenAspect = (float)Screen.width / Screen.height;
                if (screenAspect > targetAspect)
                {
                    float scale = targetAspect / screenAspect;
                    cam.rect = new Rect((1f - scale) / 2f, 0f, scale, 1f);
                }
                else if (screenAspect < targetAspect)
                {
                    float scale = screenAspect / targetAspect;
                    cam.rect = new Rect(0f, (1f - scale) / 2f, 1f, scale);
                }
                else
                {
                    cam.rect = new Rect(0f, 0f, 1f, 1f);
                }
            }
        }
        else
        {
            Screen.SetResolution(Config.referenceResolution.x, Config.referenceResolution.y, true);
        }

        var scaler = Object.FindObjectOfType<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = Config.canvasScaleFactor;
        }
    }
}
