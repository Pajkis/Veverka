using UnityEngine;

/// <summary>
/// Movable object setup and control
/// </summary>
public abstract class NutTile : TileObject
{
    #region Fields
    [SerializeField] protected int moveDistance = 1;
    [SerializeField] protected float moveDuration = 0.15f;
   
  //  protected float speedMultiplier = 1f;
    protected NutEventPayload payload = new();
    protected SmoothMover smoothMover;
    protected NutType nutType;
    #endregion

    #region  events
    [SerializeField] protected NutEvents nutEvents;
    [SerializeField] protected UndoEvents undoEvents;
    #endregion

    #region Event handling
    /// <summary>
    /// on enable - add listeners
    /// </summary>
    protected void OnEnable()
    {
        nutEvents.AddListener(OnNutEvent);
        undoEvents?.AddListener(OnUndoEvent);
    }
    /// <summary>
    /// remove listeners
    /// </summary>
    protected void OnDisable()
    {
        nutEvents.RemoveListener(OnNutEvent);
        undoEvents?.RemoveListener(OnUndoEvent);
    }
    
    /// <summary>
    /// Handle undo events for this nut
    /// </summary>
    /// <param name="payload"></param>
    private void OnUndoEvent(UndoEventPayload payload)
    {
        if (payload.EventType != UndoEventType.RequestNutUndo)
            return;

        // Check if this undo request is for this nut
        var expectedObjectId = $"Nut-{GetInstanceID()}";
        if (payload.UndoData.ObjectId != expectedObjectId)
            return;

        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Processing undo request {payload.RequestId} for nut move", this);

        ExecuteUndoMove(payload);
    }

    private void ExecuteUndoMove(UndoEventPayload payload)
    {
        if (smoothMover.IsMoving)
        {
            DebugLogger.LogWarning(DebugLogCategory.UndoLogic, "Nut already moving, skipping undo", this);
            CompleteUndoRequest(payload.RequestId);
            return;
        }

        Vector3 fromWorld = GridUtils.GridToWorld(payload.UndoData.CurrentPosition);
        Vector3 toWorld = GridUtils.GridToWorld(payload.UndoData.PreviousPosition);

        // Calculate duration using SAME formula as normal movement
        float effectiveDuration = moveDuration / payload.SpeedMultiplier;

        smoothMover.Move(fromWorld, toWorld, effectiveDuration,
            onStart: null,
            onComplete: () => OnUndoMoveComplete(payload.UndoData, payload.RequestId));

        // Update nut events for grid system
        nutEvents.Raise(new NutEventPayload
        {
            EventType = NutEventType.NutRemoved,
            CurrentPosition = payload.UndoData.CurrentPosition
        });

        nutEvents.Raise(new NutEventPayload
        {
            EventType = NutEventType.NutSet,
            NutType = this.nutType,
            NutTile = this,
            CurrentPosition = payload.UndoData.PreviousPosition
        });

        GridPosition = payload.UndoData.PreviousPosition;

        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Started undo nut move from {payload.UndoData.CurrentPosition} to {payload.UndoData.PreviousPosition} with speed {payload.SpeedMultiplier}", this);
    }

    private void OnUndoMoveComplete(UndoData undoData, string requestId)
    {
        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Undo nut move completed: {undoData.CurrentPosition} → {undoData.PreviousPosition}", this);
        CompleteUndoRequest(requestId);
    }

