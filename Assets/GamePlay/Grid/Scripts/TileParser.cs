using System;
using UnityEngine;

/// <summary>
/// Parser structure used when parsing level data.
/// </summary>
public struct ParsedTile
{
    public TileType TileType;
    public GoalType GoalType;
    public ObstacleType ObstacleType;
    public NutType NutType;
    public RoadType RoadType;
}

/// <summary>
/// Static utility class for parsing tile symbols from CSV files into tile types.
/// Uses TileParsingConfig for symbol-to-type mappings.
/// </summary>
public static class TileParser
{
    private static TileParsingConfig _config;

    /// <summary>
    /// Set the parsing configuration to use
    /// </summary>
    /// <param name="config">TileParsingConfig ScriptableObject</param>
    public static void SetConfig(TileParsingConfig config)
    {
        _config = config;
        if (_config != null)
        {
            _config.Initialize();
        }
    }

    /// <summary>
    /// Parse grid symbols into tile types using the configured mappings.
    /// Falls back to default mappings if no config is set.
    /// </summary>
    /// <param name="symbol">String representation of a tile from CSV</param>
    /// <returns>Parsed tile data</returns>
    public static ParsedTile ParseTileType(string symbol)
    {
        // Try to use config first
        if (_config != null && _config.TryGetMapping(symbol, out var mapping))
        {
            return new ParsedTile
            {
                TileType = mapping.tileType,
                GoalType = mapping.goalType,
                ObstacleType = mapping.obstacleType,
                NutType = mapping.nutType,
                RoadType = mapping.roadType
            };
        }

        // Fallback to hardcoded defaults if config is not available
        return ParseTileTypeFallback(symbol);
    }

    /// <summary>
    /// Fallback parsing when no config is available (uses original switch logic)
    /// </summary>
    private static ParsedTile ParseTileTypeFallback(string symbol)
    {
        var result = new ParsedTile
        {
            TileType = TileType.ErrorTile,
            GoalType = GoalType.BasicGoal,
            ObstacleType = ObstacleType.BasicWall,
            NutType = NutType.BasicNut,
            RoadType = RoadType.BasicRoad,
        };

        switch (symbol)
        {
            case "W":
                result.TileType = TileType.Obstacle;
                result.ObstacleType = ObstacleType.BasicWall;
                break;
            case "WS":
                result.TileType = TileType.Obstacle;
                result.ObstacleType = ObstacleType.StoneWall;
                break;
            case "V":
                result.TileType = TileType.Veverka;
                break;
            case "N":
                result.TileType = TileType.Nut;
                result.NutType = NutType.BasicNut;
                break;
            case "NS":
                result.TileType = TileType.Nut;
                result.NutType = NutType.StoneNut;
                break;
            case "NW":
                result.TileType = TileType.Nut;
                result.NutType = NutType.WaterNut;
                break;
            case "G":
                result.TileType = TileType.Goal;
                result.GoalType = GoalType.BasicGoal;
                break;
            case "H":
                result.TileType = TileType.Obstacle;
                result.ObstacleType = ObstacleType.Hole;
                break;
            case "HW":
                result.TileType = TileType.Obstacle;
                result.ObstacleType = ObstacleType.WaterHole;
                break;
            case "R":
                result.TileType = TileType.Road;
                result.RoadType = RoadType.BasicRoad;
                break;
            case "RS":
                result.TileType = TileType.Road;
                result.RoadType = RoadType.StoneRoad;
                break;
            case "RSF":
                result.TileType = TileType.Road;
                result.RoadType = RoadType.StoneFilledHole;
                break;
            case ".":
                result.TileType = TileType.Road;
                result.RoadType = RoadType.Empty;
                break;
            default:
                result.TileType = TileType.ErrorTile;
                break;
        }

        return result;
    }

    /// <summary>
    /// Loads data from TextAsset into arrays and caches them in GridUtils
    /// </summary>
    /// <param name="levelData">TextAsset containing the CSV level data</param>
    /// <returns>2D array of TileType representing the level</returns>
    public static TileType[,] LoadGridFromTextAsset(TextAsset levelData)
    {
        try
        {
            if (levelData == null)
            {
                DebugLogger.LogError(DebugLogCategory.LevelSystem, "TileParser: Level data is null!");
                return null;
            }

            // Get size of the array
            string[] lines = levelData.text.Split('\n');

            // Remove empty lines
            var validLines = new System.Collections.Generic.List<string>();
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    validLines.Add(line.Trim());
                }
            }

            if (validLines.Count == 0)
            {
                DebugLogger.LogError(DebugLogCategory.LevelSystem, "TileParser: No valid data found in level file!");
                return null;
            }

            int height = validLines.Count;
            int width = validLines[0].Split(',').Length;

            // Clear cached grids in GridUtils
            GridUtils.ClearCachedGrids();

            TileType[,] levelGrid = new TileType[width, height];
            NutType[,] nutGrid = new NutType[width, height];
            GoalType[,] goalGrid = new GoalType[width, height];
            ObstacleType[,] obstacleGrid = new ObstacleType[width, height];
            RoadType[,] roadGrid = new RoadType[width, height];

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"TileParser: Loading level '{levelData.name}' with size (width, height): {width}, {height}");

            // Put data into grid (flip Y axis to match Unity's coordinate system)
            for (int y = 0; y < height; y++)
            {
                string[] row = validLines[height - 1 - y].Split(",");

                if (row.Length != width)
                {
                    DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"TileParser: Row {y} has {row.Length} columns, expected {width}. Level: {levelData.name}");
                }

                for (int x = 0; x < width && x < row.Length; x++)
                {
                    ParsedTile parsed = ParseTileType(row[x].Trim());
                    levelGrid[x, y] = parsed.TileType;
                    nutGrid[x, y] = parsed.NutType;
                    goalGrid[x, y] = parsed.GoalType;
                    obstacleGrid[x, y] = parsed.ObstacleType;
                    roadGrid[x, y] = parsed.RoadType;

                    // Catch error types
                    if (parsed.TileType == TileType.ErrorTile)
                    {
                        DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"TileParser: Error tile at position [{x},{y}] in level '{levelData.name}'. Symbol: '{row[x].Trim()}'");
                    }
                }
            }

            // Cache the grids in GridUtils
            GridUtils.SetCachedGrids(nutGrid, goalGrid, obstacleGrid, roadGrid);

            return levelGrid;
        }
        catch (Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"TileParser: Error loading level from TextAsset '{levelData?.name}': {ex.Message}");
            return null;
        }
    }
}
