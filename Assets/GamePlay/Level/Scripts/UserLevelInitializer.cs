using UnityEngine;

/// <summary>
/// Initialize user levels on game start
/// </summary>
public class UserLevelInitializer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        UserLevelSet.InitializeUserLevels();
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User levels initialized at: {UserLevelSet.GetUserLevelsPath()}");
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor menu to manually initialize user levels
    /// </summary>
    [UnityEditor.MenuItem("Tools/User Levels/Initialize User Levels")]
    private static void EditorInitializeUserLevels()
    {
        UserLevelSet.InitializeUserLevels();
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
