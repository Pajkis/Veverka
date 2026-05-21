using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static helper class for applying runtime visual effects to GameObjects.
/// Called by GameObjectAnimator after spawn animation completes.
/// Easily extensible for future effect types (Rotating, Scaling, Glowing, Particle, etc.)
/// </summary>
public static class RuntimeEffectApplicator
{
    /// <summary>
    /// Apply runtime effects to a GameObject based on effect configurations.
    /// Iterates through all effect configs and applies enabled effects.
    /// </summary>
    /// <param name="targetObject">GameObject to apply effects to</param>
    /// <param name="effects">List of effect configurations to apply</param>
    public static void ApplyEffects(GameObject targetObject, List<RuntimeEffectConfig> effects)
    {
        if (effects == null || effects.Count == 0)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem,
                $"No runtime effects to apply on {targetObject.name}", targetObject);
            return;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem,
            $"Applying {effects.Count} runtime effect(s) to {targetObject.name}", targetObject);

        foreach (var effectConfig in effects)
        {
            if (!effectConfig.enabled)
            {
                DebugLogger.Log(DebugLogCategory.GridSystem,
                    $"Skipping disabled effect {effectConfig.effectType} on {targetObject.name}", targetObject);
                continue;
            }

            switch (effectConfig.effectType)
            {
                case RuntimeEffectType.Pulsating:
                    ApplyPulsatingEffect(targetObject, effectConfig);
                    break;

                case RuntimeEffectType.None:
                    // No effect - skip
                    break;

                // Future effects can be added here:
                // case RuntimeEffectType.Rotating:
                //     ApplyRotatingEffect(targetObject, effectConfig);
                //     break;
                //
                // case RuntimeEffectType.Scaling:
                //     ApplyScalingEffect(targetObject, effectConfig);
                //     break;
                //
                // case RuntimeEffectType.Glowing:
                //     ApplyGlowingEffect(targetObject, effectConfig);
                //     break;
            }
        }
    }

    /// <summary>
    /// Applies pulsating effect to a GameObject.
    /// If PulsatingSprite already exists on prefab, updates its parameters from config.
    /// Otherwise, adds new PulsatingSprite component with config parameters.
    /// </summary>
    private static void ApplyPulsatingEffect(GameObject targetObject, RuntimeEffectConfig config)
    {
        // Check if already has PulsatingSprite (manually added to prefab)
        PulsatingSprite existing = targetObject.GetComponent<PulsatingSprite>();
        if (existing != null)
        {
            // Update parameters from config
            existing.pulseSpeed = config.pulseSpeed;
            existing.scaleMultiplier = config.scaleMultiplier;
            existing.contrastMultiplier = config.contrastMultiplier;

            DebugLogger.Log(DebugLogCategory.GridSystem,
                $"Updated existing PulsatingSprite on {targetObject.name} (speed={config.pulseSpeed}, scale={config.scaleMultiplier}, contrast={config.contrastMultiplier})",
                targetObject);
            return;
        }

        // Add new PulsatingSprite component
        PulsatingSprite pulsating = targetObject.AddComponent<PulsatingSprite>();
        pulsating.pulseSpeed = config.pulseSpeed;
        pulsating.scaleMultiplier = config.scaleMultiplier;
        pulsating.contrastMultiplier = config.contrastMultiplier;

        DebugLogger.Log(DebugLogCategory.GridSystem,
            $"Applied PulsatingSprite effect to {targetObject.name} (speed={config.pulseSpeed}, scale={config.scaleMultiplier}, contrast={config.contrastMultiplier})",
            targetObject);
    }

    // Future effect implementations:
    //
    // private static void ApplyRotatingEffect(GameObject targetObject, RuntimeEffectConfig config)
    // {
    //     RotatingSprite rotating = targetObject.AddComponent<RotatingSprite>();
    //     rotating.rotationSpeed = config.rotationSpeed;
    //     DebugLogger.Log(DebugLogCategory.GridSystem,
    //         $"Applied RotatingSprite effect to {targetObject.name} (speed={config.rotationSpeed})", targetObject);
    // }
    //
    // private static void ApplyScalingEffect(GameObject targetObject, RuntimeEffectConfig config)
    // {
    //     ScalingSprite scaling = targetObject.AddComponent<ScalingSprite>();
    //     scaling.scaleSpeed = config.scaleSpeed;
    //     scaling.minScale = config.minScale;
    //     scaling.maxScale = config.maxScale;
    //     DebugLogger.Log(DebugLogCategory.GridSystem,
    //         $"Applied ScalingSprite effect to {targetObject.name}", targetObject);
    // }
}
