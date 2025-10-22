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
            NotificationManager.ShowError(ErrorCode.FileNotFound, "Manual");
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
            NotificationManager.ShowError(ErrorCode.FileWriteFailed, "Manual");
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
            NotificationManager.ShowError(ErrorCode.FileWriteFailed, "Manual");
        }
    }

    #endregion

    #region Level Export Operations

    /// <summary>
    /// Save all user levels to local storage (Downloads on Android, Documents on Windows)
    /// </summary>
    public void SaveAllLevels()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Starting export of all user levels");

        int successCount = 0;
        int failCount = 0;

        for (int i = 0; i < 10; i++) // 10 user levels (0-9)
        {
            string levelPath = UserLevelSet.GetUserLevelPath(i);

            if (!File.Exists(levelPath))
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"Level {i + 1} not found at: {levelPath}");
                failCount++;
                continue;
            }

            try
            {
#if UNITY_ANDROID
                string downloadsPath = "/storage/emulated/0/Download";
                string fileName = $"UserLevel{i + 1}.csv";
                string destinationPath = Path.Combine(downloadsPath, fileName);

                // Skip if file already exists
                if (File.Exists(destinationPath))
                {
                    DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level {i + 1} already exists in Downloads, skipping");
                    continue;
                }

                File.Copy(levelPath, destinationPath, overwrite: false);
#else
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                string getNutsFolder = Path.Combine(documentsPath, "GetNuts");

                if (!Directory.Exists(getNutsFolder))
                {
                    Directory.CreateDirectory(getNutsFolder);
                }

                string fileName = $"UserLevel{i + 1}.csv";
                string destinationPath = Path.Combine(getNutsFolder, fileName);

                // Skip if file already exists
                if (File.Exists(destinationPath))
                {
                    DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level {i + 1} already exists in Documents\\GetNuts, skipping");
                    continue;
                }

                File.Copy(levelPath, destinationPath, overwrite: false);
#endif

                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level {i + 1} exported successfully");
                successCount++;
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to export level {i + 1}: {ex.Message}");
                failCount++;
            }
        }

        // Show result notification
        if (failCount == 0)
        {
#if UNITY_ANDROID
            NotificationManager.ShowSuccess($"All {successCount} levels saved to Downloads");
#else
            System.Diagnostics.Process.Start("explorer.exe", Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "GetNuts"));
            NotificationManager.ShowSuccess($"All {successCount} levels saved to Documents\\GetNuts");
