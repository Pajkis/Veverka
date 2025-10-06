using UnityEngine;

/// <summary>
/// ScriptableObject representing a set of levels (e.g., Stone set, Basic set)
/// </summary>
[CreateAssetMenu(menuName = "Data/LevelSet")]
public class LevelSet : ScriptableObject
{
    [Header("Set Information")]
    public LevelSetType setType;
    public string setDisplayName;

    [Header("Level CSV Files")]
    [SerializeField] private TextAsset[] levelCsvFiles;

    /// <summary>
    /// Get the CSV file for a specific level number
    /// </summary>
    /// <param name="levelNumber">Level number (0-based index)</param>
    /// <returns>TextAsset containing the CSV data, or null if not found</returns>
    public virtual TextAsset GetLevelCsv(int levelNumber)
    {
        if (levelNumber < 0 || levelNumber >= levelCsvFiles.Length)
        {
            DebugLogger.LogError(DebugLogCategory.LevelLoading, $"LevelSet: Level {levelNumber} not found in set {setType}. Available levels: 0-{levelCsvFiles.Length - 1}");
            return null;
        }

        return levelCsvFiles[levelNumber];
    }

    /// <summary>
    /// Get the total number of levels in this set
    /// </summary>
    public virtual int LevelCount => levelCsvFiles.Length;

    /// <summary>
    /// Check if a level number exists in this set
    /// </summary>
    /// <param name="levelNumber">Level number to check</param>
    /// <returns>True if level exists</returns>
    public virtual bool HasLevel(int levelNumber)
    {
        return levelNumber >= 0 && levelNumber < levelCsvFiles.Length;
    }
}