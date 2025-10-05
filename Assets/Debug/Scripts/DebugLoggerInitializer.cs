using UnityEngine;

/// <summary>
/// Global initializer for DebugLogger system
/// Ensures DebugLogger is initialized before any other scripts try to use it
/// </summary>
public class DebugLoggerInitializer : MonoBehaviour
{
    [SerializeField] private DebugLogConfig debugConfig;

    /// <summary>
    /// Initialize DebugLogger as early as possible
    /// This runs before any scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad()
    {
        // Try to find DebugLogConfig in the project
        DebugLogConfig config = FindDebugLogConfig();

        if (config != null)
        {
            DebugLogger.Initialize(config);
            Debug.Log($"[DebugLogger] Initialized globally with config from: {config.name}");
        }
        else
        {
            Debug.LogWarning("[DebugLogger] No DebugLogConfig found - debug logging may not work properly");
        }
    }

    /// <summary>
    /// Find DebugLogConfig in the project
    /// </summary>
    private static DebugLogConfig FindDebugLogConfig()
    {
#if UNITY_EDITOR
        // In editor, search assets
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DebugLogConfig");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<DebugLogConfig>(path);
        }
#else
        // In build, try to load from Resources folder
        return Resources.Load<DebugLogConfig>("DebugLogConfig");
#endif
        return null;
    }
}
