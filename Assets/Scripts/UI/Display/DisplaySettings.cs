// Assets/Scripts/Display/DisplaySettings.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DisplayConfigSO displayConfigSO;

    // aktivní rozřešený profil
    private DisplayConfig _active;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    void OnDestroy()
    {
        if (_active != null && _active.autoApplyOnSceneLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// Volej z GameInit.Awake() (jak už děláš) nebo se spolehni na Awake tady.
    /// </summary>
    public void Init()
    {
        ResolveActiveProfile();

        ApplyPerformance();
        ApplyOrientation();

        // Aplikuj ihned pro aktuální scénu
        ApplyDisplayToCurrentScene();

        // Re-apply po každém načtení scény
        if (_active.autoApplyOnSceneLoaded)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void ResolveActiveProfile()
    {
        if (displayConfigSO == null)
        {
            Debug.LogWarning("[DisplaySettings] Missing DisplayConfigSO — using defaults.");
            // nouzová konfigurace
            _active = new DisplayConfig
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

        _active = displayConfigSO.ResolveProfile();
        if (_active == null)
        {
            Debug.LogWarning("[DisplaySettings] DisplayConfigSO has no profiles — using defaults.");
            _active = new DisplayConfig
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

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        ApplyDisplayToCurrentScene();
    }

    /// <summary>
    /// Hlavní “aplikace” po loadu scény: Kamera, UI, SafeArea.
    /// </summary>
    public void ApplyDisplayToCurrentScene()
    {
        ApplyCameraBars();
        ApplyCanvasScalers();
 
    }

    #region Performance & Orientation

    private void ApplyPerformance()
    {
        // VSync a targetFrameRate se navzájem ovlivňují
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

        // Povolit rozumné auto-rotate podle zvolené orientace
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
            // Pillarbox (pruhy vlevo/vpravo)
            cam.rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
        }
        else
        {
            // Letterbox (pruhy nahoře/dole)
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
        }
    }

    #endregion

    #region UI

    private void ApplyCanvasScalers()
    {
        var scalers = FindObjectsOfType<CanvasScaler>(includeInactive: true);
        foreach (var s in scalers)
        {
            // sjednoť režim
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = _active.referenceResolution;
            s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            s.matchWidthOrHeight = _active.uiMatchWidthOrHeight;

            // Poznámka: u Overlay canvasu kamera pruhy neovlivní – je to správně.
            // Pokud chceš, aby UI obsah držel 16:9 box, použij marker AspectBox (viz níže).
        }

        // Volitelně sjednotit Pixel Perfect apod. (dle potřeby)
    }

    #endregion
}
