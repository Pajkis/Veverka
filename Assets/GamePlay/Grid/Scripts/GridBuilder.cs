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
    [SerializeField] private LevelDatabase levelDatabase;
    #endregion

    #region Events
    [Header("Events")]
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private LevelSelectEvent levelSelectEvent;
    #endregion

    #region Prefabs
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject stoneFilledHolePrefab;
    [SerializeField] private GameObject veverkaPrefab;
    [SerializeField] private GameObject nutPrefab;
    [SerializeField] private GameObject nutStonePrefab;
    [SerializeField] private GameObject nutWaterPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallStonePrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private GameObject waterHolePrefab;
    [SerializeField] private GameObject goalPrefab;

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

            DebugLogger.Log(DebugLogCategory.GridSystem, $"Grid reset complete - Starting level {levelDatabase.CurrentLevelIndex}", this);
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

        // Initialize level database
        levelDatabase.gridSize = gridData.GridSize;
        int goalCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new(x, y);
                if (gridData.GetTileType(pos) == TileType.Goal && gridData.GetGoalType(pos) == GoalType.BasicGoal)
                {
                    goalCount++;
                }
            }
        }
        levelDatabase.GoalsCount = goalCount;
        levelDatabase.gridOrigin = transform;

        // Raise event to notify build completion
        gridEvents.Raise(new GridEventPayload { EventType = GridEventType.BuildGrid, BuildDone = true });
    }

    /// <summary>
    /// Reset current level and destroy all objects
    /// </summary>
    public void ResetLevel()
    {
        // Destroy old grid except pooled backgrounds
        foreach (Transform child in gridRoot.transform)
        {
            // Pool of surrounding obstacles
            if (surroundingObstacles.Contains(child.gameObject))
            {
                child.gameObject.SetActive(false);
                continue;
            }

            // Pool of in-game backgrounds
            if (!backgroundPool.Contains(child.gameObject))
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var background in backgroundPool)
        {
            background.SetActive(false);
        }

        // Clear GridData
        gridData.ClearGrids();

        // Clear level database
        if (levelDatabase != null)
        {
            levelDatabase.gridSize = Vector2Int.zero;
            levelDatabase.GoalsCount = 0;
            levelDatabase.gridCenterStartTarget = null;
            levelDatabase.gridOrigin = null;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, "Level reset complete", this);
    }
    #endregion

    #region Private Build Methods
    /// <summary>
    /// Ensures that the background pool contains at least the specified number of objects.
    /// </summary>
    private void EnsureBackgroundPool(int requiredCount)
    {
        int currentCount = backgroundPool.Count;
        if (requiredCount > currentCount)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Expanding background pool from {currentCount} to {requiredCount} objects", this);
        }

        for (int i = backgroundPool.Count; i < requiredCount; i++)
        {
            var background = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity, gridRoot);
            background.SetActive(false);
            backgroundPool.Add(background);
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Background pool ready with {backgroundPool.Count} objects", this);
    }

    /// <summary>
    /// Build a level from grid data
    /// </summary>
    private void BuildLevelFromGrid()
    {
        Vector2Int gridSize = gridData.GridSize;
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Building level from grid - Processing {gridSize.x * gridSize.y} tiles", this);
        EnsureBackgroundPool(gridSize.x * gridSize.y);

        int bgIndex = 0;

        // For cycle over grid width
        for (int x = 0; x < gridSize.x; x++)
        {
            // For cycle over grid height
            for (int y = 0; y < gridSize.y; y++)
            {
                // Prepare tile from the grid
                Vector2Int tilePos = new(x, y);
                Vector3 worldTilePos = GridUtils.GridToWorld(tilePos);
                TileType tileType = gridData.GetTileType(tilePos);

                var background = backgroundPool[bgIndex++];
                background.transform.SetParent(gridRoot, false);
                background.transform.localPosition = worldTilePos;
                background.SetActive(true);

                // Insert specific tiles
                switch (tileType)
                {
                    case TileType.Empty:
                        gridData.SetTileType(tilePos, TileType.Empty);
                        break;

                    case TileType.Obstacle:
                        ObstacleType obstacleType = gridData.GetObstacleType(tilePos);
                        if (obstacleType == ObstacleType.Hole)
                        {
                            HoleTile holeTile = Instantiate(holePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<HoleTile>();
                            holeTile.transform.SetParent(gridRoot, false);
                            holeTile.transform.localPosition = worldTilePos;
                            holeTile.Init(tileType, tilePos, ObstacleType.Hole);
                        }
                        else if (obstacleType == ObstacleType.WaterHole)
                        {
                            WaterHoleTile waterHoleTile = Instantiate(waterHolePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<WaterHoleTile>();
                            waterHoleTile.transform.SetParent(gridRoot, false);
                            waterHoleTile.transform.localPosition = worldTilePos;
                            waterHoleTile.Init(tileType, tilePos, ObstacleType.WaterHole);
                        }
                        else if (obstacleType == ObstacleType.StoneWall)
                        {
                            GameObject tile = Instantiate(wallStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            tile.transform.SetParent(gridRoot, false);
                            tile.transform.localPosition = worldTilePos;
                        }
                        else // BasicWall
                        {
                            GameObject tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                            tile.transform.SetParent(gridRoot, false);
                            tile.transform.localPosition = worldTilePos;
                        }
                        gridData.SetTileType(tilePos, TileType.Obstacle);
                        break;

                    case TileType.Veverka:
                        gridData.SetTileType(tilePos, TileType.Empty);

                        // Generate veverka
                        CharVeverka veverka = Instantiate(veverkaPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<CharVeverka>();
                        veverka.transform.SetParent(gridRoot, false);
                        veverka.transform.localPosition = worldTilePos;
                        veverka.Init(tileType, CharacterType.BasicVeverka, tilePos);

                        // Center grid according to veverka
                        levelDatabase.gridCenterStartTarget = veverka.transform;
                        DebugLogger.Log(DebugLogCategory.Gameplay, $"Player character (Veverka) placed at {tilePos} and set as camera target", this);
                        break;

                    case TileType.Nut:
                        NutType nutType = gridData.GetNutType(tilePos);
                        if (nutType == NutType.StoneNut)
                        {
                            StoneNutTile stoneNutTile = Instantiate(nutStonePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<StoneNutTile>();
                            stoneNutTile.transform.SetParent(gridRoot, false);
                            stoneNutTile.transform.localPosition = worldTilePos;
                            stoneNutTile.Init(tileType, tilePos, NutType.StoneNut);
                        }
                        else if (nutType == NutType.BasicNut)
                        {
                            BasicNutTile nutTile = Instantiate(nutPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<BasicNutTile>();
                            nutTile.transform.SetParent(gridRoot, false);
                            nutTile.transform.localPosition = worldTilePos;
                            nutTile.Init(tileType, tilePos, NutType.BasicNut);
                        }
                        else if (nutType == NutType.WaterNut)
                        {
                            WaterNutTile waterNutTile = Instantiate(nutWaterPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<WaterNutTile>();
                            waterNutTile.transform.SetParent(gridRoot, false);
                            waterNutTile.transform.localPosition = worldTilePos;
                            waterNutTile.Init(tileType, tilePos, NutType.WaterNut);
                        }
                        break;

                    case TileType.Goal:
                        GoalType goalType = gridData.GetGoalType(tilePos);
                        if (goalType == GoalType.BasicGoal)
                        {
                            GoalTile goalTile = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<GoalTile>();
                            goalTile.transform.SetParent(gridRoot, false);
                            goalTile.transform.localPosition = worldTilePos;
                            goalTile.Init(tileType, tilePos, GoalType.BasicGoal);
                        }
                        break;

                    default: break;
                }
            }
        }

        for (int i = bgIndex; i < backgroundPool.Count; i++)
        {
            backgroundPool[i].SetActive(false);
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Level build complete - {bgIndex} tiles processed, {backgroundPool.Count - bgIndex} background objects deactivated", this);
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
