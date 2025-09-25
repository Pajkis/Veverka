using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defining Game grid and basic grid functions
/// Gameplay/Grid/Scripts/GameGrid
/// </summary>
public class GameGrid : MonoBehaviour
{
    #region global vars
    // global grid - Singleton 
    public static GameGrid Instance { get; private set; }
    #endregion

    #region events
    [Header("Events")]
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private LevelSelectEvent levelSelectEvent;
    [SerializeField] private LevelDatabase levelDatabase;

    [SerializeField] private NutEvents nutEvents;
    [SerializeField] private GoalEvents goalEvents;
    [SerializeField] private WallEvents wallEvents;
    [SerializeField] private RoadEvents roadEvents;
    #endregion

    #region tile objects
    [Header("Tile objects")]
    // Prefabs
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

    [Header("Other objects")]
    [SerializeField] private GameObject backgroundPrefab;
    //Adjust screen for each level size
    [SerializeField] private Transform gridRoot;
    #endregion

    #region fields
    // grid variables       
    readonly Vector2Int maxScreenGridSize = new(15, 11);
    private Vector2Int gridSize;     
    private TileType[,] grid;
    private NutType[,] nutGrid;
    private GoalType[,] goalGrid;
    private WallType[,] wallGrid;
    private RoadType[,] roadGrid;
    private readonly List<GameObject> backgroundPool = new();
    // pool for surrounding wall tiles to avoid repeated instantiation
    private readonly List<GameObject> surroundingWalls = new();
    #endregion
           
