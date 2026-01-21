using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls overlay appearance and disappearance animations.
/// Automatically animates on spawn and provides smooth close functionality.
/// Uses Time.unscaledDeltaTime to work during paused game (Time.timeScale = 0).
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class OverlayAnimationController : MonoBehaviour
{
    [Header("Overlay Info")]
    [SerializeField] private OverlayType overlayType;

    // Cached components
    private CanvasGroup canvasGroup;

    // Animation state
    private bool isAnimating = false;
    private Coroutine currentAnimation;

    #region Initialization

    private void Awake()
    {
        // Cache components
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            DebugLogger.Log(DebugLogCategory.SceneManager, "CanvasGroup component added to overlay", this);
        }
    }

    private void Start()
    {
        // Automatically play open animation when overlay spawns
        PlayOpenAnimation();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Sets the overlay type (used by SceneFlowManager when spawning).
    /// </summary>
    public void SetOverlayType(OverlayType type)
    {
        overlayType = type;
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Overlay {gameObject.name} type set to: {type}", this);
    }

    /// <summary>
    /// Fade out animation - returns control after animation completes.
    /// Use with yield return StartCoroutine() to wait for completion.
    /// </summary>
    public IEnumerator FadeOutAsync()
    {
        if (isAnimating)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Animation already in progress - ignoring fade request", this);
            yield break;
        }

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        // Start fade out coroutine and wait for it to complete
        yield return StartCoroutine(FadeOutCoroutine());
    }

    /// <summary>
    /// Helper method: Fade out + destroy in one call.
    /// Starts coroutine internally, does NOT wait.
    /// </summary>
    public void FadeOutAndDestroy()
    {
        StartCoroutine(FadeOutAndDestroyCoroutine());
    }

    /// <summary>
    /// Immediately destroys the overlay GameObject without animation.
    /// </summary>
    public void DestroyImmediately()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Destroying {overlayType} overlay immediately", this);
        Destroy(gameObject);
    }

    #endregion

    #region Unity Editor OnClick Methods

    /// <summary>
    /// Unity Editor button binding: Fade out + destroy (standard behavior)
    /// </summary>
    public void OnCloseButton_AnimatedDestroy()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Close button (animated destroy) clicked for {overlayType}", this);
        FadeOutAndDestroy();
    }

    /// <summary>
    /// Unity Editor button binding: Fade out only (no destroy)
    /// </summary>
    public void OnCloseButton_FadeOnly()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Close button (fade only) clicked for {overlayType}", this);
        StartCoroutine(FadeOutAsync());
    }

    /// <summary>
    /// Unity Editor button binding: Instant destroy (no animation)
    /// </summary>
    public void OnCloseButton_InstantDestroy()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Close button (instant destroy) clicked for {overlayType}", this);
        DestroyImmediately();
    }

    /// <summary>
    /// Gets the close animation duration for this overlay.
    /// Returns 0 if animation is disabled or config is unavailable.
    /// </summary>
    public float GetCloseAnimationDuration()
    {
        if (TransitionConfigProvider.Instance == null || TransitionConfigProvider.Instance.GetConfig() == null)
        {
            return 0f;
        }

        TransitionSettings settings = TransitionConfigProvider.Instance.GetSettingsForOverlay(overlayType);

        if (!settings.enableFadeOut) // Use fadeOut for overlay close (alpha 1 -> 0)
        {
            return 0f;
        }

        return settings.fadeOutDuration;
    }

    #endregion

    #region Animation Methods

    /// <summary>
    /// Plays the opening animation for this overlay.
    /// </summary>
    private void PlayOpenAnimation()
    {
        if (TransitionConfigProvider.Instance == null || TransitionConfigProvider.Instance.GetConfig() == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "TransitionConfigProvider or config not available - skipping animation", this);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            return;
        }

        TransitionSettings settings = TransitionConfigProvider.Instance.GetSettingsForOverlay(overlayType);

        if (!settings.enableFadeIn) // Use fadeIn for overlay open (alpha 0 -> 1)
        {
            // No animation - just show overlay immediately
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Open animation disabled for {overlayType}", this);
            return;
        }

        currentAnimation = StartCoroutine(OpenAnimationCoroutine(settings));
    }

    /// <summary>
    /// Coroutine for opening animation (fade in from transparent to visible).
    /// Animates ONLY the CanvasGroup (UI elements).
    /// GameObjectAnimator on tiles handles their own spawn animation independently.
    /// </summary>
    private IEnumerator OpenAnimationCoroutine(TransitionSettings settings)
    {
        isAnimating = true;

        // Disable interaction during animation
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Set CanvasGroup initial state (transparent)
        canvasGroup.alpha = 0f;

        DebugLogger.Log(DebugLogCategory.SceneManager,
            $"Playing fade-in animation for {overlayType} ({settings.fadeInDuration}s)", this);

        // Animate fade in (alpha 0 -> 1) - CanvasGroup ONLY
        float elapsed = 0f;
        while (elapsed < settings.fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time (works during pause)
            float t = Mathf.Clamp01(elapsed / settings.fadeInDuration);
            float curveValue = settings.fadeInCurve.Evaluate(t);

            // Fade CanvasGroup (UI elements)
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue);

            yield return null;
        }

        // Ensure final state (fully visible)
        canvasGroup.alpha = 1f;

        // Enable interaction
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isAnimating = false;
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Fade-in animation complete for {overlayType}", this);
    }

    /// <summary>
    /// Coroutine for fade out animation (visible → transparent).
    /// Animates ONLY the CanvasGroup (UI elements).
    /// Does NOT destroy overlay - caller must call DestroyImmediately() if needed.
    /// </summary>
    private IEnumerator FadeOutCoroutine()
    {
        if (TransitionConfigProvider.Instance == null || TransitionConfigProvider.Instance.GetConfig() == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "TransitionConfigProvider or config not available - destroying immediately", this);
            Destroy(gameObject);
            yield break;
        }

        TransitionSettings settings = TransitionConfigProvider.Instance.GetSettingsForOverlay(overlayType);

        if (!settings.enableFadeOut) // Use fadeOut for overlay close (alpha 1 -> 0)
        {
            // No animation - destroy immediately
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Close animation disabled for {overlayType} - destroying immediately", this);
            Destroy(gameObject);
            yield break;
        }

        isAnimating = true;

        // Disable interaction immediately
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Disable all PulsatingSprite components to prevent interference during fade
        PulsatingSprite[] pulsatingSprites = GetComponentsInChildren<PulsatingSprite>();
        foreach (var pulsating in pulsatingSprites)
        {
            if (pulsating != null)
            {
                pulsating.enabled = false;  // Stop Update() from running
            }
        }

        float startAlpha = canvasGroup.alpha;

        DebugLogger.Log(DebugLogCategory.SceneManager,
            $"Playing fade-out animation for {overlayType} ({settings.fadeOutDuration}s)", this);

        // Animate fade out (alpha 1 -> 0) - CanvasGroup ONLY
        float elapsed = 0f;
        while (elapsed < settings.fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time (works during pause)
            float t = Mathf.Clamp01(elapsed / settings.fadeOutDuration);
            float curveValue = settings.fadeOutCurve.Evaluate(t);

            // Fade CanvasGroup (UI elements)
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, curveValue);

            yield return null;
        }

        DebugLogger.Log(DebugLogCategory.SceneManager, $"Fade-out complete for {overlayType}", this);
        isAnimating = false;
    }

    /// <summary>
    /// Helper coroutine: Fade out then destroy.
    /// </summary>
    private IEnumerator FadeOutAndDestroyCoroutine()
    {
        yield return StartCoroutine(FadeOutCoroutine());
        DestroyImmediately();
    }

    #endregion

    #region Debug/Testing

    /// <summary>
    /// For testing: manually trigger fade out and destroy.
    /// </summary>
    [ContextMenu("Test Close Animation")]
    private void TestCloseAnimation()
    {
        FadeOutAndDestroy();
    }

    #endregion
}
