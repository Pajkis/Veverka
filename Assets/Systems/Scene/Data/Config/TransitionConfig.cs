using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Unified configuration for all transitions in the game.
/// Handles both scene transitions and overlay animations with shared defaults and per-item overrides.
/// </summary>
[CreateAssetMenu(fileName = "TransitionConfig", menuName = "Configs/TransitionConfig")]
public class TransitionConfig : ScriptableObject
{
    [Header("Default Transition Settings")]
    [Tooltip("Duration of fade out animation in seconds")]
    [Range(0.1f, 2f)]
    public float fadeOutDuration = 0.4f;

    [Tooltip("Duration of fade in animation in seconds")]
    [Range(0.1f, 2f)]
    public float fadeInDuration = 0.3f;

    [Tooltip("Animation curve for fade out (0 to 1)")]
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Animation curve for fade in (0 to 1)")]
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Color to fade to/from (Scene transitions only - overlays use alpha fade)")]
    public Color fadeColor = Color.black;

    [Tooltip("Enable fade out before transition/open (default behavior)")]
    public bool enableFadeOut = true;

    [Tooltip("Enable fade in after transition/open (default behavior)")]
    public bool enableFadeIn = true;

    [Header("Scene Transition Overrides")]
    [Tooltip("Scene-specific transition settings (overrides defaults)")]
    public List<SceneTransitionOverride> sceneOverrides = new List<SceneTransitionOverride>();

    [Header("Overlay Animation Overrides")]
    [Tooltip("Overlay-specific transition settings (overrides defaults)")]
    public List<OverlayTransitionOverride> overlayOverrides = new List<OverlayTransitionOverride>();

    /// <summary>
    /// Gets transition settings for a specific scene, using overrides if available.
    /// </summary>
    public TransitionSettings GetSettingsForScene(SceneType sceneType)
    {
        // Check for scene-specific override
        foreach (var overrideSettings in sceneOverrides)
        {
            if (overrideSettings.sceneType == sceneType)
            {
                return overrideSettings.settings;
            }
        }

        // Return default settings
        return new TransitionSettings
        {
            fadeOutDuration = this.fadeOutDuration,
            fadeInDuration = this.fadeInDuration,
            fadeOutCurve = this.fadeOutCurve,
            fadeInCurve = this.fadeInCurve,
            fadeColor = this.fadeColor,
            enableFadeOut = this.enableFadeOut,
            enableFadeIn = this.enableFadeIn
        };
    }

    /// <summary>
    /// Gets transition settings for a specific overlay, using overrides if available.
    /// </summary>
    public TransitionSettings GetSettingsForOverlay(OverlayType overlayType)
    {
        // Check for overlay-specific override
        foreach (var overrideSettings in overlayOverrides)
        {
            if (overrideSettings.overlayType == overlayType)
            {
                return overrideSettings.settings;
            }
        }

        // Return default settings
        return new TransitionSettings
        {
            fadeOutDuration = this.fadeOutDuration,
            fadeInDuration = this.fadeInDuration,
            fadeOutCurve = this.fadeOutCurve,
            fadeInCurve = this.fadeInCurve,
            fadeColor = this.fadeColor,
            enableFadeOut = this.enableFadeOut,
            enableFadeIn = this.enableFadeIn
        };
    }
}

/// <summary>
/// Per-scene transition settings override.
/// </summary>
[System.Serializable]
public class SceneTransitionOverride
{
    [Tooltip("Which scene these settings apply to")]
    public SceneType sceneType;

    [Tooltip("Custom transition settings for this scene")]
    public TransitionSettings settings;
}

/// <summary>
/// Per-overlay transition settings override.
/// </summary>
[System.Serializable]
public class OverlayTransitionOverride
{
    [Tooltip("Which overlay these settings apply to")]
    public OverlayType overlayType;

    [Tooltip("Custom transition settings for this overlay")]
    public TransitionSettings settings;
}

/// <summary>
/// Transition settings that can be customized per scene or overlay.
/// </summary>
[System.Serializable]
public class TransitionSettings
{
    [Tooltip("Duration of fade out animation in seconds")]
    [Range(0.1f, 2f)]
    public float fadeOutDuration = 0.4f;

    [Tooltip("Duration of fade in animation in seconds")]
    [Range(0.1f, 2f)]
    public float fadeInDuration = 0.3f;

    [Tooltip("Animation curve for fade out")]
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Animation curve for fade in")]
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Color to fade to/from (Scene transitions only)")]
    public Color fadeColor = Color.black;

    [Tooltip("Enable fade out before scene loads or overlay opens")]
    public bool enableFadeOut = true;

    [Tooltip("Enable fade in after scene loads or overlay opens")]
    public bool enableFadeIn = true;
}
