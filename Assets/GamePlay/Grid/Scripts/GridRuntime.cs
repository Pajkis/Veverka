using UnityEngine;

/// <summary>
/// Handles runtime grid updates during gameplay.
/// Responds to events for nuts, goals, walls, roads, and tile queries.
/// Gameplay/Grid/Scripts/GridRuntime
/// </summary>
public class GridRuntime : MonoBehaviour
{
    #region References
    [Header("Core References")]
    [SerializeField] private GridData gridData;
    [SerializeField] private Transform gridRoot;
    #endregion

    #region Events
    [Header("Events")]
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private NutEvents nutEvents;
    [SerializeField] private GoalEvents goalEvents;
    [SerializeField] private ObstacleEvents obstacleEvents;
    [SerializeField] private RoadEvents roadEvents;
    #endregion

    #region Prefabs for Runtime Spawning
    [Header("Runtime Tile Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallStonePrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject stoneFilledHolePrefab;
    [SerializeField] private GameObject waterHolePrefab;
    [SerializeField] private GameObject holePrefab;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        gridEvents.AddListener(OnGridEvent);
        nutEvents.AddListener(OnNutEvent);
        goalEvents.AddListener(OnGoalEvent);
        obstacleEvents.AddListener(OnObstacleEvent);
        roadEvents.AddListener(OnRoadEvent);
    }

    private void OnDisable()
    {
        gridEvents.RemoveListener(OnGridEvent);
        nutEvents.RemoveListener(OnNutEvent);
        goalEvents.RemoveListener(OnGoalEvent);
        obstacleEvents.RemoveListener(OnObstacleEvent);
        roadEvents.RemoveListener(OnRoadEvent);
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles grid events - specifically TileQuery
    /// </summary>
    private void OnGridEvent(GridEventPayload payload)
    {
        switch (payload.EventType)
        {
            case GridEventType.TileQuery:
                OnTileQuery(payload.Query);
                break;
        }
    }

    /// <summary>
    /// Handles nut-related events
    /// </summary>
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
    /// Handles goal-related events
    /// </summary>
    private void OnGoalEvent(GoalEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnGoalEvent - EventType: {payload.EventType}, Position: {payload.Position}, GoalType: {payload.GoalType}, GoalRemove: {payload.GoalRemove}", this);

        switch (payload.EventType)
        {
            case GoalEventsType.GoalSet:
                OnGoalSet(payload.Position, payload.GoalType);
                break;

            case GoalEventsType.GoalResolve:
                OnGoalResolveEvent(payload);
                break;
        }
    }

    /// <summary>
    /// Handles obstacle-related events
    /// </summary>
    private void OnObstacleEvent(ObstacleEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnObstacleEvent - EventType: {payload.EventType}, Position: {payload.Position}, ObstacleType: {payload.ObstacleType}", this);

        switch (payload.EventType)
        {
            case ObstacleEventsType.ObstacleSet:
                if (!payload.InstantiateTile)
                {
                    DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"OnObstacleEvent - InstantiateTile is false for ObstacleSet at Position: {payload.Position}. No action taken.", this);
                    return;
                }
                OnObstacleSet(payload.Position, payload.ObstacleType);
                break;
            case ObstacleEventsType.ObstacleResolve:
                OnObstacleResolveEvent(payload);
                break;
        }
    }

    /// <summary>
    /// Handles road-related events
    /// </summary>
    private void OnRoadEvent(RoadEventPayload payload)
    {
        if (!payload.InstantiateTile)
        {
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"OnRoadEvent - InstantiateTile is false for EventType: {payload.EventType} at Position: {payload.Position}. No action taken.", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnRoadEvent - EventType: {payload.EventType}, Position: {payload.Position}, RoadType: {payload.RoadType}", this);

        switch (payload.EventType)
        {
            case RoadEventType.RoadSet:
                OnRoadSet(payload.Position, payload.RoadType);
                break;
        }
    }
    #endregion

    #region Nut Event Handlers
    /// <summary>
    /// Set nut tile on position and update tile type in the grid
    /// </summary>
    private void OnNutSet(NutEventPayload payload)
    {
        if (!gridData.IsInGrid(payload.CurrentPosition))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot set nut tile - Position {payload.CurrentPosition} is outside grid bounds", this);
            return;
        }
        gridData.SetTileType(payload.CurrentPosition, TileType.Nut);
        gridData.SetNutType(payload.CurrentPosition, payload.NutType);
    }

    /// <summary>
    /// Remove nut tile on position and update tile type in the grid
    /// </summary>
    private void OnNutRemoved(NutEventPayload payload)
    {
        if (!gridData.IsInGrid(payload.CurrentPosition))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot remove nut tile - Position {payload.CurrentPosition} is outside grid bounds", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnNutRemoved at {payload.CurrentPosition} - Old TileType: {gridData.GetTileType(payload.CurrentPosition)}", this);
        gridData.SetTileType(payload.CurrentPosition, TileType.Empty);
        gridData.SetNutType(payload.CurrentPosition, NutType.None);
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Nut removed from {payload.CurrentPosition} - New TileType: {gridData.GetTileType(payload.CurrentPosition)}, NutType cleared", this);
    }
    #endregion

