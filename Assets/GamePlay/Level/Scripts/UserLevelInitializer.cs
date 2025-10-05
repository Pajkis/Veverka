using UnityEngine;

/// <summary>
/// Initialize user levels on game start
/// </summary>
public class UserLevelInitializer : MonoBehaviour
{
    [SerializeField] private LevelSetManager levelSetManager;
    [SerializeField] private DisplayConfig displayConfig;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, "UserLevelInitializer: Starting runtime initialization");

        // Try to find DisplayConfig in the project
        DisplayConfig config = FindDisplayConfig();
        if (config != null)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "DisplayConfig found for user level initialization");
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "DisplayConfig not found - using fallback values");
        }

        UserLevelSet.InitializeUserLevels(config);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User levels runtime initialization complete at: {UserLevelSet.GetUserLevelsPath()}");
    }

    private static DisplayConfig FindDisplayConfig()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Searching for DisplayConfig in scene...");

        // Try to find in common locations or return null (will use fallback)
        var allConfigs = UnityEngine.Object.FindObjectsOfType<DisplayConfig>();
        if (allConfigs != null && allConfigs.Length > 0)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Found {allConfigs.Length} DisplayConfig instance(s)");
            return allConfigs[0];
        }

        DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "No DisplayConfig found in scene");
        return null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor menu to manually initialize user levels
    /// </summary>
    [UnityEditor.MenuItem("Tools/User Levels/Initialize User Levels")]
    private static void EditorInitializeUserLevels()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Editor: Manual user level initialization requested");

        DisplayConfig config = FindDisplayConfig();
        if (config == null)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "Editor: Searching for DisplayConfig in project assets");

            // Try to find it in assets
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DisplayConfig");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                config = UnityEditor.AssetDatabase.LoadAssetAtPath<DisplayConfig>(path);
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Editor: Found DisplayConfig at: {path}");
            }
            else
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "Editor: No DisplayConfig found in project assets");
            }
        }

        UserLevelSet.InitializeUserLevels(config);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Editor: User levels initialization complete at: {UserLevelSet.GetUserLevelsPath()}");
    }

    /// <summary>
    /// Editor menu to open user levels folder
    /// </summary>
    [UnityEditor.MenuItem("Tools/User Levels/Open User Levels Folder")]
    private static void OpenUserLevelsFolder()
    {
        string path = UserLevelSet.GetUserLevelsPath();
        if (System.IO.Directory.Exists(path))
        {
            System.Diagnostics.Process.Start(path);
        }
        else
        {
            Debug.LogWarning($"User levels folder does not exist at: {path}");
        }
    }
#endif
}
