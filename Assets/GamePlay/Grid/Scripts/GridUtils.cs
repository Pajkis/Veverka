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
    /// Raw symbol grid from CSV - used for validation error messages
    /// </summary>
    public static string[,] CachedValidationGrid { get; private set; }

    /// <summary>
    /// Row lengths from CSV - used for validation (detecting unequal row lengths)
    /// </summary>
    public static int[] CachedRowLengths { get; private set; }


    /// <summary>
    /// Clear all cached grids
    /// </summary>
    public static void ClearCachedGrids()
    {
        CachedNutGrid = null;
        CachedGoalGrid = null;
        CachedObstacleGrid = null;
        CachedRoadGrid = null;
        CachedValidationGrid = null;
        CachedRowLengths = null;
    }

    /// <summary>
    /// Set cached grids (called by TileParser after loading)
    /// </summary>
    public static void SetCachedGrids(NutType[,] nutGrid, GoalType[,] goalGrid, ObstacleType[,] obstacleGrid, RoadType[,] roadGrid, string[,] validationGrid, int[] rowLengths)
    {
        CachedNutGrid = nutGrid;
        CachedGoalGrid = goalGrid;
        CachedObstacleGrid = obstacleGrid;
        CachedRoadGrid = roadGrid;
        CachedValidationGrid = validationGrid;
        CachedRowLengths = rowLengths;
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
        bool valid = true;

        var veverkaPositions = new System.Collections.Generic.List<Vector2Int>();

        // Check row lengths - first row is used as reference
        // This must be checked FIRST as mismatched rows cause misleading errors
        if (CachedRowLengths != null && CachedRowLengths.Length > 0)
        {
            int expectedWidth = CachedRowLengths[0]; // First row determines expected width
            bool hasRowLengthErrors = false;

            for (int y = 0; y < CachedRowLengths.Length; y++)
            {
                if (CachedRowLengths[y] != expectedWidth)
                {
                    string errorMsg = ErrorMessages.Get(
                        ErrorCode.RowLengthMismatch,
                        $"Row {y}: expected {expectedWidth}, found {CachedRowLengths[y]} (Note: First row used as reference)"
                    );
                    DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
                    LevelValidationErrorManager.AddError(errorMsg);
                    hasRowLengthErrors = true;
                }
            }

            // If row lengths don't match, stop validation here
            // Other errors (empty tiles, etc.) would be misleading
            if (hasRowLengthErrors)
            {
                CachedGrid = null;
                CachedGrid = grid;
                return false;
            }
        }

        // First pass - count tiles and collect positions
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                switch (grid[x, y])
                {
                    case TileType.Veverka:
                        veverkaCount++;
                        veverkaPositions.Add(new Vector2Int(x, y));
                        break;
                    case TileType.Nut:
                        nutCount++;
                        break;
                    case TileType.Goal:
                        goalCount++;
                        break;
                    case TileType.ErrorTile:
                        // Handle error tiles individually with positions
                        if (CachedValidationGrid != null)
                        {
                            string symbol = CachedValidationGrid[x, y];
                            string errorMsg;

                            if (string.IsNullOrWhiteSpace(symbol))
                            {
                                errorMsg = ErrorMessages.Get(ErrorCode.EmptyTileCell, $"at ({x}, {y})");
                            }
                            else
                            {
                                errorMsg = ErrorMessages.Get(ErrorCode.InvalidTileSymbol, $"at ({x}, {y}): \"{symbol}\"");
                            }

                            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
                            LevelValidationErrorManager.AddError(errorMsg);
                            valid = false;
                        }
                        break;
                }
            }
        }

        // Check veverka count
        if (veverkaCount == 0)
        {
            string errorMsg = ErrorMessages.Get(ErrorCode.NoVeverkaFound);
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }
        else if (veverkaCount > 1)
        {
            // Multiple veverkas - list positions
            string positions = string.Join(", ", veverkaPositions.ConvertAll(p => $"({p.x}, {p.y})"));
            string errorMsg = ErrorMessages.Get(ErrorCode.MultipleVeverkas, $"at {positions}");
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }

        // Check nuts amount more than zero
        if (nutCount < 1)
        {
            string errorMsg = ErrorMessages.Get(ErrorCode.NoNutsFound);
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }

        // Check goal amount more than zero
        if (goalCount < 1)
        {
            string errorMsg = ErrorMessages.Get(ErrorCode.NoGoalsFound);
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            LevelValidationErrorManager.AddError(errorMsg);
            valid = false;
        }

        // Check same amount of nut and goals
        if (nutCount != goalCount)
        {
            string errorMsg = $"Nuts ({nutCount}) and Goals ({goalCount}) count mismatch!";
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"{nameof(GridUtils)}: {errorMsg}");
            // Note: Not marked as validation error (valid = false commented out)
            // valid = false;
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