    private void CompleteUndoRequest(string requestId)
    {
        undoEvents?.Raise(new UndoEventPayload
        {
            EventType = UndoEventType.UndoCompleted,
            RequestId = requestId
        });

        DebugLogger.Log(DebugLogCategory.UndoLogic, $"Undo request {requestId} completed", this);
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialize tile type on grid position, init smooth mover
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    /// <param name="nutType"></param>
    public virtual void Init(TileType tileType, Vector2Int gridPosition, NutType nutType)
    {
        // get the smooth mover component
        smoothMover = GetComponent<SmoothMover>();
        if (smoothMover == null)
        {
            DebugLogger.LogError(DebugLogCategory.NutMovement, $"SmoothMover component is missing on {gameObject.name}", this);
            return;
        }

        // set configs
        moveDuration = gameplayConfig.nutMoveTime;

        // base initialization
        base.Init(tileType, gridPosition);
        this.nutType = nutType;          
    }

    #endregion

    #region pushable methods
    /// <summary>
    /// Handle can push query - only respond if it's for this nut's position
    /// </summary>
    /// <param name="payload"></param>
    private void OnNutEvent(NutEventPayload payload)
    {
        if (payload.EventType != NutEventType.NutPush)
        {
            return;
        }

        DebugLogger.Log(DebugLogCategory.NutMovement, $"OnNutEvent PUSH - GridPosition: {GridPosition}, PayloadPrevious: {payload.PreviousPosition}, PayloadCurrent: {payload.CurrentPosition}", this);

        // Check if this nut matches with position in payload
        if (GridPosition != payload.PreviousPosition)
        {
            DebugLogger.Log(DebugLogCategory.NutMovement, $"Position mismatch - GridPosition: {GridPosition} != PayloadPrevious: {payload.PreviousPosition}", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.NutMovement, $"Position match - calling Move()", this);

        // Calculate effective speed multiplier based on splash effect
        float effectiveSpeedMultiplier = payload.SpeedMultiplier;
        if (payload.IsSplash)
        {
            effectiveSpeedMultiplier *= payload.SplashSpeedFactor;
            DebugLogger.Log(DebugLogCategory.NutMovement, $"Splash push - applying factor {payload.SplashSpeedFactor} (effective: {effectiveSpeedMultiplier})", this);
        }

        Move(payload.Direction, payload.Distance, effectiveSpeedMultiplier);
      
     }

    /// <summary>
    /// Checks, whether tile can be pushed in the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual bool CanBePushed(Direction direction)
    {
        Vector2Int targetPosition = GridUtils.GetPositionInDir(GridPosition, direction);

        TileQueryPayload query = new() { Position = targetPosition };
        gridEvents.Raise(new GridEventPayload
        {
            EventType = GridEventType.TileQuery,
            Query = query
        });
        if (!query.IsInGrid) return false;
        return query.IsPushable;
    }
    #endregion

    #region Move Methods
    /// <summary>
    /// Move of a object over a distance in set direction
    /// </summary>
    /// <param name="direction">direction of movemebt</param>
    /// <param name="distance">distance of movement in tiles</param>
    /// <param name="duration">duration of movement</param>
    public virtual void Move(Direction direction, int distance, float speedMultiplier)
    {
       // if (smoothMover.IsMoving) return;

        // Store the previous position BEFORE any potential updates to GridPosition
        Vector2Int previousPosition = GridPosition;
        DebugLogger.Log(DebugLogCategory.NutMovement, $"Move START - GridPosition: {GridPosition}, PreviousPosition: {previousPosition}", this);

        // calculate movement positions
        Vector3 currentPosition = transform.localPosition;
        Vector2Int targetPosVec2Int = GridUtils.GetPositionInDir(GridPosition, direction, moveDistance);
        Vector3 targetPosition = GridUtils.GridToWorld(targetPosVec2Int);
        
        DebugLogger.Log(DebugLogCategory.NutMovement, $"Move CALC - From: {GridPosition} To: {targetPosVec2Int}", this);

        // prepare movement payload for events and turn records
        payload = new NutEventPayload
        {
            NutType = this.nutType,
            NutTile = this,
            CurrentPosition = targetPosVec2Int,
            PreviousPosition = previousPosition,
            SpeedMultiplier = speedMultiplier,
        };

        DebugLogger.Log(DebugLogCategory.NutMovement, $"Move PAYLOAD - Current: {payload.CurrentPosition}, Previous: {payload.PreviousPosition}", this);

        // execute smooth movement
        smoothMover.Move(currentPosition, targetPosition, moveDuration / speedMultiplier, OnMoveStart, () => OnMoveComplete(targetPosVec2Int));
    }
     
    /// <summary>
    /// On Move start action
    /// </summary>
    protected virtual void OnMoveStart()
    {
      
    }

    /// <summary>
    /// On move complete action 
    /// </summary>
    /// <param name="targetPosition"></param>
    protected virtual void OnMoveComplete(Vector2Int targetPosition)
    {
        DebugLogger.Log(DebugLogCategory.NutMovement, $"OnMoveComplete START - TargetPosition: {targetPosition}, GridPosition: {GridPosition}, PayloadPrevious: {payload.PreviousPosition}", this);

        // remove nut from previous position (use payload.PreviousPosition, not current GridPosition)
        nutEvents.Raise(new NutEventPayload
        {
            EventType = NutEventType.NutRemoved,
            CurrentPosition = payload.PreviousPosition,
        });

        DebugLogger.Log(DebugLogCategory.NutMovement, $"NutRemoved sent for position: {payload.PreviousPosition}", this);

        // Check if the nut reached a goal or obstacle
        TileQueryPayload query = new() { Position = targetPosition };
        gridEvents.Raise(new GridEventPayload
        {
            EventType = GridEventType.TileQuery,
            Query = query
        });

        if (query.TileType == TileType.Goal || query.TileType == TileType.Obstacle)
        {
            // First send NutEnteringGoal event for TurnControl to start goal action
            nutEvents.Raise(new NutEventPayload
            {
                EventType = NutEventType.NutEnteringGoal,
                CurrentPosition = targetPosition,
                PreviousPosition = GridPosition,
                NutType = this.nutType,
                NutTile = this,
                SpeedMultiplier = payload.SpeedMultiplier,
            });

            // Then send NutInGoal event for GoalTile/ObstacleTile to process
            nutEvents.Raise(new NutEventPayload
            {
                EventType = NutEventType.NutInGoal,
                CurrentPosition = targetPosition,
                PreviousPosition = GridPosition,
                NutType = this.nutType,
                NutTile = this,
                SpeedMultiplier = payload.SpeedMultiplier,
            });
            Destroy(gameObject);
        }
        else
        {
            nutEvents.Raise(new NutEventPayload
            {
                EventType = NutEventType.NutSet,
                CurrentPosition = targetPosition,
                NutType = this.nutType,
                NutTile = this,
            });
        }

        DebugLogger.Log(DebugLogCategory.NutMovement, $"UPDATE GridPosition: {GridPosition} → {targetPosition}", this);
        GridPosition = targetPosition;
        DebugLogger.Log(DebugLogCategory.NutMovement, $"GridPosition updated to: {GridPosition}", this);

        // notify listeners about nut movement
        payload.EventType = NutEventType.NutMoved;
        payload.CurrentPosition = targetPosition;
        nutEvents.Raise(payload);

        DebugLogger.Log(DebugLogCategory.NutMovement, $"OnMoveComplete END - Final GridPosition: {GridPosition}", this);
    }
    #endregion

}
