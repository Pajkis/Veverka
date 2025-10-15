using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Initialize user levels on game start
/// </summary>
public class UserLevelInitializer : MonoBehaviour
{
    [SerializeField] private DisplayConfig displayConfig;
    [SerializeField] private AssetReference userLevelManualAsset;

    /// <summary>
    /// Initialize user levels - call this from GameInit or other initialization system
    /// </summary>
    public IEnumerator InitializeUserLevels()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, "UserLevelInitializer: Starting initialization");

        if (displayConfig != null)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "DisplayConfig assigned in Inspector");
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "DisplayConfig not assigned - using fallback values");
        }

        // Load manual from Addressables
        TextAsset manualTextAsset = null;
        if (userLevelManualAsset != null && userLevelManualAsset.RuntimeKeyIsValid())
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "Loading UserLevelManual from Addressables");

            AsyncOperationHandle<TextAsset> handle = userLevelManualAsset.LoadAssetAsync<TextAsset>();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                manualTextAsset = handle.Result;
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"UserLevelManual loaded successfully: {manualTextAsset.text.Length} characters");
            }
            else
            {
                DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to load UserLevelManual from Addressables: {handle.OperationException}");
            }
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "UserLevelManual AssetReference not assigned or invalid");
        }

        // Initialize user levels with manual
        UserLevelSet.InitializeUserLevels(displayConfig, manualTextAsset);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User levels initialization complete at: {UserLevelSet.GetUserLevelsPath()}");
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor menu to manually initialize user levels
    /// </summary>
    [UnityEditor.MenuItem("Tools/User Levels/Initialize User Levels")]
    private static void EditorInitializeUserLevels()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Editor: Manual user level initialization requested");

        DisplayConfig config = null;

        // Try to find DisplayConfig in assets
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Editor: Searching for DisplayConfig in project assets");
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
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"User levels folder does not exist at: {path}");
        }
    }
#endif
}
