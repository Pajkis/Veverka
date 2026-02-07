using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for handling all level sets
/// </summary>
[CreateAssetMenu(menuName = "Level/Config/Manager")]
public class LevelSetManager : ScriptableObject
{
    [Header("Available Level Sets")]
    [SerializeField] private List<LevelSet> levelSets = new List<LevelSet>();

    [Header("User Levels")]
    [SerializeField] private UserLevelSet userLevelSet;

    // Dictionary for faster lookups
    private Dictionary<LevelSetType, LevelSet> setLookup;

    /// <summary>
    /// Initialize the lookup dictionary
    /// </summary>
    private void OnEnable()
    {
        InitializeLookup();
    }

    /// <summary>
    /// Initialize the lookup dictionary for faster access
    /// </summary>
    private void InitializeLookup()
    {
        setLookup = new Dictionary<LevelSetType, LevelSet>();

        foreach (var levelSet in levelSets)
        {
            if (levelSet != null)
            {
                if (setLookup.ContainsKey(levelSet.setType))
                {
                    DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(LevelSetManager)}: Duplicate level set type found: {levelSet.setType}");
                    continue;
                }
                setLookup[levelSet.setType] = levelSet;
            }
        }
    }

    /// <summary>
    /// Get a specific level set by type
    /// </summary>
    /// <param name="setType">Type of the set to retrieve</param>
    /// <returns>LevelSet or null if not found</returns>
    public LevelSet GetLevelSet(LevelSetType setType)
    {
        // Handle UserLevels specially
        if (setType == LevelSetType.User)
        {
            if (userLevelSet == null)
            {
                DebugLogger.LogError(DebugLogCategory.LevelSystem, $"{nameof(LevelSetManager)}: UserLevelSet is not assigned!");
                return null;
            }

            // NOTE: User levels are initialized by UserLevelInitializer during GameInit
            // Do NOT initialize here to avoid creating levels with wrong/null DisplayConfig
            return userLevelSet;
        }

        // Ensure lookup is initialized
        if (setLookup == null)
        {
            InitializeLookup();
        }

        setLookup.TryGetValue(setType, out LevelSet levelSet);

        if (levelSet == null)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"{nameof(LevelSetManager)}: Level set of type {setType} not found!");
        }

        return levelSet;
    }

    /// <summary>
    /// Get CSV data for a specific level
    /// </summary>
    /// <param name="setType">Type of the level set</param>
    /// <param name="levelNumber">Level number within the set</param>
    /// <returns>TextAsset containing CSV data or null if not found</returns>
    public TextAsset GetLevelCsv(LevelSetType setType, int levelNumber)
    {
        var levelSet = GetLevelSet(setType);
        if (levelSet == null)
        {
            return null;
        }

        return levelSet.GetLevelCsv(levelNumber);
    }

    /// <summary>
    /// Check if a level exists
    /// </summary>
    /// <param name="setType">Type of the level set</param>
    /// <param name="levelNumber">Level number within the set</param>
    /// <returns>True if level exists</returns>
    public bool HasLevel(LevelSetType setType, int levelNumber)
    {
        var levelSet = GetLevelSet(setType);
        return levelSet != null && levelSet.HasLevel(levelNumber);
    }

    /// <summary>
    /// Get the total number of levels in a specific set
    /// </summary>
    /// <param name="setType">Type of the level set</param>
    /// <returns>Number of levels in the set, or 0 if set not found</returns>
    public int GetLevelCount(LevelSetType setType)
    {
        var levelSet = GetLevelSet(setType);
        return levelSet?.LevelCount ?? 0;
    }

    /// <summary>
    /// Get all available level sets
    /// </summary>
    /// <returns>List of all available level sets</returns>
    public List<LevelSet> GetAllLevelSets()
    {
        return new List<LevelSet>(levelSets);
    }
}