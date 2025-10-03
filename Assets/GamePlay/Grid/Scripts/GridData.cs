using UnityEngine;

/// <summary>
/// Central data storage for game grid state.
/// Holds all grid arrays and provides query/validation methods.
/// Gameplay/Grid/Scripts/GridData
/// </summary>
public class GridData : MonoBehaviour
{
    #region Fields
    // Grid arrays - current state
    private TileType[,] grid;
    private NutType[,] nutGrid;
    private GoalType[,] goalGrid;
    private ObstacleType[,] obstacleGrid;
    private RoadType[,] roadGrid;

    // Grid dimensions
    private Vector2Int gridSize;
    private readonly Vector2Int maxScreenGridSize = new(15, 11);
    #endregion

    #region Properties
    /// <summary>
    /// Current size of the active grid
    /// </summary>
    public Vector2Int GridSize => gridSize;

    /// <summary>
    /// Maximum grid size that fits on screen
    /// </summary>
    public Vector2Int MaxScreenGridSize => maxScreenGridSize;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialize all grid arrays with given size
    /// </summary>
    public void InitializeGrids(int width, int height)
    {
        gridSize = new Vector2Int(width, height);
        grid = new TileType[width, height];
        nutGrid = new NutType[width, height];
        goalGrid = new GoalType[width, height];
        obstacleGrid = new ObstacleType[width, height];
        roadGrid = new RoadType[width, height];

        DebugLogger.Log(DebugLogCategory.GridSystem, $"GridData initialized - Size: {width}x{height}", this);
    }

    /// <summary>
    /// Clear all grid data
    /// </summary>
    public void ClearGrids()
    {
        grid = null;
        nutGrid = null;
        goalGrid = null;
        obstacleGrid = null;
        roadGrid = null;
        gridSize = Vector2Int.zero;

        DebugLogger.Log(DebugLogCategory.GridSystem, "GridData cleared", this);
    }
    #endregion

    #region Getters - Read-only access
    /// <summary>
    /// Get tile type at position
    /// </summary>
    public TileType GetTileType(Vector2Int position)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"GetTileType: Position {position} is outside grid bounds", this);
            return TileType.Empty;
        }
        return grid[position.x, position.y];
    }

    /// <summary>
    /// Get nut type at position
    /// </summary>
    public NutType GetNutType(Vector2Int position)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"GetNutType: Position {position} is outside grid bounds", this);
            return NutType.None;
        }
        return nutGrid[position.x, position.y];
    }

    /// <summary>
    /// Get goal type at position
    /// </summary>
    public GoalType GetGoalType(Vector2Int position)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"GetGoalType: Position {position} is outside grid bounds", this);
            return GoalType.None;
        }
        return goalGrid[position.x, position.y];
    }

    /// <summary>
    /// Get wall type at position
    /// </summary>
    public ObstacleType GetObstacleType(Vector2Int position)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"GetObstacleType: Position {position} is outside grid bounds", this);
            return ObstacleType.None;
        }
        return obstacleGrid[position.x, position.y];
    }

    /// <summary>
    /// Get road type at position
    /// </summary>
    public RoadType GetRoadType(Vector2Int position)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"GetRoadType: Position {position} is outside grid bounds", this);
            return RoadType.None;
        }
        return roadGrid[position.x, position.y];
    }
    #endregion

    #region Setters - Update grid state
    /// <summary>
    /// Set tile type at position
    /// </summary>
    public void SetTileType(Vector2Int position, TileType tileType)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"SetTileType: Position {position} is outside grid bounds", this);
            return;
        }
        grid[position.x, position.y] = tileType;
    }

    /// <summary>
    /// Set nut type at position
    /// </summary>
    public void SetNutType(Vector2Int position, NutType nutType)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"SetNutType: Position {position} is outside grid bounds", this);
            return;
        }
        nutGrid[position.x, position.y] = nutType;
    }

    /// <summary>
    /// Set goal type at position
    /// </summary>
    public void SetGoalType(Vector2Int position, GoalType goalType)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"SetGoalType: Position {position} is outside grid bounds", this);
            return;
        }
        goalGrid[position.x, position.y] = goalType;
    }

    /// <summary>
    /// Set wall type at position
    /// </summary>
    public void SetObstacleType(Vector2Int position, ObstacleType obstacleType)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"SetObstacleType: Position {position} is outside grid bounds", this);
            return;
        }
        obstacleGrid[position.x, position.y] = obstacleType;
    }

    /// <summary>
    /// Set road type at position
    /// </summary>
    public void SetRoadType(Vector2Int position, RoadType roadType)
    {
        if (!IsInGrid(position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"SetRoadType: Position {position} is outside grid bounds", this);
            return;
        }
        roadGrid[position.x, position.y] = roadType;
    }
    #endregion

    #region Validation & Query Methods
    /// <summary>
    /// Check whether position is within grid bounds
    /// </summary>
    public bool IsInGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSize.x
            && position.y >= 0 && position.y < gridSize.y;
    }

    /// <summary>
    /// Checks whether a tile at position can be walked on by a character.
    /// </summary>
    public bool IsWalkableAt(Vector2Int position)
    {
        if (!IsInGrid(position))
            return false;

        var tile = grid[position.x, position.y];
        if (tile == TileType.Goal)
        {
            // Characters can walk on BasicGoals but not holes
            var obstacleType = obstacleGrid[position.x, position.y];
            return obstacleType != ObstacleType.Hole && obstacleType != ObstacleType.WaterHole;
        }
        return tile == TileType.Empty || tile == TileType.Road;
    }

    /// <summary>
    /// Checks whether a tile at position can be entered by a nut.
    /// </summary>
    public bool IsPushableAt(Vector2Int position)
    {
        if (!IsInGrid(position))
            return false;

        var tile = grid[position.x, position.y];

        // Basic pushable types
        if (tile == TileType.Empty || tile == TileType.Road || tile == TileType.Goal)
            return true;

        // Obstacles are pushable only if they are Hole or WaterHole
        if (tile == TileType.Obstacle)
        {
            var obstacleType = obstacleGrid[position.x, position.y];
            return obstacleType == ObstacleType.Hole || obstacleType == ObstacleType.WaterHole;
        }

        return false;
    }
    #endregion
}
