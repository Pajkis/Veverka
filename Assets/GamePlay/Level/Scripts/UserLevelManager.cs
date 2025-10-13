using System.IO;
using UnityEngine;

/// <summary>
/// Manages user level operations: import, export, and manual download
/// </summary>
public class UserLevelManager : MonoBehaviour
{
    private const string MANUAL_FILE_NAME = "UserLevelManual.txt";

    /// <summary>
    /// Save manual to local storage (Downloads on Android, Documents on Windows)
    /// </summary>
    public void SaveManual()
    {
        string manualPath = Path.Combine(UserLevelSet.GetUserLevelsPath(), MANUAL_FILE_NAME);

        if (!File.Exists(manualPath))
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Manual not found at: {manualPath}");
            // TODO: Show error message to user
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

            File.Copy(manualPath, destinationPath, overwrite: true);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Manual saved to Downloads: {destinationPath}");
            // TODO: Show success message to user
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to save manual to Downloads: {ex.Message}");
            // TODO: Show error message to user
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

            // TODO: Show success message to user
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to copy manual: {ex.Message}");
            // TODO: Show error message to user
        }
    }
}
