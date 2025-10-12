using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Display settings manager - applies display settings from DisplayConfigSO
/// </summary>
public class DisplaySettings : MonoBehaviour
{
    [Header("ConfigSO")]
    [SerializeField] private DisplayConfig displayConfig;

    // active profile
    private DisplayConfigPars _active;

    /// <summary>
    /// initialization - don't destroy on load and init display settings
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    /// <summary>
    /// on destroy remove scene loaded event handler
    /// </summary>
    void OnDestroy()
    {
        if (_active != null && _active.autoApplyOnSceneLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// initialize display settings from config
    /// </summary>
    public void Init()
    {
        ResolveActiveProfile();
        ApplyPerformance();
        ApplyOrientation();  
        ApplyDisplayToCurrentScene();

        // Re-apply after scene load
        if (_active.autoApplyOnSceneLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    /// <summary>
    /// resolve active profile from config
    /// </summary>
    private void ResolveActiveProfile()
    {
        if (displayConfig == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Display, "DisplaySettings: Missing DisplayConfigSO — using defaults.");
            // fallback defaults
            _active = new DisplayConfigPars
            {
                referenceResolution = new Vector2Int(1920, 1080),
                uiMatchWidthOrHeight = 1f,
                useLetterboxing = true,
                targetAspect = new Vector2(16, 9),
                forceOrientation = false,
                orientation = ScreenOrientation.LandscapeLeft,
                targetFrameRate = 60,
                vSyncCount = 0,
                applySafeArea = true,
                autoApplyOnSceneLoaded = true
            };
            return;
        }

        _active = displayConfig.ResolveProfile();
        if (_active == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Display, "DisplaySettings: DisplayConfigSO has no profiles — using defaults.");
            _active = new DisplayConfigPars
            {
                referenceResolution = new Vector2Int(1920, 1080),
                uiMatchWidthOrHeight = 1f,
                useLetterboxing = true,
                targetAspect = new Vector2(16, 9),
                forceOrientation = false,
                orientation = ScreenOrientation.LandscapeLeft,
                targetFrameRate = 60,
                vSyncCount = 0,
                applySafeArea = true,
                autoApplyOnSceneLoaded = true
            };
        }
    }

    /// <summary>
    /// on scene loaded event handler
    /// </summary>
    /// <param name="s"></param>
    /// <param name="m"></param>
    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        ApplyDisplayToCurrentScene();
    }

    /// <summary>
    ///main method to apply display settings to current scene
    /// </summary>
    public void ApplyDisplayToCurrentScene()
    {
        ApplyCameraBars();
        ApplyCanvasScalers();
 
    }

    #region Performance & Orientation

    private void ApplyPerformance()
    {
        // VSync and targetFrameRate settings 
        QualitySettings.vSyncCount = Mathf.Clamp(_active.vSyncCount, 0, 4);
        if (QualitySettings.vSyncCount == 0 && _active.targetFrameRate > 0)
            Application.targetFrameRate = _active.targetFrameRate;
        else
            Application.targetFrameRate = -1; // default

        Screen.sleepTimeout = SleepTimeout.NeverSleep; // volitelné
    }

    private void ApplyOrientation()
    {
        if (!_active.forceOrientation) return;

        //rotation lock
        bool landscape = _active.orientation == ScreenOrientation.LandscapeLeft
                      || _active.orientation == ScreenOrientation.LandscapeRight;

        Screen.autorotateToLandscapeLeft = landscape;
        Screen.autorotateToLandscapeRight = landscape;
        Screen.autorotateToPortrait = !landscape;
        Screen.autorotateToPortraitUpsideDown = !landscape;

        Screen.orientation = _active.orientation;
    }

    #endregion

    #region Camera
    /// <summary>
    /// apply letterbox/pillarbox to main camera
    /// </summary>
    private void ApplyCameraBars()
    {
        if (!_active.useLetterboxing) return;

        var cam = Camera.main;
        if (!cam) return;

        float targetAspect = _active.targetAspect.x / _active.targetAspect.y;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            // letterbox right/left 
            cam.rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
        }
        else
        {
            // Letterbox up/down
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
        }
    }

    #endregion

    #region UI
    /// <summary>
    /// apply CanvasScaler settings to all CanvasScalers in the scene
    /// </summary>
    private void ApplyCanvasScalers()
    {
        var scalers = FindObjectsOfType<CanvasScaler>(includeInactive: true);
        foreach (var s in scalers)
        {
            // Unified settings for all CanvasScalers in the scene
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = _active.referenceResolution;
            s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            s.matchWidthOrHeight = _active.uiMatchWidthOrHeight;
        }

    }

    #endregion
}
