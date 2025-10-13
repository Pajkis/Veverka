using System.IO;
using UnityEngine;

/// <summary>
/// Manages user level operations: import, export, and manual download
/// </summary>
public class UserLevelManager : MonoBehaviour
{
    private const string MANUAL_FILE_NAME = "UserLevelManual.txt";

    [Header("Current Level")]
    [SerializeField] private int currentLevelNumber = 0; // Which level to export (0-9)

    /// <summary>
    /// Set which level to export/import
    /// </summary>
    public void SetCurrentLevel(int levelNumber)
    {
        currentLevelNumber = levelNumber;
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"UserLevelManager: Current level set to {levelNumber}");
    }

    #region Manual Operations

    /// <summary>
    /// Save manual to local storage (Downloads on Android, Documents on Windows)
    /// </summary>
    public void SaveManual()
    {
        string manualPath = Path.Combine(UserLevelSet.GetUserLevelsPath(), MANUAL_FILE_NAME);

        if (!File.Exists(manualPath))
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Manual not found at: {manualPath}");
            NotificationManager.ShowError("Manual not found in game files");
            return;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Saving manual from: {manualPath}");

#if UNITY_ANDROID
        SaveManualToDownloadsAndroid(manualPath);
#else
        SaveManualWindows(manualPath);
#endif
    }

    /// <summary>
    /// Save manual to Downloads folder on Android
    /// </summary>
    private void SaveManualToDownloadsAndroid(string manualPath)
    {
        try
        {
            string downloadsPath = "/storage/emulated/0/Download";
            string destinationPath = Path.Combine(downloadsPath, MANUAL_FILE_NAME);

            // Always overwrite to get latest version
            File.Copy(manualPath, destinationPath, overwrite: true);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Manual saved to Downloads: {destinationPath}");
            NotificationManager.ShowSuccess("Manual saved to Downloads folder");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to save manual to Downloads: {ex.Message}");
            NotificationManager.ShowError("Failed to save manual");
        }
    }

    /// <summary>
    /// Copy manual to Documents folder on Windows
    /// </summary>
    private void SaveManualWindows(string manualPath)
    {
        try
        {
            // Create GetNuts folder in Documents
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string getNutsFolder = Path.Combine(documentsPath, "GetNuts");

            if (!Directory.Exists(getNutsFolder))
            {
                Directory.CreateDirectory(getNutsFolder);
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Created folder: {getNutsFolder}");
            }

            // Copy manual
            string destinationPath = Path.Combine(getNutsFolder, MANUAL_FILE_NAME);
            File.Copy(manualPath, destinationPath, overwrite: true);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Manual copied to: {destinationPath}");

            // Open Explorer at the folder
            System.Diagnostics.Process.Start("explorer.exe", getNutsFolder);

            NotificationManager.ShowSuccess($"Manual saved to Documents\\GetNuts");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to copy manual: {ex.Message}");
            NotificationManager.ShowError("Failed to save manual");
        }
    }

    #endregion

    #region Level Export Operations

    /// <summary>
    /// Save current level to local storage (Downloads on Android, Documents on Windows)
    /// </summary>
    public void SaveLevel()
    {
        string levelPath = UserLevelSet.GetUserLevelPath(currentLevelNumber);

        if (!File.Exists(levelPath))
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Level file not found at: {levelPath}");
            NotificationManager.ShowError($"Level {currentLevelNumber + 1} not found", "404");
            return;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Saving level {currentLevelNumber + 1} from: {levelPath}");

#if UNITY_ANDROID
        SaveLevelToDownloadsAndroid(levelPath);
#else
        SaveLevelWindows(levelPath);
#endif
    }

    /// <summary>
    /// Save level to Downloads folder on Android
    /// </summary>
    private void SaveLevelToDownloadsAndroid(string levelPath)
    {
        try
        {
            string downloadsPath = "/storage/emulated/0/Download";
            string fileName = $"UserLevel{currentLevelNumber + 1}.csv";
            string destinationPath = Path.Combine(downloadsPath, fileName);

            // Always overwrite to get latest version
            File.Copy(levelPath, destinationPath, overwrite: true);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level saved to Downloads: {destinationPath}");
            NotificationManager.ShowSuccess($"Level {currentLevelNumber + 1} saved to Downloads");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to save level to Downloads: {ex.Message}");
            NotificationManager.ShowError("Failed to save level", "500");
        }
    }

    /// <summary>
    /// Copy level to Documents folder on Windows
    /// </summary>
    private void SaveLevelWindows(string levelPath)
    {
        try
        {
            // Create GetNuts folder in Documents
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string getNutsFolder = Path.Combine(documentsPath, "GetNuts");

            if (!Directory.Exists(getNutsFolder))
            {
                Directory.CreateDirectory(getNutsFolder);
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Created folder: {getNutsFolder}");
            }

            // Copy level
            string fileName = $"UserLevel{currentLevelNumber + 1}.csv";
            string destinationPath = Path.Combine(getNutsFolder, fileName);
            File.Copy(levelPath, destinationPath, overwrite: true);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level copied to: {destinationPath}");

            // Open Explorer at the folder
            System.Diagnostics.Process.Start("explorer.exe", getNutsFolder);

            NotificationManager.ShowSuccess($"Level {currentLevelNumber + 1} saved to Documents\\GetNuts");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to copy level: {ex.Message}");
            NotificationManager.ShowError("Failed to save level", "500");
        }
    }

    #endregion
}
