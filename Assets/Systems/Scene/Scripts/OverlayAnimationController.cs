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
    private OverlayType overlayType;

    // Cached components
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

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

        rectTransform = GetComponent<RectTransform>();
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
    /// Closes the overlay with animation, then destroys it.
    /// </summary>
    public void CloseWithAnimation()
    {
        if (isAnimating)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Animation already in progress - ignoring close request", this);
            return;
        }

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(CloseAnimationCoroutine());
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

        if (!settings.enableFadeOut) // enableFadeOut for overlays = enableOpenAnimation
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
    /// Coroutine for opening animation.
    /// </summary>
    private IEnumerator OpenAnimationCoroutine(TransitionSettings settings)
    {
        isAnimating = true;

        // Disable interaction during animation
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Set initial state
        canvasGroup.alpha = 0f;

        DebugLogger.Log(DebugLogCategory.SceneManager, $"Playing fade open animation for {overlayType} ({settings.fadeOutDuration}s)", this);

        // Animate fade in
        float elapsed = 0f;
        while (elapsed < settings.fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time (works during pause)
            float t = Mathf.Clamp01(elapsed / settings.fadeOutDuration);
            float curveValue = settings.fadeOutCurve.Evaluate(t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue);
            yield return null;
        }

        // Ensure final state
        canvasGroup.alpha = 1f;

        // Enable interaction
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isAnimating = false;
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Open animation complete for {overlayType}", this);
    }

    /// <summary>
    /// Coroutine for closing animation.
    /// </summary>
    private IEnumerator CloseAnimationCoroutine()
    {
        if (TransitionConfigProvider.Instance == null || TransitionConfigProvider.Instance.GetConfig() == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "TransitionConfigProvider or config not available - destroying immediately", this);
            Destroy(gameObject);
            yield break;
        }

        TransitionSettings settings = TransitionConfigProvider.Instance.GetSettingsForOverlay(overlayType);

        if (!settings.enableFadeIn) // enableFadeIn for overlays = enableCloseAnimation
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

        float startAlpha = canvasGroup.alpha;

        DebugLogger.Log(DebugLogCategory.SceneManager, $"Playing fade close animation for {overlayType} ({settings.fadeInDuration}s)", this);

        // Animate fade out
        float elapsed = 0f;
        while (elapsed < settings.fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time (works during pause)
            float t = Mathf.Clamp01(elapsed / settings.fadeInDuration);
            float curveValue = settings.fadeInCurve.Evaluate(t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, curveValue);
            yield return null;
        }

        // Destroy overlay
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Close animation complete for {overlayType} - destroying", this);
        Destroy(gameObject);
    }

    #endregion

    #region Debug/Testing

    /// <summary>
    /// For testing: manually trigger close animation.
    /// </summary>
    [ContextMenu("Test Close Animation")]
    private void TestCloseAnimation()
    {
        CloseWithAnimation();
    }

    #endregion
}