    #region Awake
    /// <summary>
    /// init singleton and events
    /// </summary>
    void Awake()
    {
        // singleton routine
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// On object enable
    /// </summary>
    private void OnEnable()
    {
        // add listeners for events
        levelSelectEvent.AddListener(OnLevelSelected);
        gridEvents.AddListener(OnGridEvent);
        nutEvents.AddListener(OnNutEvent);
        goalEvents.AddListener(OnGoalEvent);
        wallEvents.AddListener(OnWallEvent);
        roadEvents.AddListener(OnRoadEvent);
    }

    /// <summary>
    /// On object disable
    /// </summary>
    private void OnDisable()
    {
        // remove listeners for events
        levelSelectEvent.RemoveListener(OnLevelSelected);
        gridEvents.RemoveListener(OnGridEvent);
        nutEvents.RemoveListener(OnNutEvent);
        goalEvents.RemoveListener(OnGoalEvent);
        wallEvents.RemoveListener(OnWallEvent);
        roadEvents.RemoveListener(OnRoadEvent);
    }

    /// <summary>
    /// Handles grid-related events based on the specified payload.
    /// </summary>
    /// <remarks>This method processes different types of grid events: <list type="bullet"> <item>
    /// <description> For <see cref="GridEventType.BuildGrid"/>, initializes the grid if the build is not already
    /// completed. </description> </item> <item> <description> For <see cref="GridEventType.ResetGrid"/>, resets the
    /// grid to its default state. </description> </item> <item> <description> For <see
    /// cref="GridEventType.TileQuery"/>, processes a tile query using the provided query data. </description> </item>
    /// </list></remarks>
    /// <param name="payload">The event payload containing the event type and associated data.  The payload must not be <see
    /// langword="null"/>.</param>
    private void OnGridEvent(GridEventPayload payload)
    {
        switch (payload.EventType)
        {
            case GridEventType.BuildGrid:
                if (!payload.BuildDone)
                {
                    InitializeGrid(payload.Grid);
                }
                break;
            case GridEventType.ResetGrid:
                ResetGrid();
                break;
            case GridEventType.TileQuery:
                OnTileQuery(payload.Query);
                break;
        }
    }

    #endregion

    #region Grid Init
    /// <summary>
    /// Initialize a new grid
    /// </summary>
    /// <param name="grid"></param>
    private void InitializeGrid(TileType[,] grid)
    {
        //get new grid size
        this.grid = grid;
        this.nutGrid = GridUtils.CachedNutGrid;
        this.goalGrid = GridUtils.CachedGoalGrid;
        this.wallGrid= GridUtils.CachedWallGrid;
        this.roadGrid = GridUtils.CachedRoadGrid;
        this.gridSize.x = grid.GetLength(0);
        this.gridSize.y = grid.GetLength(1);
            

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Grid initialized - Size: {gridSize.x}x{gridSize.y}", this);

        // Spawn objects based on tile types
        DebugLogger.Log(DebugLogCategory.GridSystem, "Starting level build from grid data", this);
        BuildLevelFromGrid();

        // add surrounding walls where grid does not fill the screen
        DebugLogger.Log(DebugLogCategory.GridSystem, "Building surrounding walls for screen fill", this);
        BuildSurroundings();

        //level database initialization
        levelDatabase.gridSize = gridSize;
        int goalCount = 0;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y] == TileType.Goal && goalGrid[x, y] == GoalType.BasicGoal)
                {
                    goalCount++;
                }
            }
        }
        levelDatabase.GoalsCount = goalCount;
        levelDatabase.gridOrigin = transform;

        // raise event to notify build completion
        gridEvents.Raise(new GridEventPayload { EventType = GridEventType.BuildGrid, BuildDone = true });
    }

    /// <summary>
    /// Ensures that the background pool contains at least the specified number of objects.
    /// </summary>
    /// <remarks>If the current count of objects in the background pool is less than <paramrefname="requiredCount"/>,
    /// additional objects are instantiated using the <c>backgroundPrefab</c> and added to
    /// the pool.  Newly instantiated objects are initialized at the origin, deactivated, and parented to
    /// <c>gridRoot</c>.</remarks>
    /// <param name="requiredCount">The minimum number of objects that the background pool should contain.</param>
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
    /// Build a level from a grid
    /// </summary>
    private void BuildLevelFromGrid()
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Building level from grid - Processing {gridSize.x * gridSize.y} tiles", this);
        EnsureBackgroundPool(gridSize.x * gridSize.y);

        int bgIndex = 0;

        // for cycle over grid width
        for (int x = 0; x < gridSize.x; x++)
        {
            // for cycle over grid height
            for (int y = 0; y < gridSize.y; y++)
            {
                //Prepare tile from the grid
                Vector2Int tilePos = new(x, y);
                Vector3 worldTilePos = GridUtils.GridToWorld(tilePos);
                TileType tileType =  GetTileType(tilePos);

                var background = backgroundPool[bgIndex++];
                background.transform.SetParent(gridRoot, false);
                background.transform.localPosition = worldTilePos;
                background.SetActive(true);

                // insert specific tiles
                switch (tileType)
                {
                    case TileType.Empty:

                        SetTileType(tilePos, TileType.Empty);
                        break;

                    case TileType.Wall:

                        GameObject tile;
                        if (wallGrid[x, y] == WallType.StoneWall)
                        {
                            tile = Instantiate(wallStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                        }
                        else
                        {
                            tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                        }
                        tile.transform.SetParent(gridRoot, false);
                        tile.transform.localPosition = worldTilePos;
                        SetTileType(tilePos, TileType.Wall);
                        break;

                    case TileType.Veverka:

                        SetTileType(tilePos, TileType.Empty);

                        // generete veverka
                        CharVeverka veverka = Instantiate(veverkaPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<CharVeverka>();
                        veverka.transform.SetParent(gridRoot, false);
                        veverka.transform.localPosition = worldTilePos;
                        veverka.Init(tileType, CharacterType.BasicVeverka, tilePos);

                        //Center grid according veverka
                        levelDatabase.gridCenterStartTarget = veverka.transform;
                        DebugLogger.Log(DebugLogCategory.Gameplay, $"Player character (Veverka) placed at {tilePos} and set as camera target", this);
                        break;

                    case TileType.Nut:

                        if (nutGrid[x, y] == NutType.StoneNut)
                        {
                            StoneNutTile stoneNutTile = Instantiate(nutStonePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<StoneNutTile>();
                            stoneNutTile.transform.SetParent(gridRoot, false);
                            stoneNutTile.transform.localPosition = worldTilePos;
                            stoneNutTile.Init(tileType, tilePos, NutType.StoneNut);
                        }
                        else if (nutGrid[x, y] == NutType.BasicNut)
                        {
                            BasicNutTile nutTile = Instantiate(nutPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<BasicNutTile>();
                            nutTile.transform.SetParent(gridRoot, false);
                            nutTile.transform.localPosition = worldTilePos;
                            nutTile.Init(tileType, tilePos, NutType.BasicNut);
                        }
                        else if (nutGrid[x, y] == NutType.WaterNut)
                        {
                            WaterNutTile waterNutTile = Instantiate(nutWaterPrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<WaterNutTile>();
                            waterNutTile.transform.SetParent(gridRoot, false);
                            waterNutTile.transform.localPosition = worldTilePos;
                            waterNutTile.Init(tileType, tilePos, NutType.WaterNut);
                        }
                        break;

                    case TileType.Goal:

                        if (goalGrid[x, y] == GoalType.HoleGoal)
                        {
                            HoleTile holeTile = Instantiate(holePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<HoleTile>();
                            holeTile.transform.SetParent(gridRoot, false);
                            holeTile.transform.localPosition = worldTilePos;
                            holeTile.Init(tileType, tilePos, GoalType.HoleGoal);
                        }
                        else if (goalGrid[x, y] == GoalType.WaterHoleGoal)
                        {
                            WaterHoleTile holeTile = Instantiate(waterHolePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<WaterHoleTile>();
                            holeTile.transform.SetParent(gridRoot, false);
                            holeTile.transform.localPosition = worldTilePos;
                            holeTile.Init(tileType, tilePos, GoalType.WaterHoleGoal);
                        }
                        else
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
    void BuildSurroundings()
    {
        // Calculate differences for each dimension
        int diffX = maxScreenGridSize.x - gridSize.x;
        int diffY = maxScreenGridSize.y - gridSize.y;

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Building surroundings - Grid: {gridSize.x}x{gridSize.y}, MaxScreen: {maxScreenGridSize.x}x{maxScreenGridSize.y}, Diff: {diffX}x{diffY}", this);

        // If both dimensions are larger than or equal to max screen size, do not build surroundings
        if (diffX <= 0 && diffY <= 0)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, "Grid fills or exceeds screen size - no surrounding walls needed", this);
            return;
        }

        // Calculate offsets - if dimension is larger than screen, offset should be 0
        int offsetLeft = diffX > 0 ? diffX / 2 : 0;
        int offsetRight = diffX > 0 ? diffX - offsetLeft : 0;
        int offsetDown = diffY > 0 ? diffY / 2 : 0;
        int offsetUp = diffY > 0 ? diffY - offsetDown : 0;

        // Calculate number of needed surrounding walls
        int widthWithOffset = gridSize.x + offsetLeft + offsetRight;
        int needed = 0;

        // Count walls needed for top/bottom (if height is smaller than screen)
        if (diffY > 0)
        {
            needed += widthWithOffset * (offsetDown + offsetUp);
        }

        // Count walls needed for left/right (if width is smaller than screen)
        if (diffX > 0)
        {
            needed += gridSize.y * (offsetLeft + offsetRight);
        }

        // If no walls are needed, return early
        if (needed == 0)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, "No surrounding walls needed", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Need {needed} surrounding walls - Offsets: Left:{offsetLeft}, Right:{offsetRight}, Down:{offsetDown}, Up:{offsetUp}", this);

        // If there are not enough walls in pool, instantiate new ones
        int currentWalls = surroundingWalls.Count;
        if (needed > currentWalls)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Expanding wall pool from {currentWalls} to {needed} walls", this);
        }

        for (int i = surroundingWalls.Count; i < needed; i++)
        {
            var wall = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
            surroundingWalls.Add(wall);
        }

        int index = 0;

        // Position surrounding walls on bottom (only if height is smaller than screen)
        if (diffY > 0)
        {
            for (int y = -offsetDown; y < 0; y++)
            {
                for (int x = -offsetLeft; x < gridSize.x + offsetRight; x++)
                {
                    PositionWall(index++, x, y);
                }
            }
        }

        // Position surrounding walls on top (only if height is smaller than screen)
        if (diffY > 0)
        {
            for (int y = gridSize.y; y < gridSize.y + offsetUp; y++)
            {
                for (int x = -offsetLeft; x < gridSize.x + offsetRight; x++)
                {
                    PositionWall(index++, x, y);
                }
            }
        }

        // Position surrounding walls on left (only if width is smaller than screen)
        if (diffX > 0)
        {
            for (int x = -offsetLeft; x < 0; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PositionWall(index++, x, y);
                }
            }
        }

        // Position surrounding walls on right (only if width is smaller than screen)
        if (diffX > 0)
        {
            for (int x = gridSize.x; x < gridSize.x + offsetRight; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PositionWall(index++, x, y);
                }
            }
        }

        // Deactivate remaining walls in pool
        int deactivatedWalls = 0;
        for (; index < surroundingWalls.Count; index++)
        {
            surroundingWalls[index].SetActive(false);
            deactivatedWalls++;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Surrounding walls complete - {index} walls positioned, {deactivatedWalls} walls deactivated", this);
    }

    /// <summary>
    /// Positions a wall at the specified grid coordinates and activates it.
    /// </summary>
    /// <remarks>The method activates the wall at the specified index, sets its parent to the grid
    /// root, and positions it at the corresponding world coordinates. Ensure that the <paramref name="index"/> is
    /// within the bounds of the surrounding walls collection.</remarks>
    /// <param name="index">The index of the wall in the surrounding walls collection. Must be a valid index within the collection.</param>
    /// <param name="x">The x-coordinate of the grid position where the wall should be placed.</param>
    /// <param name="y">The y-coordinate of the grid position where the wall should be placed.</param>
    void PositionWall(int index, int x, int y)
    {
        var wall = surroundingWalls[index];
        wall.SetActive(true);
        Vector3 worldTilePos = GridUtils.GridToWorld(new Vector2Int(x, y));
        wall.transform.SetParent(gridRoot, false);
        wall.transform.localPosition = worldTilePos;
    }

    /// <summary>
    /// Reset grid and raise again to load selected level
    /// </summary>
    /// <param name="payload"> level select payload</param>
    private void OnLevelSelected(LevelSelectPayload payload)
    {
        // Executes only when reset is requested
        if (payload.resetRequested)
        {
            ResetGrid();

            // raise again for other listeners
            levelSelectEvent.Raise(new LevelSelectPayload
            {
                levelNumber = payload.levelNumber,
                resetRequested = false
            });

            DebugLogger.Log(DebugLogCategory.GridSystem, $"Grid reset complete - Starting level {levelDatabase.CurrentLevelIndex}", this);
            return;
        }
    }

    /// <summary>
    /// Reset grid layout 
    /// </summary>
    /// <param name="noInt">no integer input necessary</param>
    private void ResetGrid()
    {
        //Destroy old grid except pooled backgrounds
        foreach (Transform child in gridRoot.transform)
        {
            // pool of surrounding walls
            if (surroundingWalls.Contains(child.gameObject))
            {
                child.gameObject.SetActive(false);
                continue;
            }
                
            // pool of in game backgrounds
            if (!backgroundPool.Contains(child.gameObject))
            {
                Destroy(child.gameObject);
            }                
        }              

        foreach (var background in backgroundPool)
        {
            background.SetActive(false);
        }

        // clear grids
        nutGrid = null;
        goalGrid = null;
        wallGrid = null;
        roadGrid = null;

        // clear level database
        if (levelDatabase != null)
        {
            levelDatabase.gridSize = Vector2Int.zero;
            levelDatabase.GoalsCount = 0;
            levelDatabase.gridCenterStartTarget = null;
            levelDatabase.gridOrigin = null;
        }
    }
    #endregion

    #region tile handling
        
    /// <summary>
    /// Handles wall-related events.
    /// </summary>
    /// <param name="payload"></param>
    private void OnWallEvent(WallEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnWallEvent - EventType: {payload.EventType}, Position: {payload.Position}, WallType: {payload.WallType}", this);

        switch (payload.EventType)
        {
            case WallEventType.WallSet:
                if (payload.InstantiateTile)
                {
                    GameObject tile;
                    if (payload.WallType == WallType.StoneWall)
                    {
                        tile = Instantiate(wallStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                    }
                    else
                    {
                        tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                    }

                    tile.transform.SetParent(gridRoot, false);
                    tile.transform.localPosition = GridUtils.GridToWorld(payload.Position);
                }

                // set tile and wall type in grid
                SetTileType(payload.Position, TileType.Wall);
                wallGrid[payload.Position.x, payload.Position.y] = payload.WallType;
                DebugLogger.Log(DebugLogCategory.GridSystem, $"Wall created at {payload.Position} - TileType: {GetTileType(payload.Position)}, WallType: {payload.WallType}", this);
                break;
        }
    }

    /// <summary>
    /// Handles road-related events.
    /// </summary>
    /// <param name="payload"></param>
    private void OnRoadEvent(RoadEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnRoadEvent - EventType: {payload.EventType}, Position: {payload.Position}, RoadType: {payload.RoadType}", this);

        switch (payload.EventType)
        {
            case RoadEventType.RoadSet:
                if (payload.InstantiateTile)
                {
                    GameObject tile;
                    if (payload.RoadType == RoadType.StoneFilledHole)
                    {
                        tile = Instantiate(stoneFilledHolePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                    }
                    else
                    {
                        tile = Instantiate(roadPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                    }

                    tile.transform.SetParent(gridRoot, false);
                    tile.transform.localPosition = GridUtils.GridToWorld(payload.Position);
                }

                // set tile type and road type in grid
                SetTileType(payload.Position, TileType.Road);
                roadGrid[payload.Position.x, payload.Position.y] = payload.RoadType;
                DebugLogger.Log(DebugLogCategory.GridSystem, $"Road created at {payload.Position} - TileType: {GetTileType(payload.Position)}, RoadType: {payload.RoadType}", this);
                break;
        }
    }

    private void OnNutEvent(NutEventPayload payload)
    {
        switch (payload.EventType)
        {
            case NutEventType.NutSet:
                OnNutSet(payload);
                break;
            case NutEventType.NutRemoved:
                OnNutRemoved(payload);
                break;
        }
    }

    /// <summary>
    /// Set nut tile on position and update tile type in the grid
    /// </summary>
    /// <param name="payload"></param>
    private void OnNutSet(NutEventPayload payload)
    {
        if (!IsInGrid(payload.CurrentPosition))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot set nut tile - Position {payload.CurrentPosition} is outside grid bounds", this);
            return;
        }
        SetTileType(payload.CurrentPosition, TileType.Nut);
        nutGrid[payload.CurrentPosition.x, payload.CurrentPosition.y] = payload.NutType;
    }

    /// <summary>
    /// Remove nut tile on position and update tile type in the grid
    /// </summary>
    /// <param name="payload"></param>
    private void OnNutRemoved(NutEventPayload payload)
    {
        if (!IsInGrid(payload.CurrentPosition))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot remove nut tile - Position {payload.CurrentPosition} is outside grid bounds", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnNutRemoved at {payload.CurrentPosition} - Old TileType: {GetTileType(payload.CurrentPosition)}", this);
        SetTileType(payload.CurrentPosition, TileType.Empty);
        nutGrid[payload.CurrentPosition.x, payload.CurrentPosition.y] = NutType.None; // Clear nut from grid
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Nut removed from {payload.CurrentPosition} - New TileType: {GetTileType(payload.CurrentPosition)}, NutType cleared", this);
    }

    /// <summary>
    /// Set goal tile on position and update tile type in the grid
    /// </summary>
    /// <param name="payload"></param>
    private void OnGoalEvent(GoalEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnGoalEvent - EventType: {payload.EventType}, Position: {payload.Position}, GoalType: {payload.GoalType}, GoalRemove: {payload.GoalRemove}", this);

        switch (payload.EventType)
        {
            case GoalEventsType.GoalSet:

                if(!payload.InstantiateTile)
                {
                    SetTileType(payload.Position, TileType.Goal);
                    goalGrid[payload.Position.x, payload.Position.y] = payload.GoalType;
                    return;
                }

                // Instantiate new tile on goal position
                if ( payload.GoalType == GoalType.WaterHoleGoal)
                {                   
                    WaterHoleTile holeTile = Instantiate(waterHolePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<WaterHoleTile>();
                    holeTile.transform.SetParent(gridRoot, false);
                    holeTile.transform.localPosition = GridUtils.GridToWorld(payload.Position);
                    holeTile.Init(TileType.Goal, payload.Position, payload.GoalType);
                    
                    // Set tile type and goal type in grid
                    SetTileType(payload.Position, TileType.Goal);
                    goalGrid[payload.Position.x, payload.Position.y] = payload.GoalType;

                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Water hole goal created at {payload.Position}", this);
                }              
                break;

            case GoalEventsType.GoalResolved:
                if (!payload.GoalRemove)
                {
                    return;
                }

                if (!IsInGrid(payload.Position))
                {
                    DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot remove nut tile - Position {payload.Position} is outside grid bounds", this);
                    return;
                }

                if (!payload.InstantiateTile)
                {
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"GoalResolved - Setting {payload.Position} to Empty (GoalRemove: {payload.GoalRemove})", this);
                    SetTileType(payload.Position, TileType.Empty);
                    goalGrid[payload.Position.x, payload.Position.y] = GoalType.None; // Clear goal from grid
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Goal resolved at {payload.Position} - TileType: {GetTileType(payload.Position)}, GoalType cleared", this);
                    return;
                }

                break;
        }
    }

    /// <summary>
    /// Responds to tile query events and fills the payload with information about the tile.
    /// </summary>
    /// <param name="payload">Query payload containing position to check.</param>
    private void OnTileQuery(TileQueryPayload payload)
    {
        payload.IsInGrid = IsInGrid(payload.Position);
        if (!payload.IsInGrid) return;

        payload.IsWalkable = IsWalkableAt(payload.Position);
        payload.IsPushable = IsPushableAt(payload.Position);
        payload.TileType = grid[payload.Position.x, payload.Position.y];
        payload.GoalType = goalGrid[payload.Position.x, payload.Position.y];
        payload.WallType = wallGrid[payload.Position.x, payload.Position.y];

        DebugLogger.Log(DebugLogCategory.GridSystem, $"TileQuery at {payload.Position} - TileType: {payload.TileType}, IsWalkable: {payload.IsWalkable}, IsPushable: {payload.IsPushable}", this);
        payload.NutType = nutGrid[payload.Position.x, payload.Position.y];
        payload.RoadType = roadGrid[payload.Position.x, payload.Position.y];
    }

    /// <summary>
    /// get tile type on input position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileType GetTileType(Vector2Int position)
    {
        return grid[position.x, position.y];
    }

    /// <summary>
    /// Set tile type on position
    /// </summary>
    /// <param name="position"> position of set tile</param>
    /// <param name="tileType">tile type to be set </param>
    public void SetTileType(Vector2Int position, TileType tileType)
    {
        grid[position.x, position.y] = tileType;
    }

    /// <summary>
    /// Checks whether a tile at position can be walked on by a character.
    /// </summary>
    public bool IsWalkableAt(Vector2Int position)
    {
        var tile = grid[position.x, position.y];
        if (tile == TileType.Goal)
        {
            // Characters can walk on BasicGoals but not holes
            var goalType = goalGrid[position.x, position.y];
            return goalType != GoalType.HoleGoal && goalType != GoalType.WaterHoleGoal;
        }
        return tile == TileType.Empty || tile == TileType.Road;
    }

    /// <summary>
    /// Checks whether a tile at position can be entered by a nut.
    /// </summary>
    public bool IsPushableAt(Vector2Int position)
    {
        var tile = grid[position.x, position.y];       
        return tile == TileType.Empty || tile == TileType.Road || tile == TileType.Goal;
    }
        
    /// <summary>
    /// Check whether position is in game grid
    /// </summary>
    /// <param name="position">tile position to check (x,y)</param>
    /// <returns></returns>
    public bool IsInGrid(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridSize.x
            && position.y >= 0 && position.y < gridSize.y)
        {
            return true;
        }
        else
        {              
            return false;
        }                 
    }        
    #endregion
}