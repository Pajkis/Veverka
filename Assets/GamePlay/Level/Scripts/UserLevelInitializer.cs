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
        // Try to find DisplayConfig in the project
        DisplayConfig config = FindDisplayConfig();
        UserLevelSet.InitializeUserLevels(config);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User levels initialized at: {UserLevelSet.GetUserLevelsPath()}");
    }

    private static DisplayConfig FindDisplayConfig()
    {
        // Try to find in common locations or return null (will use fallback)
        var allConfigs = UnityEngine.Object.FindObjectsOfType<DisplayConfig>();
        if (allConfigs != null && allConfigs.Length > 0)
        {
            return allConfigs[0];
        }
        return null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor menu to manually initialize user levels
    /// </summary>
    [UnityEditor.MenuItem("Tools/User Levels/Initialize User Levels")]
    private static void EditorInitializeUserLevels()
    {
        DisplayConfig config = FindDisplayConfig();
        if (config == null)
        {
            // Try to find it in assets
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DisplayConfig");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                config = UnityEditor.AssetDatabase.LoadAssetAtPath<DisplayConfig>(path);
            }
        }

        UserLevelSet.InitializeUserLevels(config);
        Debug.Log($"User levels initialized at: {UserLevelSet.GetUserLevelsPath()}");
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
