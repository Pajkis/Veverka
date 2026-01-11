using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for building and resetting game levels.
/// Handles grid initialization, object instantiation, and object pooling.
/// Gameplay/Grid/Scripts/GridBuilder
/// </summary>
public class GridBuilder : MonoBehaviour
{
    #region References
    [Header("Core References")]
    [SerializeField] private GridData gridData;
    [SerializeField] private Transform gridRoot;
    [SerializeField] private LevelSelection levelSelection;
    [SerializeField] private LevelInitData levelInitData;
    #endregion

    #region Events
    [Header("Events")]
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private LevelSelectEvent levelSelectEvent;
    #endregion

    #region Prefabs
    [Header("Tile Prefabs")]
    
    // road prefabs
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject stoneRoadPrefab;
    [SerializeField] private GameObject stoneFilledHolePrefab;
    [SerializeField] private GameObject roadGoldenGoalScoredPrefab;

    // character prefab
    [SerializeField] private GameObject veverkaPrefab;
    
    // nut prefabs
    [SerializeField] private GameObject nutPrefab;
    [SerializeField] private GameObject nutStonePrefab;
    [SerializeField] private GameObject nutWaterPrefab;
    [SerializeField] private GameObject nutGoldenPrefab;

    // obstacle prefabs
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallStonePrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private GameObject waterHolePrefab;
    [SerializeField] private GameObject goldenStatuePrefab;

    // goal prefab
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject goldenGoalPrefab;


    [Header("Other Prefabs")]
    [SerializeField] private GameObject backgroundPrefab;
    #endregion