    #region Goal Event Handlers
    /// <summary>
    /// Set goal tile on position and update tile type in the grid
    /// </summary>
    private void OnGoalSet(Vector2Int position, GoalType goalType)
    {
       
        GameObject tile = null;

        //instantiate goal prefab based on goal type
        switch (goalType)
        {
            case GoalType.BasicGoal:
                tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                break;
            default:
                DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"OnGoalSet: Unsupported GoalType {goalType} at {position}, defaulting to BasicGoal", this);
                break;
        }

        // Check if instantiation was successful
        if (tile == null)
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"OnGoalSet: Failed to instantiate goal prefab for GoalType {goalType} at {position}", this);
            return;
        }
        // Get GoalTile component
        GoalTile goalTile = tile.GetComponent<GoalTile>();
        if (goalTile == null)
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"OnGoalSet: GoalTile component not found in tile {goalType} at {position}", this);
            return;
        }

        //set position of the goal
        goalTile.transform.SetParent(gridRoot, false);
        goalTile.transform.localPosition = GridUtils.GridToWorld(position);
        goalTile.Init(TileType.Goal, position, goalType);
        
        // Set tile type and goal type in grid
        gridData.SetTileType(position, TileType.Goal);
        gridData.SetGoalType(position,goalType);

        DebugLogger.Log(DebugLogCategory.GridSystem, $" Goal: {goalType} created at {position}", this);        
    }

    /// <summary>
    /// Handle goal resolved event
    /// </summary>
    private void OnGoalResolveEvent(GoalEventPayload payload)
    {        
        if (!gridData.IsInGrid(payload.Position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot remove goal tile - Position {payload.Position} is outside grid bounds", this);
            return;
        }

        // Goal without removal
        if (!payload.GoalRemove)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"GoalResolve event received without goal removal {payload.Position}", this);            
        }
        // goal removal with replacement
        else if (payload.GoalRemove && !payload.CreateReplacement)
        {          
            gridData.SetTileType(payload.Position, TileType.Empty);
            gridData.SetGoalType(payload.Position, GoalType.None);
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Goal resolved at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}, GoalType cleared", this);
            return;
        }
        // goal removal with replacement and tile instantiation
        else if (payload.GoalRemove && payload.CreateReplacement)
        {
            // Set tile type and replacement type in grid
            gridData.SetGoalType(payload.Position, GoalType.None);

            // Handle replacement based on type
            switch (payload.ReplacementType)
            {
                case TileType.Obstacle:
                    OnObstacleSet(payload.Position, payload.ReplacementObstacleType);
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Build new {payload.ReplacementObstacleType} at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}", this);
                    break;
                case TileType.Road:
                    OnRoadSet(payload.Position, payload.ReplacementRoadType);
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Build new {payload.ReplacementRoadType} at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}", this);
                    break;
                case TileType.Goal:
                    OnGoalSet(payload.Position, payload.ReplacementGoalType);
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Build new {payload.ReplacementGoalType} at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}", this);
                    break;                    
                default:
                    DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"GoalResolve with replacement at {payload.Position} has unsupported ReplacementType: {payload.ReplacementType}", this);
                break;
            }
        }
    }
    #endregion

    #region Obstacle Event Handlers
    /// <summary>
    /// Set obstacle tile on position and update tile type in the grid
    /// </summary>
    private void OnObstacleSet(Vector2Int position, ObstacleType obstacleType)
    {
        GameObject tile = null;

        //instantiate obstacle prefab based on obstacle type
        switch (obstacleType)
        {
            case ObstacleType.StoneWall:
                tile = Instantiate(wallStonePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                break;
            case ObstacleType.BasicWall:
                tile = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, gridRoot);
                break;
            case ObstacleType.Hole:
                tile = Instantiate(holePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                break;
            case ObstacleType.WaterHole:
                tile = Instantiate(waterHolePrefab, Vector3.zero, Quaternion.identity, gridRoot);
                break;
            default:
                DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"OnObstacleSet: Unsupported ObstacleType {obstacleType} at {position}", this);
            break;
        }

        // Check if instantiation was successful
        if (tile == null)
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"OnObstacleSet: Failed to instantiate obstacle prefab for ObstacleType {obstacleType} at {position}", this);
            return;
        }

        //set position of the obstacle
        tile.transform.SetParent(gridRoot, false);
        tile.transform.localPosition = GridUtils.GridToWorld(position);

        // Set tile and obstacle type in grid
        gridData.SetTileType(position, TileType.Obstacle);
        gridData.SetObstacleType(position, obstacleType);
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Obstacle created at {position} - TileType: {gridData.GetTileType(position)}, ObstacleType: {obstacleType}", this);
    }

    /// <summary>
    /// Handle obstacle resolved event
    /// </summary>
    private void OnObstacleResolveEvent(ObstacleEventPayload payload)
    {
        if (!gridData.IsInGrid(payload.Position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot resolve obstacle - Position {payload.Position} is outside grid bounds", this);
            return;
        }

        // Obstacle without removal
        if (!payload.ObstacleRemove)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"ObstacleResolve event received without obstacle removal {payload.Position}", this);
        }
        // Obstacle removal without replacement
        else if (payload.ObstacleRemove && !payload.CreateReplacement)
        {
            gridData.SetTileType(payload.Position, TileType.Empty);
            gridData.SetObstacleType(payload.Position, ObstacleType.None);
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Obstacle resolved at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}, ObstacleType cleared", this);
            return;
        }
        // Obstacle removal with replacement and tile instantiation
        else if (payload.ObstacleRemove && payload.CreateReplacement)
        {
            // Clear obstacle type
            gridData.SetObstacleType(payload.Position, ObstacleType.None);

            // Handle replacement based on type
            switch (payload.ReplacementType)
            {
                case TileType.Obstacle:
                    OnObstacleSet(payload.Position, payload.ReplacementObstacleType);
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Build new {payload.ReplacementObstacleType} at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}", this);
                    break;
                case TileType.Road:
                    OnRoadSet(payload.Position, payload.ReplacementRoadType);
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Build new {payload.ReplacementRoadType} at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}", this);
                    break;
                case TileType.Goal:
                    OnGoalSet(payload.Position, payload.ReplacementGoalType);
                    DebugLogger.Log(DebugLogCategory.GridSystem, $"Build new {payload.ReplacementGoalType} at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}", this);
                    break;
                default:
                    DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"ObstacleResolve with replacement at {payload.Position} has unsupported ReplacementType: {payload.ReplacementType}", this);
                break;
            }
        }
    }
    #endregion

    #region Road Event Handlers
    /// <summary>
    /// Set road tile on position and update tile type in the grid
    /// </summary>
    private void OnRoadSet(Vector2Int position, RoadType roadType)
    {
        // Instantiate new tile on road position
        GameObject tile = null;

        //instantiate road prefab based on road type
        switch (roadType)
        { 
         case RoadType.BasicRoad:
            tile = Instantiate(roadPrefab, Vector3.zero, Quaternion.identity, gridRoot);
            break;
        case RoadType.StoneFilledHole:
            tile = Instantiate(stoneFilledHolePrefab, Vector3.zero, Quaternion.identity, gridRoot);
            break;
        default:
            DebugLogger.LogWarning(DebugLogCategory.GridSystem, $"OnRoadSet: Unsupported RoadType {roadType} at {position}, defaulting to basic road", this);
            break;
        }

        // Check if instantiation was successful
        if (tile == null)
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"OnRoadSet: Failed to instantiate road prefab for RoadType {roadType} at {position}", this);
            return;
        }

        //set position of the road
        tile.transform.SetParent(gridRoot, false);
        tile.transform.localPosition = GridUtils.GridToWorld(position);        

        // Set tile type and road type in grid
        gridData.SetTileType(position, TileType.Road);
        gridData.SetRoadType(position, roadType);
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Road created at {position} - TileType: {gridData.GetTileType(position)}, RoadType: {roadType}", this);
    }
    #endregion

    #region Tile Query Handler
    /// <summary>
    /// Responds to tile query events and fills the payload with information about the tile.
    /// </summary>
    private void OnTileQuery(TileQueryPayload payload)
    {
        payload.IsInGrid = gridData.IsInGrid(payload.Position);
        if (!payload.IsInGrid) return;

        payload.IsWalkable = gridData.IsWalkableAt(payload.Position);
        payload.IsPushable = gridData.IsPushableAt(payload.Position);
        payload.TileType = gridData.GetTileType(payload.Position);
        payload.GoalType = gridData.GetGoalType(payload.Position);
        payload.ObstacleType = gridData.GetObstacleType(payload.Position);
        payload.NutType = gridData.GetNutType(payload.Position);
        payload.RoadType = gridData.GetRoadType(payload.Position);

        DebugLogger.Log(DebugLogCategory.GridSystem, $"TileQuery at {payload.Position} - TileType: {payload.TileType}, IsWalkable: {payload.IsWalkable}, IsPushable: {payload.IsPushable}", this);
    }
    #endregion
}
