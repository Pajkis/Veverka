using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Self-contained spawn animation component for GameObjects (tiles, characters, props).
/// Automatically handles fade-in animation and runtime effect application on instantiate.
/// Attach to any prefab that needs spawn animation.
/// </summary>
public class GameObjectAnimator : MonoBehaviour
{
    [Header("Configuration Source")]
    [Tooltip("Default configuration asset (centralized settings). Leave empty to use custom settings below.")]
    [SerializeField] private GameObjectConfiguration configAsset;

    [Tooltip("Use custom settings instead of config asset? If checked, values below override the asset.")]
    [SerializeField] private bool useCustomSettings = false;

    [Header("Custom Spawn Animation (only if useCustomSettings = true)")]
    [Tooltip("Should this GameObject fade in when spawned?")]
    [SerializeField] private bool enableFadeIn = false;

    [Tooltip("Duration of fade-in animation in seconds")]
    [SerializeField] private float fadeInDuration = 0.3f;

    [Tooltip("Animation curve for fade-in (0=start, 1=end)")]
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Initial alpha when spawned (0=invisible, 1=visible)")]
    [SerializeField] [Range(0f, 1f)] private float initialAlpha = 0f;

    [Tooltip("Target alpha after fade-in completes (0=invisible, 1=visible)")]
    [SerializeField] [Range(0f, 1f)] private float targetAlpha = 1f;

    [Header("Custom Runtime Effects (only if useCustomSettings = true)")]
    [Tooltip("Visual effects to apply after spawn animation completes")]
    [SerializeField] private List<RuntimeEffectConfig> customRuntimeEffects = new List<RuntimeEffectConfig>();

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    private void Awake()
    {
        // Cache all sprite renderers (including children)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];

        // Store original colors and apply initial alpha BEFORE first frame renders
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;

            // Apply initial alpha from config (prevents 1-frame visibility gap)
            float initAlpha = GetInitialAlpha();
            Color color = spriteRenderers[i].color;
            color.a = initAlpha;
            spriteRenderers[i].color = color;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem,
            $"GameObjectAnimator initialized on {gameObject.name} - {spriteRenderers.Length} renderers set to alpha {GetInitialAlpha()}", this);
    }

    private void Start()
    {
        // Start fade-in animation if enabled
        if (ShouldFadeIn())
        {
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            // No fade-in - apply runtime effects immediately
            ApplyRuntimeEffects();
        }
    }

    /// <summary>
    /// Coroutine that animates sprite alpha from initialAlpha to targetAlpha over fadeInDuration.
    /// Uses AnimationCurve for smooth easing. Applies runtime effects after completion.
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        float duration = GetFadeInDuration();
        AnimationCurve curve = GetFadeInCurve();
        float initAlpha = GetInitialAlpha();
        float targetAlpha = GetTargetAlpha();

        DebugLogger.Log(DebugLogCategory.GridSystem,
            $"Starting fade-in animation on {gameObject.name} ({duration}s, {initAlpha:F2} → {targetAlpha:F2})", this);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = curve.Evaluate(t);

            // Lerp alpha for all sprite renderers
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null) continue;

                Color color = originalColors[i];
                color.a = Mathf.Lerp(initAlpha, targetAlpha, curveValue);
                spriteRenderers[i].color = color;
            }

            yield return null;
        }

        // Ensure final alpha state
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            Color color = originalColors[i];
            color.a = targetAlpha;
            spriteRenderers[i].color = color;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Fade-in complete on {gameObject.name}", this);

        // Apply runtime effects AFTER fade-in completes
        ApplyRuntimeEffects();
    }

    /// <summary>
    /// Applies runtime visual effects (PulsatingSprite, future effects) to this GameObject.
    /// Called after fade-in completes or immediately if fade-in is disabled.
    /// </summary>
    private void ApplyRuntimeEffects()
    {
        List<RuntimeEffectConfig> effects = GetRuntimeEffects();
        RuntimeEffectApplicator.ApplyEffects(gameObject, effects);
    }

    /// <summary>
    /// Fade out animation - fades from current alpha to 0.
    /// Returns coroutine that can be yielded.
    /// </summary>
    public IEnumerator FadeOut(float duration, AnimationCurve curve)
    {
        // Disable PulsatingSprite if present
        PulsatingSprite pulsating = GetComponent<PulsatingSprite>();
        if (pulsating != null)
        {
            pulsating.enabled = false;
        }

        // Store current alphas as start values
        float[] startAlphas = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                startAlphas[i] = spriteRenderers[i].color.a;
            }
        }

        DebugLogger.Log(DebugLogCategory.GridSystem,
            $"Starting fade-out animation on {gameObject.name} ({duration}s)", this);

        // Animate fade out
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = curve.Evaluate(t);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null) continue;

                Color color = originalColors[i];
                color.a = Mathf.Lerp(startAlphas[i], 0f, curveValue);
                spriteRenderers[i].color = color;
            }

            yield return null;
        }

        // Ensure final state (invisible)
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            Color color = originalColors[i];
            color.a = 0f;
            spriteRenderers[i].color = color;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Fade-out complete on {gameObject.name}", this);
    }

    #region Configuration Getters

    private bool ShouldFadeIn()
    {
        if (useCustomSettings) return enableFadeIn;
        if (configAsset == null) return false;
        return configAsset.enableFadeIn;
    }

    private float GetFadeInDuration()
    {
        if (useCustomSettings) return fadeInDuration;
        if (configAsset == null) return 0.3f;
        return configAsset.fadeInDuration;
    }

    private AnimationCurve GetFadeInCurve()
    {
        if (useCustomSettings) return fadeInCurve;
        if (configAsset == null) return AnimationCurve.EaseInOut(0, 0, 1, 1);
        return configAsset.fadeInCurve;
    }

    private float GetInitialAlpha()
    {
        if (useCustomSettings) return initialAlpha;
        if (configAsset == null) return 0f;
        return configAsset.initialAlpha;
    }

    private float GetTargetAlpha()
    {
        if (useCustomSettings) return targetAlpha;
        if (configAsset == null) return 1f;
        return configAsset.targetAlpha;
    }

    private List<RuntimeEffectConfig> GetRuntimeEffects()
    {
        if (useCustomSettings) return customRuntimeEffects;
        if (configAsset == null) return new List<RuntimeEffectConfig>();
        return configAsset.runtimeEffects;
    }

    #endregion
}
