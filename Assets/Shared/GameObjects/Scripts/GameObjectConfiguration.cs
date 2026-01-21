using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized configuration for GameObject spawn animations and runtime effects.
/// Used by GameObjectAnimator component to control fade-in animation and visual effects.
/// Create asset via: Assets -> Create -> Config -> GameObject Configuration
/// </summary>
[CreateAssetMenu(fileName = "GameObjectConfiguration", menuName = "Config/GameObject Configuration")]
public class GameObjectConfiguration : ScriptableObject
{
    [Header("Spawn Animation Settings")]
    [Tooltip("Should this GameObject fade in when spawned?")]
    public bool enableFadeIn = false;

    [Tooltip("Duration of fade-in animation in seconds")]
    public float fadeInDuration = 0.3f;

    [Tooltip("Animation curve for fade-in (0=start, 1=end)")]
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Initial alpha when spawned (0=invisible, 1=visible)")]
    [Range(0f, 1f)]
    public float initialAlpha = 0f;

    [Tooltip("Target alpha after fade-in completes (0=invisible, 1=visible)")]
    [Range(0f, 1f)]
    public float targetAlpha = 1f;

    [Header("Runtime Effects")]
    [Tooltip("Visual effects to apply after spawn animation completes")]
    public List<RuntimeEffectConfig> runtimeEffects = new List<RuntimeEffectConfig>();
}

/// <summary>
/// Configuration for a runtime visual effect to apply to a GameObject.
/// Supports multiple effect types (Pulsating, future: Rotating, Scaling, etc.)
/// </summary>
[System.Serializable]
public class RuntimeEffectConfig
{
    [Tooltip("Type of runtime effect to apply")]
    public RuntimeEffectType effectType = RuntimeEffectType.None;

    [Tooltip("Is this effect enabled?")]
    public bool enabled = true;

    [Header("Pulsating Effect Settings")]
    [Tooltip("Speed of pulsation animation")]
    public float pulseSpeed = 2f;

    [Tooltip("Scale oscillation range (0-1)")]
    [Range(0f, 1f)]
    public float scaleMultiplier = 0.2f;

    [Tooltip("Brightness/contrast oscillation range (0-1)")]
    [Range(0f, 1f)]
    public float contrastMultiplier = 0.2f;

    // Future effects can add more parameters here:
    // [Header("Rotating Effect Settings")]
    // public float rotationSpeed = 90f;
    //
    // [Header("Scaling Effect Settings")]
    // public float scaleSpeed = 1f;
    // public float minScale = 0.8f;
    // public float maxScale = 1.2f;
}

/// <summary>
/// Types of runtime visual effects that can be applied to GameObjects.
/// </summary>
public enum RuntimeEffectType
{
    None,
    Pulsating,
    // Future effects:
    // Rotating,
    // Scaling,
    // Glowing,
    // Particle,
}