    #region Object Pools
    private readonly List<GameObject> backgroundPool = new();
    private readonly List<GameObject> surroundingObstacles = new();
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        levelSelectEvent.AddListener(OnLevelSelected);
        gridEvents.AddListener(OnGridEvent);
    }

    private void OnDisable()
    {
        levelSelectEvent.RemoveListener(OnLevelSelected);
        gridEvents.RemoveListener(OnGridEvent);
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles grid-related events (BuildGrid, ResetGrid)
    /// </summary>
    private void OnGridEvent(GridEventPayload payload)
    {
        switch (payload.EventType)
        {
            case GridEventType.BuildGrid:
                if (!payload.BuildDone)
                {
                    BuildLevel(payload.Grid);
                }
                break;
            case GridEventType.ResetGrid:
                ResetLevel();
                break;
        }
    }

    /// <summary>
    /// Reset grid and raise again to load selected level
    /// </summary>
    private void OnLevelSelected(LevelSelectPayload payload)
    {
        if (payload.resetRequested)
        {
            ResetLevel();

            // Raise again for other listeners
            levelSelectEvent.Raise(new LevelSelectPayload
            {
                levelNumber = payload.levelNumber,
                resetRequested = false
            });

            DebugLogger.Log(DebugLogCategory.GridSystem, $"Grid reset complete - Starting level {levelSelection.CurrentLevelIndex}", this);
            return;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Build a new level from grid data
    /// </summary>
    public void BuildLevel(TileType[,] grid)
    {
        // Get grid size
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Initialize GridData
        gridData.InitializeGrids(width, height);

        // Copy grids from GridUtils cache to GridData
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new(x, y);
                gridData.SetTileType(pos, grid[x, y]);
                gridData.SetNutType(pos, GridUtils.CachedNutGrid[x, y]);
                gridData.SetGoalType(pos, GridUtils.CachedGoalGrid[x, y]);
                gridData.SetObstacleType(pos, GridUtils.CachedObstacleGrid[x, y]);
                gridData.SetRoadType(pos, GridUtils.CachedRoadGrid[x, y]);
            }
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Grid initialized - Size: {width}x{height}", this);

        // Spawn objects based on tile types
        DebugLogger.Log(DebugLogCategory.GridSystem, "Starting level build from grid data", this);
        BuildLevelFromGrid();

        // Add surrounding obstacles where grid does not fill the screen
        DebugLogger.Log(DebugLogCategory.GridSystem, "Building surrounding obstacles for screen fill", this);
        BuildSurroundings();

        // Initialize level init data
        levelInitData.gridSize = gridData.GridSize;
        int goalCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new(x, y);
                if (gridData.GetTileType(pos) == TileType.Goal) // && gridData.GetGoalType(pos) == GoalType.BasicGoal)
                {
                    goalCount++;
                }
            }
        }
        levelInitData.StartGoalsCount = goalCount;
        levelInitData.gridOrigin = transform;

        // Raise event to notify build completion
        gridEvents.Raise(new GridEventPayload { EventType = GridEventType.BuildGrid, BuildDone = true });
    }

    /// <summary>
    /// Reset current level and destroy all objects
    /// </summary>
    public void ResetLevel()
    {
        // Use GridSpawner helper to clear non-pooled objects
        GridSpawner.ClearNonPooledObjects(gridRoot, backgroundPool, surroundingObstacles);

        // Clear GridData
        gridData.ClearGrids();

        // Clear level init data
        if (levelInitData != null)
        {
            levelInitData.gridSize = Vector2Int.zero;
            levelInitData.StartGoalsCount = 0;
            levelInitData.gridCenterStartTarget = null;
            levelInitData.gridOrigin = null;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, "Level reset complete", this);
    }
    #endregion

    #region Private Build Methods

    /// <summary>
    /// Build a level from grid data using GridSpawner helper
    /// </summary>
    private void BuildLevelFromGrid()
    {
        // Create spawn configuration
        var config = new GridSpawner.SpawnConfig
        {
            // Road prefabs
            emptyPrefab = this.emptyPrefab,
            roadPrefab = this.roadPrefab,
            stoneRoadPrefab = this.stoneRoadPrefab,
            stoneFilledHolePrefab = this.stoneFilledHolePrefab,
            roadGoldenGoalScoredPrefab = this.roadGoldenGoalScoredPrefab,

            // Character prefab
            veverkaPrefab = this.veverkaPrefab,

            // Nut prefabs
            nutPrefab = this.nutPrefab,
            nutStonePrefab = this.nutStonePrefab,
            nutWaterPrefab = this.nutWaterPrefab,
            nutGoldenPrefab = this.nutGoldenPrefab,

            // Obstacle prefabs
            wallPrefab = this.wallPrefab,
            wallStonePrefab = this.wallStonePrefab,
            holePrefab = this.holePrefab,
            waterHolePrefab = this.waterHolePrefab,
            goldenStatuePrefab = this.goldenStatuePrefab,

            // Goal prefabs
            goalPrefab = this.goalPrefab,
            goldenGoalPrefab = this.goldenGoalPrefab,

            // Other prefabs
            backgroundPrefab = this.backgroundPrefab,

            // Context
            gridRoot = this.gridRoot,
            levelInitData = this.levelInitData,
            backgroundPool = this.backgroundPool,
            surroundingObstacles = this.surroundingObstacles
        };

        // Use shared helper to spawn grid
        GridSpawner.SpawnGrid(gridData, config);
    }

    /// <summary>
    /// Build surround environment for grids smaller than screen
    /// </summary>
    private void BuildSurroundings()
    {
        Vector2Int gridSize = gridData.GridSize;
        Vector2Int maxScreenGridSize = gridData.MaxScreenGridSize;

        // Calculate differences for each dimension
        int diffX = maxScreenGridSize.x - gridSize.x;
        int diffY = maxScreenGridSize.y - gridSize.y;

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Building surroundings - Grid: {gridSize.x}x{gridSize.y}, MaxScreen: {maxScreenGridSize.x}x{maxScreenGridSize.y}, Diff: {diffX}x{diffY}", this);

        // If both dimensions are larger than or equal to max screen size, do not build surroundings
        if (diffX <= 0 && diffY <= 0)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, "Grid fills or exceeds screen size - no surrounding obstacles needed", this);
            return;
        }

        // Calculate offsets - if dimension is larger than screen, offset should be 0
        int offsetLeft = diffX > 0 ? diffX / 2 : 0;
        int offsetRight = diffX > 0 ? diffX - offsetLeft : 0;
        int offsetDown = diffY > 0 ? diffY / 2 : 0;
        int offsetUp = diffY > 0 ? diffY - offsetDown : 0;

        // Calculate number of needed surrounding obstacles
        int widthWithOffset = gridSize.x + offsetLeft + offsetRight;
        int needed = 0;

        // Count obstacles needed for top/bottom (if height is smaller than screen)
        if (diffY > 0)
        {
            needed += widthWithOffset * (offsetDown + offsetUp);
        }

        // Count obstacles needed for left/right (if width is smaller than screen)
        if (diffX > 0)
        {
            needed += gridSize.y * (offsetLeft + offsetRight);
        }

        // If no obstacles are needed, return early
        if (needed == 0)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, "No surrounding obstacles needed", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Need {needed} surrounding obstacles - Offsets: Left:{offsetLeft}, Right:{offsetRight}, Down:{offsetDown}, Up:{offsetUp}", this);

        // If there are not enough obstacles in pool, instantiate new ones
        int currentObstacles = surroundingObstacles.Count;
        if (needed > currentObstacles)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Expanding obstacle pool from {currentObstacles} to {needed} obstacles", this);
        }

        for (int i = surroundingObstacles.Count; i < needed; i++)
        {
            var obstacle = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
            surroundingObstacles.Add(obstacle);
        }

        int index = 0;

        // Position surrounding obstacles on bottom (only if height is smaller than screen)
        if (diffY > 0)
        {
            for (int y = -offsetDown; y < 0; y++)
            {
                for (int x = -offsetLeft; x < gridSize.x + offsetRight; x++)
                {
                    PositionObstacle(index++, x, y);
                }
            }
        }

        // Position surrounding obstacles on top (only if height is smaller than screen)
        if (diffY > 0)
        {
            for (int y = gridSize.y; y < gridSize.y + offsetUp; y++)
            {
                for (int x = -offsetLeft; x < gridSize.x + offsetRight; x++)
                {
                    PositionObstacle(index++, x, y);
                }
            }
        }

        // Position surrounding obstacles on left (only if width is smaller than screen)
        if (diffX > 0)
        {
            for (int x = -offsetLeft; x < 0; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PositionObstacle(index++, x, y);
                }
            }
        }

        // Position surrounding obstacles on right (only if width is smaller than screen)
        if (diffX > 0)
        {
            for (int x = gridSize.x; x < gridSize.x + offsetRight; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PositionObstacle(index++, x, y);
                }
            }
        }

        // Deactivate remaining obstacles in pool
        int deactivatedObstacles = 0;
        for (; index < surroundingObstacles.Count; index++)
        {
            surroundingObstacles[index].SetActive(false);
            deactivatedObstacles++;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Surrounding obstacles complete - {index} obstacles positioned, {deactivatedObstacles} obstacles deactivated", this);
    }

    /// <summary>
    /// Positions an obstacle at the specified grid coordinates and activates it.
    /// </summary>
    private void PositionObstacle(int index, int x, int y)
    {
        var obstacle = surroundingObstacles[index];
        obstacle.SetActive(true);
        Vector3 worldTilePos = GridUtils.GridToWorld(new Vector2Int(x, y));
        obstacle.transform.SetParent(gridRoot, false);
        obstacle.transform.localPosition = worldTilePos;
    }
    #endregion
}
