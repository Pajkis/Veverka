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
    public static void InitializeUserLevels(DisplayConfig displayConfig)
    {
        string userLevelsPath = GetUserLevelsPath();

        // Create directory if it doesn't exist
        if (!Directory.Exists(userLevelsPath))
        {
            Directory.CreateDirectory(userLevelsPath);
            Debug.Log($"Created user levels directory at: {userLevelsPath}");
        }

        // Get grid size from DisplayConfig
        Vector2Int gridSize = new Vector2Int(15, 11); // Fallback
        if (displayConfig != null)
        {
            var profile = displayConfig.ResolveProfile();
            if (profile != null)
            {
                gridSize = profile.maxStaticScreenSize;
            }
        }

        // Create empty CSV files for each level if they don't exist
        for (int i = 0; i < USER_LEVEL_COUNT; i++)
        {
            string levelPath = GetUserLevelPath(i);
            if (!File.Exists(levelPath))
            {
                string emptyGrid = CreateEmptyGridCSV(gridSize.x, gridSize.y);
                File.WriteAllText(levelPath, emptyGrid);
                Debug.Log($"Created empty user level at: {levelPath}");
            }
        }
    }

    /// <summary>
    /// Create an empty grid CSV string
    /// </summary>
    private static string CreateEmptyGridCSV(int width, int height)
    {
        string csv = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                csv += "."; // . = Empty road
                if (x < width - 1)
                {
                    csv += ",";
                }
            }
            csv += "\n";
        }
        return csv;
    }

    /// <summary>
    /// Load user level from persistent storage
    /// </summary>
    public override TextAsset GetLevelCsv(int levelNumber)
    {
        if (levelNumber < 0 || levelNumber >= USER_LEVEL_COUNT)
        {
            Debug.LogError($"User level {levelNumber} not found. Available levels: 0-{USER_LEVEL_COUNT - 1}");
            return null;
        }

        string levelPath = GetUserLevelPath(levelNumber);

        if (!File.Exists(levelPath))
        {
            Debug.LogError($"User level file not found at: {levelPath}");
            return null;
        }

        // Read CSV from persistent storage
        string csvContent = File.ReadAllText(levelPath);

        // Create a TextAsset from the content
        TextAsset textAsset = new TextAsset(csvContent);
        textAsset.name = $"UserLevel{levelNumber + 1}";

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
            return false;
        }

        string levelPath = GetUserLevelPath(levelNumber);
        return File.Exists(levelPath);
    }
}