#endif
        }
        else if (successCount > 0)
        {
            NotificationManager.ShowSuccess($"{successCount} levels saved, {failCount} failed");
        }
        else
        {
            NotificationManager.ShowError(ErrorCode.InternalError, "Export all levels");
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Export all completed: {successCount} succeeded, {failCount} failed");
    }

    /// <summary>
    /// Save current level to local storage (Downloads on Android, Documents on Windows)
    /// </summary>
    public void SaveLevel()
    {
        string levelPath = UserLevelSet.GetUserLevelPath(currentLevelNumber);

        if (!File.Exists(levelPath))
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Level file not found at: {levelPath}");
            NotificationManager.ShowError(ErrorCode.FileNotFound, $"Level {currentLevelNumber + 1}");
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

            // Check if file already exists
            if (File.Exists(destinationPath))
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"Level {currentLevelNumber + 1} already exists in Downloads");
                NotificationManager.ShowError(ErrorCode.FileAlreadyExists, $"Level {currentLevelNumber + 1} in Downloads");
                return;
            }

            File.Copy(levelPath, destinationPath, overwrite: false);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level saved to Downloads: {destinationPath}");
            NotificationManager.ShowSuccess($"Level {currentLevelNumber + 1} saved to Downloads");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to save level to Downloads: {ex.Message}");
            NotificationManager.ShowError(ErrorCode.FileWriteFailed, $"Level {currentLevelNumber + 1}");
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

            // Check if file already exists
            if (File.Exists(destinationPath))
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"Level {currentLevelNumber + 1} already exists in Documents\\GetNuts");
                NotificationManager.ShowError(ErrorCode.FileAlreadyExists, $"Level {currentLevelNumber + 1} in Documents\\GetNuts");
                return;
            }

            File.Copy(levelPath, destinationPath, overwrite: false);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level copied to: {destinationPath}");

            // Open Explorer at the folder
            System.Diagnostics.Process.Start("explorer.exe", getNutsFolder);

            NotificationManager.ShowSuccess($"Level {currentLevelNumber + 1} saved to Documents\\GetNuts");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to copy level: {ex.Message}");
            NotificationManager.ShowError(ErrorCode.FileWriteFailed, $"Level {currentLevelNumber + 1}");
        }
    }

    #endregion

    #region Level Import Operations

    /// <summary>
    /// Import level from file picker
    /// </summary>
    public void ImportLevel()
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Starting import for level {currentLevelNumber + 1}");

        // Use SimpleFileBrowser for all platforms (cross-platform solution)
        SimpleFileBrowser.FileBrowser.SetFilters(true, new SimpleFileBrowser.FileBrowser.Filter("CSV Files", ".csv"));
        SimpleFileBrowser.FileBrowser.SetDefaultFilter(".csv");

        SimpleFileBrowser.FileBrowser.ShowLoadDialog(
            onSuccess: (paths) =>
            {
                if (paths != null && paths.Length > 0)
                {
                    DebugLogger.Log(DebugLogCategory.LevelSystem, $"File selected: {paths[0]}");
                    ImportLevelFromPath(paths[0]);
                }
                else
                {
                    DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "No file selected");
                }
            },
            onCancel: () =>
            {
                DebugLogger.Log(DebugLogCategory.LevelSystem, "File picker cancelled by user");
            },
            pickMode: SimpleFileBrowser.FileBrowser.PickMode.Files,
            allowMultiSelection: false,
            initialPath: null,
            initialFilename: null,
            title: "Select Level CSV",
            loadButtonText: "Import"
        );
    }


    /// <summary>
    /// Import level from specified file path with basic validation
    /// Handles both regular file paths and Android Content URIs
    /// </summary>
    private void ImportLevelFromPath(string sourcePath)
    {
        try
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Importing from path: {sourcePath}");

            string csvContent = null;

            // Check if it's an Android Content URI
            if (sourcePath.StartsWith("content://"))
            {
                DebugLogger.Log(DebugLogCategory.LevelSystem, "Detected Android Content URI, using SimpleFileBrowser to read");

                // Use SimpleFileBrowser's utility to read from Content URI
                byte[] fileBytes = SimpleFileBrowser.FileBrowserHelpers.ReadBytesFromFile(sourcePath);

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    NotificationManager.ShowError(ErrorCode.FileNotFound, "Could not read file");
                    DebugLogger.LogError(DebugLogCategory.LevelSystem, "Failed to read bytes from Content URI");
                    return;
                }

                csvContent = System.Text.Encoding.UTF8.GetString(fileBytes);
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Read {fileBytes.Length} bytes from Content URI");
            }
            else
            {
                DebugLogger.Log(DebugLogCategory.LevelSystem, "Using standard file system access");

                // Check if file exists (standard file path)
                if (!File.Exists(sourcePath))
                {
                    NotificationManager.ShowError(ErrorCode.FileNotFound, sourcePath);
                    DebugLogger.LogError(DebugLogCategory.LevelSystem, $"File not found: {sourcePath}");
                    return;
                }

                // Read CSV content (standard file path)
                csvContent = File.ReadAllText(sourcePath);
            }

            // Validate content
            if (string.IsNullOrEmpty(csvContent))
            {
                NotificationManager.ShowError(ErrorCode.InvalidLevelFormat, "File is empty");
                DebugLogger.LogError(DebugLogCategory.LevelSystem, "CSV content is empty");
                return;
            }

            // Check if it's a CSV file (by checking extension in path or content)
            if (!sourcePath.ToLower().Contains(".csv"))
            {
                NotificationManager.ShowError(ErrorCode.InvalidLevelFormat, "File must be .csv format");
                DebugLogger.LogError(DebugLogCategory.LevelSystem, "File is not CSV format");
                return;
            }

            // Copy to UserLevels folder - full validation happens when level loads in game
            string destinationPath = UserLevelSet.GetUserLevelPath(currentLevelNumber);
            File.WriteAllText(destinationPath, csvContent);

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level imported successfully to: {destinationPath}");
            NotificationManager.ShowSuccess($"Level {currentLevelNumber + 1} imported successfully");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to import level from {sourcePath}: {ex.Message}\nStack trace: {ex.StackTrace}");
            NotificationManager.ShowError(ErrorCode.FileReadFailed, $"Level {currentLevelNumber + 1}");
        }
    }

    #endregion
}
