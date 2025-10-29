using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Configuration for scene transition visual effects and timing.
/// Supports global defaults and per-scene overrides (e.g., disable fade-in for loading screens).
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

    [Tooltip("Color to fade to/from (usually black)")]
    public Color fadeColor = Color.black;

    [Tooltip("Animation curve for fade out (0 to 1)")]
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Animation curve for fade in (0 to 1)")]
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Enable fade out before scene loads (default behavior)")]
    public bool enableFadeOut = true;

    [Tooltip("Enable fade in after scene loads (default behavior)")]
    public bool enableFadeIn = true;

    [Header("Per-Scene Overrides")]
    [Tooltip("Scene-specific transition settings (overrides defaults)")]
    public List<SceneTransitionOverride> sceneOverrides = new List<SceneTransitionOverride>();

    /// <summary>
    /// Gets the transition settings for a specific scene, using overrides if available.
    /// </summary>
    /// <param name="sceneType">The scene to get settings for</param>
    /// <returns>Transition settings (either override or defaults)</returns>
    public SceneTransitionSettings GetSettingsForScene(SceneType sceneType)
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
        return new SceneTransitionSettings
        {
            fadeOutDuration = this.fadeOutDuration,
            fadeInDuration = this.fadeInDuration,
            fadeColor = this.fadeColor,
            fadeOutCurve = this.fadeOutCurve,
            fadeInCurve = this.fadeInCurve,
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
    public SceneTransitionSettings settings;
}

/// <summary>
/// Transition settings that can be customized per scene.
/// </summary>
[System.Serializable]
public class SceneTransitionSettings
{
    [Tooltip("Duration of fade out animation in seconds")]
    [Range(0.1f, 2f)]
    public float fadeOutDuration = 0.4f;

    [Tooltip("Duration of fade in animation in seconds")]
    [Range(0.1f, 2f)]
    public float fadeInDuration = 0.3f;

    [Tooltip("Color to fade to/from")]
    public Color fadeColor = Color.black;

    [Tooltip("Animation curve for fade out")]
    public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Animation curve for fade in")]
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Enable fade out before scene loads (disable to skip fade out)")]
    public bool enableFadeOut = true;

    [Tooltip("Enable fade in after scene loads (disable for loading screens with visible background)")]
    public bool enableFadeIn = true;
}
