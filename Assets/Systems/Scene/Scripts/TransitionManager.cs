using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

/// <summary>
/// Manages smooth scene transitions with fade effects and music crossfading.
/// Creates a persistent transition canvas that covers the screen during scene changes.
/// Integrates with existing AudioManager for music crossfade.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the TransitionManager.
    /// </summary>
    public static TransitionManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private TransitionConfig config;

    [Header("Events")]
    [SerializeField] private AudioEvents audioEvents;

    [Header("Transition Canvas Settings")]
    [Tooltip("Sort order for transition canvas (should be highest to cover everything)")]
    [SerializeField] private int canvasSortOrder = 9999;

    // Canvas components
    private Canvas transitionCanvas;
    private CanvasGroup canvasGroup;
    private Image fadeImage;

    // Transition state
    private bool isTransitioning = false;

    #region Initialization

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DebugLogger.Log(DebugLogCategory.SceneManager, "TransitionManager initialized - persisting across scene loads", this);
        }
        else if (Instance != this)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Duplicate TransitionManager found - destroying this instance", this);
            Destroy(gameObject);
            return;
        }

        // Create transition canvas
        CreateTransitionCanvas();
    }

    /// <summary>
    /// Creates the persistent transition canvas that will be used for all scene transitions.
    /// </summary>
    private void CreateTransitionCanvas()
    {
        // Create canvas GameObject
        GameObject canvasObject = new GameObject("TransitionCanvas");
        canvasObject.transform.SetParent(transform);
        DontDestroyOnLoad(canvasObject);

        // Add and configure Canvas component
        transitionCanvas = canvasObject.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = canvasSortOrder;

        // Add CanvasScaler for resolution independence
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Add CanvasGroup for alpha control
        canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start transparent
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true; // Always block raycasts when visible (prevents input during transition)

        // Create fade panel
        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(canvasObject.transform, false);

        // Configure RectTransform to fill screen
        RectTransform rectTransform = fadePanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        // Add Image component for fade effect
        fadeImage = fadePanel.AddComponent<Image>();
        fadeImage.color = config != null ? config.fadeColor : Color.black;
        fadeImage.raycastTarget = true;

        DebugLogger.Log(DebugLogCategory.SceneManager, "Transition canvas created and configured", this);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Transitions to a new scene with fade effects and music crossfade.
    /// </summary>
    /// <param name="sceneType">The scene to transition to</param>
    /// <param name="sceneRef">Addressable reference to the scene</param>
    public void TransitionToScene(SceneType sceneType, AssetReference sceneRef)
    {
        if (isTransitioning)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Transition already in progress - ignoring request", this);
            return;
        }

        if (config == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "TransitionConfig not assigned - loading scene directly without transition", this);
            sceneRef.LoadSceneAsync();
            return;
        }

        StartCoroutine(TransitionCoroutine(sceneType, sceneRef));
    }

    #endregion

    #region Transition Coroutine

    /// <summary>
    /// Main transition coroutine: Fade Out → Music Crossfade → Load Scene → Fade In
    /// </summary>
    private IEnumerator TransitionCoroutine(SceneType sceneType, AssetReference sceneRef)
    {
        isTransitioning = true;
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Starting transition to scene: {sceneType}", this);

        // Get settings for this specific scene (may have overrides)
        SceneTransitionSettings settings = config.GetSettingsForScene(sceneType);

        // Update fade color in case it changed
        fadeImage.color = settings.fadeColor;

        // Ensure canvas starts transparent (unless we're fading out)
        if (!settings.enableFadeOut)
        {
            canvasGroup.alpha = 0f;
        }

        // 1. FADE OUT (if enabled)
        if (settings.enableFadeOut)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Fading out ({settings.fadeOutDuration}s)", this);
            yield return FadeOut(settings.fadeOutDuration, settings.fadeOutCurve);
        }

        // 2. TRIGGER MUSIC CROSSFADE
        MusicType newMusicType = GetMusicTypeForScene(sceneType);
        if (audioEvents != null)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Triggering music crossfade to: {newMusicType}", this);
            audioEvents.Raise(new AudioEventPayload
            {
                EventType = AudioEventType.PlayMusic,
                Music = newMusicType
            });
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "AudioEvents not assigned - skipping music crossfade", this);
        }

        // 3. LOAD SCENE ASYNCHRONOUSLY
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Loading scene: {sceneType} via Addressables", this);
        yield return LoadSceneAsync(sceneRef);

        // 4. FADE IN (if enabled)
        if (settings.enableFadeIn)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Fading in ({settings.fadeInDuration}s)", this);
            yield return FadeIn(settings.fadeInDuration, settings.fadeInCurve);
        }
        else
        {
            // No fade in - ensure canvas is transparent so scene is visible
            canvasGroup.alpha = 0f;
            DebugLogger.Log(DebugLogCategory.SceneManager, "Fade in disabled - hiding canvas", this);
        }

        isTransitioning = false;
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Transition to {sceneType} complete", this);
    }

    #endregion

    #region Animation Coroutines

    /// <summary>
    /// Fades the screen to the configured color (makes canvas opaque).
    /// Uses unscaled time so it works even if game is paused.
    /// </summary>
    private IEnumerator FadeOut(float duration, AnimationCurve curve)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time (works during pause)
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = curve.Evaluate(t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, curveValue);
            yield return null;
        }

        // Ensure final value
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Fades the screen from the configured color (makes canvas transparent).
    /// Uses unscaled time so it works even if game is paused.
    /// </summary>
    private IEnumerator FadeIn(float duration, AnimationCurve curve)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time (works during pause)
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = curve.Evaluate(t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, curveValue);
            yield return null;
        }

        // Ensure final value
        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Loads a scene asynchronously via Addressables.
    /// Waits for the load operation to complete.
    /// </summary>
    private IEnumerator LoadSceneAsync(AssetReference sceneRef)
    {
        AsyncOperationHandle<SceneInstance> handle = sceneRef.LoadSceneAsync();

        // Wait for scene to finish loading
        while (!handle.IsDone)
        {
            // Could expose progress here if we want a loading bar later
            // float progress = handle.PercentComplete;
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, "Scene loaded successfully", this);
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.SceneManager, $"Scene load failed: {handle.OperationException}", this);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Determines which music type should play for a given scene.
    /// </summary>
    private MusicType GetMusicTypeForScene(SceneType scene)
    {
        return scene switch
        {
            SceneType.GamePlay => MusicType.Game,
            SceneType.MainMenu => MusicType.Menu,
            SceneType.LevelSelect => MusicType.Menu,
            SceneType.LevelTransition => MusicType.Menu, // Keep menu music during loading
            SceneType.PlayerMenu => MusicType.Menu,
            SceneType.HighScoreMenu => MusicType.Menu,
            _ => MusicType.Menu // Default to menu music
        };
    }

    #endregion

    #region Debug/Testing

    /// <summary>
    /// For testing: manually trigger fade out.
    /// </summary>
    [ContextMenu("Test Fade Out")]
    private void TestFadeOut()
    {
        if (config != null)
        {
            StartCoroutine(FadeOut(config.fadeOutDuration, config.fadeOutCurve));
        }
    }

    /// <summary>
    /// For testing: manually trigger fade in.
    /// </summary>
    [ContextMenu("Test Fade In")]
    private void TestFadeIn()
    {
        if (config != null)
        {
            StartCoroutine(FadeIn(config.fadeInDuration, config.fadeInCurve));
        }
    }

    #endregion
}
