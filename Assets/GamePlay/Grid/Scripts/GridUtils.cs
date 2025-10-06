using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Grid utilities class
/// </summary>
public static class GridUtils
{
    /// <summary>
    /// loaded/validated grid for building the level
    /// </summary>
    public static TileType[,] CachedGrid { get; private set; }
    public static NutType[,] CachedNutGrid { get; private set; }
    public static GoalType[,] CachedGoalGrid { get; private set; }
    public static ObstacleType[,] CachedObstacleGrid { get; private set; }
    public static RoadType[,] CachedRoadGrid { get; private set; }


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
    /// Parse grid symbols into tile types.
    /// </summary>
    /// <param name="symbol">String representation of a tile.</param>
    /// <returns>Parsed tile data.</returns>
    public static ParsedTile ParseTileType(string symbol)
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
    /// Loads data from TextAsset into array
    /// </summary>
    /// <param name="levelData">TextAsset containing the CSV level data</param>
    /// <returns>2D array of TileType representing the level</returns>
    public static TileType[,] LoadGridFromTextAsset(TextAsset levelData)
    {
        try
        {
            if (levelData == null)
            {
                DebugLogger.LogError(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Level data is null!");
                return null;
            }

            // get size of the array
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
                DebugLogger.LogError(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: No valid data found in level file!");
                return null;
            }

            int height = validLines.Count;
            int width = validLines[0].Split(',').Length;

            // Clear cached grids
            CachedNutGrid = null;
            CachedGoalGrid = null;
            CachedObstacleGrid = null;
            CachedRoadGrid = null;

            TileType[,] levelGrid = new TileType[width, height];
            NutType[,] nutGrid = new NutType[width, height];
            GoalType[,] goalGrid = new GoalType[width, height];
            ObstacleType[,] obstacleGrid = new ObstacleType[width, height];
            RoadType[,] roadGrid = new RoadType[width, height];

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Loading level '{levelData.name}' with size (width, height): {width}, {height}");

            // put data into grid (flip Y axis to match Unity's coordinate system)
            for (int y = 0; y < height; y++)
            {
                string[] row = validLines[height - 1 - y].Split(",");

                if (row.Length != width)
                {
                    DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Row {y} has {row.Length} columns, expected {width}. Level: {levelData.name}");
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
                        DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Error tile at position [{x},{y}] in level '{levelData.name}'. Symbol: '{row[x].Trim()}'");
                    }
                }
            }

            // Cache the grids
            CachedNutGrid = nutGrid;
            CachedGoalGrid = goalGrid;
            CachedObstacleGrid = obstacleGrid;
            CachedRoadGrid = roadGrid;

            return levelGrid;
        }
        catch (Exception ex)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Error loading level from TextAsset '{levelData?.name}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Validates the grid loaded from file
    /// </summary>
    /// <param name="grid">loaded grid from file</param>
    /// <returns>grid is Valid <= true</returns>
    public static bool ValidateGrid(TileType[,] grid)
    {
        int veverkaCount = 0;
        int nutCount = 0;
        int goalCount = 0;
        int errorTileCount = 0;
        bool valid = true;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                switch (grid[x, y])
                {
                    case TileType.Veverka: veverkaCount++; break;
                    case TileType.Nut: nutCount++; break;
                    case TileType.Goal: goalCount++; break;
                    case TileType.ErrorTile: errorTileCount++; break;
                }
            }
        }

        // check veverka count
        if (veverkaCount != 1)
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Expected 1 veverka but found {veverkaCount}");
            valid = false;
        }

        // check nuts amount more than zero
        if (nutCount < 1)
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Expected at least 1 nut but found {nutCount}");
            valid = false;
        }

        // check goal amount more than zero
        if (goalCount < 1)
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Expected at least 1 goal but found {goalCount}");
            valid = false;
        }

        // check same amount of nut and goals
        if (nutCount != goalCount)
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: The amount of goals and nuts does not match! Found goals: {goalCount}, found nuts {nutCount}");
            // valid =  false;
        }

        // Check for invalid tiles
        if (errorTileCount > 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: Invalid tiles in input CSV file: Count {errorTileCount}");
            valid = false;
        }

        //Set up grid
        CachedGrid = null;
        CachedGrid = grid;
        return valid;
    }

    /// <summary>
    /// Gets position of adjacent (or further - based on distance) tile based on direction
    /// </summary>
    /// <param name="currentPosition">current position of object</param>
    /// <param name="direction">In which direction is the position checked</param>
    /// <param name="distance">Amount tiles in the direction</param>
    /// <returns>position of adjacent tile</returns>
    public static Vector2Int GetPositionInDir(Vector2Int currentPosition, Direction direction, int distance = 1)
    {
        if (distance < 1) distance = 1;

        switch (direction)
        {
            case Direction.Up:
                return currentPosition + Vector2Int.up * distance;
            case Direction.Down:
                return currentPosition + Vector2Int.down * distance;
            case Direction.Left:
                return currentPosition + Vector2Int.left * distance;
            case Direction.Right:
                return currentPosition + Vector2Int.right * distance;
            default:
                DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"{nameof(GridUtils)}: Unknown direction {direction}, returning current position");
                return currentPosition;
        }
    }

    /// <summary>
    /// Converts a grid position to a world space position.
    /// </summary>
    /// <param name="gridPosition">Tile grid position (x, y)</param>
    /// <returns>World space position (Vector3)</returns>
    public static Vector3 GridToWorld(Vector2Int gridPosition, float tileSize = 1f)
    {
        float offset = 0f;
        //float halfTile = tileSize / 2f;
        return new Vector3(gridPosition.x * tileSize + offset,
                           gridPosition.y * tileSize + offset,
                           0);
    }
}