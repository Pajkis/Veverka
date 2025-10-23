using UnityEngine;
using System.IO;

/// <summary>
/// Special LevelSet for user-created levels stored in persistent storage
/// </summary>
[CreateAssetMenu(menuName = "Data/UserLevelSet")]
public class UserLevelSet : LevelSet
{
    private const int USER_LEVEL_COUNT = 10;
    private const string USER_LEVELS_FOLDER = "UserLevels";
    private const string MANUAL_FILE_NAME = "UserLevelManual.txt";

    private TextAsset[] cachedLevels;

    /// <summary>
    /// Get the path to user levels folder in persistent storage
    /// </summary>
    public static string GetUserLevelsPath()
    {
        return Path.Combine(Application.persistentDataPath, USER_LEVELS_FOLDER);
    }

    /// <summary>
    /// Get the path to a specific user level CSV file
    /// </summary>
    public static string GetUserLevelPath(int levelNumber)
    {
        return Path.Combine(GetUserLevelsPath(), $"UserLevel{levelNumber + 1}.csv");
    }

    /// <summary>
    /// Initialize user levels folder and create empty CSV files if they don't exist
    /// </summary>
    /// <param name="displayConfig">Display configuration for grid size</param>
    /// <param name="manualTextAsset">Optional manual text asset to copy to user levels folder</param>
    public static void InitializeUserLevels(DisplayConfig displayConfig, TextAsset manualTextAsset = null)
    {
        string userLevelsPath = GetUserLevelsPath();

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"===== INITIALIZE USER LEVELS CALLED =====");
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Initializing user levels at: {userLevelsPath}");
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"DisplayConfig provided: {(displayConfig == null ? "NULL" : "NOT NULL")}");
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Manual provided: {(manualTextAsset == null ? "NULL" : "NOT NULL")}");
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"STACK TRACE:\n{UnityEngine.StackTraceUtility.ExtractStackTrace()}");

        // Create directory if it doesn't exist
        if (!Directory.Exists(userLevelsPath))
        {
            Directory.CreateDirectory(userLevelsPath);
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Created user levels directory at: {userLevelsPath}");
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "User levels directory already exists");
        }

        // Get grid size from DisplayConfig
        Vector2Int gridSize = new Vector2Int(15, 11); // Fallback
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"DisplayConfig is null: {displayConfig == null}");

        if (displayConfig != null)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "DisplayConfig is not null, attempting to resolve profile");
            var profile = displayConfig.ResolveProfile();
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"ResolveProfile returned: {(profile == null ? "NULL" : "valid profile")}");

            if (profile != null)
            {
                gridSize = profile.maxStaticScreenSize;
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Using grid size from DisplayConfig: {gridSize.x}x{gridSize.y}");
            }
            else
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "DisplayConfig profile is null, using fallback grid size 15x11");
            }
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "DisplayConfig is null, using fallback grid size 15x11");
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Final grid size for user level initialization: {gridSize.x}x{gridSize.y}");

        // Create empty CSV files for each level if they don't exist
        int createdCount = 0;
        for (int i = 0; i < USER_LEVEL_COUNT; i++)
        {
            string levelPath = GetUserLevelPath(i);
            if (!File.Exists(levelPath))
            {
                string emptyGrid = CreateEmptyGridCSV(gridSize.x, gridSize.y);
                File.WriteAllText(levelPath, emptyGrid);
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Created empty user level {i + 1} at: {levelPath}");
                createdCount++;
            }
        }

        if (createdCount == 0)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "All user level files already exist, no new files created");
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"User levels initialization complete - created {createdCount} new level(s)");
        }

        // Copy UserLevelManual.txt to UserLevels folder if provided
        if (manualTextAsset != null)
        {
            CopyManualToUserLevelsFolder(userLevelsPath, manualTextAsset);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "Manual TextAsset not provided - skipping manual copy");
        }
    }

    /// <summary>
    /// Copy UserLevelManual from TextAsset to UserLevels folder if it doesn't exist
    /// </summary>
    private static void CopyManualToUserLevelsFolder(string userLevelsPath, TextAsset manualTextAsset)
    {
        string destinationPath = Path.Combine(userLevelsPath, MANUAL_FILE_NAME);

        // Only copy if manual doesn't exist in UserLevels folder
        if (File.Exists(destinationPath))
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "UserLevelManual.txt already exists in UserLevels folder");
            return;
        }

        // Copy manual from TextAsset
        try
        {
            File.WriteAllText(destinationPath, manualTextAsset.text);
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Copied UserLevelManual.txt to: {destinationPath}");
        }
        catch (System.Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to copy UserLevelManual.txt: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a minimal playable grid CSV string with required elements
    /// </summary>
    private static string CreateEmptyGridCSV(int width, int height)
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"CreateEmptyGridCSV called with width={width}, height={height}");

        string csv = "";
        int vCount = 0, nCount = 0, gCount = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Place required elements at top-left corner to make level playable
                // Position (0,0) = Veverka, (0,1) = Nut, (0,2) = Goal
                if (x == 0 && y == 0)
                {
                    csv += "V"; // Veverka at position (0,0)
                    vCount++;
                }
                else if (x == 0 && y == 1)
                {
                    csv += "N"; // Nut at position (0,1)
                    nCount++;
                }
                else if (x == 0 && y == 2)
                {
                    csv += "G"; // Goal at position (0,2)
                    gCount++;
                }
                else
                {
                    csv += "."; // Empty road everywhere else
                }

                if (x < width - 1)
                {
                    csv += ",";
                }
            }
            csv += "\n";
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"CreateEmptyGridCSV generated CSV with V={vCount}, N={nCount}, G={gCount}, total length={csv.Length}");

        // Log first few lines for debugging
        string[] lines = csv.Split('\n');
        if (lines.Length >= 3)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"First 3 lines:\n  Line 0: {lines[0]}\n  Line 1: {lines[1]}\n  Line 2: {lines[2]}");
        }

        return csv;
    }

    /// <summary>
    /// Load user level from persistent storage
    /// </summary>
    public override TextAsset GetLevelCsv(int levelNumber)
    {
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Attempting to load user level {levelNumber}");

        if (levelNumber < 0 || levelNumber >= USER_LEVEL_COUNT)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"User level {levelNumber} out of range. Available levels: 0-{USER_LEVEL_COUNT - 1}");
            return null;
        }

        string levelPath = GetUserLevelPath(levelNumber);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User level path: {levelPath}");

        if (!File.Exists(levelPath))
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"User level file not found at: {levelPath}");
            return null;
        }

        // Read CSV from persistent storage
        string csvContent = File.ReadAllText(levelPath);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Successfully read user level {levelNumber} - {csvContent.Length} characters");

        // Create a TextAsset from the content
        TextAsset textAsset = new TextAsset(csvContent);
        textAsset.name = $"UserLevel{levelNumber + 1}";

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User level {levelNumber} loaded successfully as '{textAsset.name}'");
        return textAsset;
    }

    /// <summary>
    /// Get the total number of user levels
    /// </summary>
    public override int LevelCount => USER_LEVEL_COUNT;

    /// <summary>
    /// Check if a user level number exists
    /// </summary>
    public override bool HasLevel(int levelNumber)
    {
        if (levelNumber < 0 || levelNumber >= USER_LEVEL_COUNT)
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"HasLevel check failed: User level {levelNumber} out of range (0-{USER_LEVEL_COUNT - 1})");
            return false;
        }

        string levelPath = GetUserLevelPath(levelNumber);
        bool exists = File.Exists(levelPath);

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"User level {levelNumber} exists check: {exists} at {levelPath}");
        return exists;
    }
}
