using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single mapping entry for tile symbol to tile types
/// </summary>
[System.Serializable]
public class TileSymbolMapping
{
    [Tooltip("CSV symbol (e.g., 'W', 'NS', 'HW')")]
    public string symbol;

    [Header("Tile Types")]
    public TileType tileType;
    public NutType nutType = NutType.None;
    public GoalType goalType = GoalType.None;
    public ObstacleType obstacleType = ObstacleType.None;
    public RoadType roadType = RoadType.None;
}

/// <summary>
/// Configuration for tile parsing from CSV symbols to tile types.
/// Centralizes all tile symbol mappings in one place.
/// </summary>
[CreateAssetMenu(fileName = "TileParsingConfig", menuName = "Configs/TileParsingConfig")]
public class TileParsingConfig : ScriptableObject
{
    [Header("Tile Symbol Mappings")]
    [Tooltip("List of all tile symbol mappings used in CSV level files")]
    public List<TileSymbolMapping> tileMappings = new List<TileSymbolMapping>();

    // Dictionary cache for fast lookup
    private Dictionary<string, TileSymbolMapping> _mappingCache;

    /// <summary>
    /// Initialize the dictionary cache for fast symbol lookup
    /// </summary>
    public void Initialize()
    {
        _mappingCache = new Dictionary<string, TileSymbolMapping>();

        foreach (var mapping in tileMappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.symbol))
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "TileParsingConfig: Found mapping with empty symbol, skipping");
                continue;
            }

            if (_mappingCache.ContainsKey(mapping.symbol))
            {
                DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"TileParsingConfig: Duplicate symbol '{mapping.symbol}' found, using first occurrence");
                continue;
            }

            _mappingCache[mapping.symbol] = mapping;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"TileParsingConfig: Initialized with {_mappingCache.Count} tile mappings");
    }

    /// <summary>
    /// Get tile mapping for a given symbol
    /// </summary>
    /// <param name="symbol">CSV symbol to look up</param>
    /// <param name="mapping">Output mapping if found</param>
    /// <returns>True if mapping was found</returns>
    public bool TryGetMapping(string symbol, out TileSymbolMapping mapping)
    {
        if (_mappingCache == null)
        {
            Initialize();
        }

        return _mappingCache.TryGetValue(symbol, out mapping);
    }

    /// <summary>
    /// Validate the configuration in the Unity Editor
    /// </summary>
    private void OnValidate()
    {
        // Check for duplicate symbols
        HashSet<string> seenSymbols = new HashSet<string>();
        List<string> duplicates = new List<string>();

        foreach (var mapping in tileMappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.symbol))
                continue;

            if (seenSymbols.Contains(mapping.symbol))
            {
                duplicates.Add(mapping.symbol);
            }
            else
            {
                seenSymbols.Add(mapping.symbol);
            }
        }

        if (duplicates.Count > 0)
        {
            Debug.LogWarning($"TileParsingConfig: Found duplicate symbols: {string.Join(", ", duplicates)}");
        }
    }
}
