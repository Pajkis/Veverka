using UnityEngine;

/// <summary>
/// Singleton provider for TransitionConfig.
/// Provides global access to transition configuration for both scene transitions and overlay animations.
/// Must exist in the Bootstrap scene with DontDestroyOnLoad.
/// </summary>
public class TransitionConfigProvider : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the provider.
    /// </summary>
    public static TransitionConfigProvider Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private TransitionConfig config;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DebugLogger.Log(DebugLogCategory.SceneManager, "TransitionConfigProvider initialized", this);
        }
        else if (Instance != this)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Duplicate TransitionConfigProvider found - destroying this instance", this);
            Destroy(gameObject);
            return;
        }

        // Validate config
        if (config == null)
        {
            DebugLogger.LogError(DebugLogCategory.SceneManager, "TransitionConfig not assigned to TransitionConfigProvider!", this);
        }
    }

    /// <summary>
    /// Gets the transition configuration.
    /// </summary>
    public TransitionConfig GetConfig()
    {
        if (config == null)
        {
            DebugLogger.LogError(DebugLogCategory.SceneManager, "TransitionConfig is null! Assign it in the Inspector.", this);
        }
        return config;
    }

    /// <summary>
    /// Shortcut to get transition settings for a scene.
    /// </summary>
    public TransitionSettings GetSettingsForScene(SceneType sceneType)
    {
        return config != null ? config.GetSettingsForScene(sceneType) : null;
    }

    /// <summary>
    /// Shortcut to get transition settings for an overlay.
    /// </summary>
    public TransitionSettings GetSettingsForOverlay(OverlayType overlayType)
    {
        return config != null ? config.GetSettingsForOverlay(overlayType) : null;
    }
}
