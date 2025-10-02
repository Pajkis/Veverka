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
    [SerializeField] private WallEvents wallEvents;
    [SerializeField] private RoadEvents roadEvents;
    #endregion

    #region Prefabs for Runtime Spawning
    [Header("Runtime Tile Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallStonePrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject stoneFilledHolePrefab;
    [SerializeField] private GameObject waterHolePrefab;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        gridEvents.AddListener(OnGridEvent);
        nutEvents.AddListener(OnNutEvent);
        goalEvents.AddListener(OnGoalEvent);
        wallEvents.AddListener(OnWallEvent);
        roadEvents.AddListener(OnRoadEvent);
    }

    private void OnDisable()
    {
        gridEvents.RemoveListener(OnGridEvent);
        nutEvents.RemoveListener(OnNutEvent);
        goalEvents.RemoveListener(OnGoalEvent);
        wallEvents.RemoveListener(OnWallEvent);
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
                OnGoalSet(payload);
                break;

            case GoalEventsType.GoalResolved:
                OnGoalResolved(payload);
                break;
        }
    }

    /// <summary>
    /// Handles wall-related events
    /// </summary>
    private void OnWallEvent(WallEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnWallEvent - EventType: {payload.EventType}, Position: {payload.Position}, WallType: {payload.WallType}", this);

        switch (payload.EventType)
        {
            case WallEventType.WallSet:
                OnWallSet(payload);
                break;
        }
    }

    /// <summary>
    /// Handles road-related events
    /// </summary>
    private void OnRoadEvent(RoadEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.GridSystem, $"OnRoadEvent - EventType: {payload.EventType}, Position: {payload.Position}, RoadType: {payload.RoadType}", this);

        switch (payload.EventType)
        {
            case RoadEventType.RoadSet:
                OnRoadSet(payload);
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
    private void OnGoalSet(GoalEventPayload payload)
    {
        if (!payload.InstantiateTile)
        {
            gridData.SetTileType(payload.Position, TileType.Goal);
            gridData.SetGoalType(payload.Position, payload.GoalType);
            return;
        }

        // Instantiate new tile on goal position
        if (payload.GoalType == GoalType.WaterHoleGoal)
        {
            WaterHoleTile holeTile = Instantiate(waterHolePrefab, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<WaterHoleTile>();
            holeTile.transform.SetParent(gridRoot, false);
            holeTile.transform.localPosition = GridUtils.GridToWorld(payload.Position);
            holeTile.Init(TileType.Goal, payload.Position, payload.GoalType);

            // Set tile type and goal type in grid
            gridData.SetTileType(payload.Position, TileType.Goal);
            gridData.SetGoalType(payload.Position, payload.GoalType);

            DebugLogger.Log(DebugLogCategory.GridSystem, $"Water hole goal created at {payload.Position}", this);
        }
    }

    /// <summary>
    /// Handle goal resolved event
    /// </summary>
    private void OnGoalResolved(GoalEventPayload payload)
    {
        if (!payload.GoalRemove)
        {
            return;
        }

        if (!gridData.IsInGrid(payload.Position))
        {
            DebugLogger.LogError(DebugLogCategory.GridSystem, $"Cannot remove goal tile - Position {payload.Position} is outside grid bounds", this);
            return;
        }

        if (!payload.InstantiateTile)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"GoalResolved - Setting {payload.Position} to Empty (GoalRemove: {payload.GoalRemove})", this);
            gridData.SetTileType(payload.Position, TileType.Empty);
            gridData.SetGoalType(payload.Position, GoalType.None);
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Goal resolved at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}, GoalType cleared", this);
            return;
        }
    }
    #endregion

    #region Wall Event Handlers
    /// <summary>
    /// Set wall tile on position and update tile type in the grid
    /// </summary>
    private void OnWallSet(WallEventPayload payload)
    {
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

        // Set tile and wall type in grid
        gridData.SetTileType(payload.Position, TileType.Wall);
        gridData.SetWallType(payload.Position, payload.WallType);
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Wall created at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}, WallType: {payload.WallType}", this);
    }
    #endregion

    #region Road Event Handlers
    /// <summary>
    /// Set road tile on position and update tile type in the grid
    /// </summary>
    private void OnRoadSet(RoadEventPayload payload)
    {
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

        // Set tile type and road type in grid
        gridData.SetTileType(payload.Position, TileType.Road);
        gridData.SetRoadType(payload.Position, payload.RoadType);
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Road created at {payload.Position} - TileType: {gridData.GetTileType(payload.Position)}, RoadType: {payload.RoadType}", this);
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
        payload.WallType = gridData.GetWallType(payload.Position);
        payload.NutType = gridData.GetNutType(payload.Position);
        payload.RoadType = gridData.GetRoadType(payload.Position);

        DebugLogger.Log(DebugLogCategory.GridSystem, $"TileQuery at {payload.Position} - TileType: {payload.TileType}, IsWalkable: {payload.IsWalkable}, IsPushable: {payload.IsPushable}", this);
    }
    #endregion
}
