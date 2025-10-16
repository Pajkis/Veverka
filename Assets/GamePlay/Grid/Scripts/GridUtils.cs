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
    /// Clear all cached grids
    /// </summary>
    public static void ClearCachedGrids()
    {
        CachedNutGrid = null;
        CachedGoalGrid = null;
        CachedObstacleGrid = null;
        CachedRoadGrid = null;
    }

    /// <summary>
    /// Set cached grids (called by TileParser after loading)
    /// </summary>
    public static void SetCachedGrids(NutType[,] nutGrid, GoalType[,] goalGrid, ObstacleType[,] obstacleGrid, RoadType[,] roadGrid)
    {
        CachedNutGrid = nutGrid;
        CachedGoalGrid = goalGrid;
        CachedObstacleGrid = obstacleGrid;
        CachedRoadGrid = roadGrid;
    }

    /// <summary>
    /// Validates the grid loaded from file
    /// Collects validation errors for display in LevelValidationErrorOverlay
    /// </summary>
    /// <param name="grid">loaded grid from file</param>
    /// <returns>grid is Valid <= true</returns>
    public static bool ValidateGrid(TileType[,] grid)
    {
        // Clear previous validation errors
        LevelValidationErrorManager.ClearErrors();

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
            string errorMsg = ErrorMessages.Get(ErrorCode.InvalidVeverkaCount, $"found {veverkaCount}");
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }

        // check nuts amount more than zero
        if (nutCount < 1)
        {
            string errorMsg = ErrorMessages.Get(ErrorCode.NoNutsFound, $"found {nutCount}");
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }

        // check goal amount more than zero
        if (goalCount < 1)
        {
            string errorMsg = ErrorMessages.Get(ErrorCode.NoGoalsFound, $"found {goalCount}");
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }

        // check same amount of nut and goals
        if (nutCount != goalCount)
        {
            string errorMsg = $"Nuts ({nutCount}) and Goals ({goalCount}) count mismatch!";
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            // Note: Not marked as validation error (valid = false commented out)
            // valid = false;
        }

        // Check for invalid tiles
        if (errorTileCount > 0)
        {
            string errorMsg = ErrorMessages.Get(ErrorCode.InvalidTileSymbols, $"{errorTileCount} error(s)");
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
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