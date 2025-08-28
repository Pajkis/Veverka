using UnityEngine;

/// <summary>
/// Movable object setup and control
/// </summary>
public class NutTile : TileObject
{
    #region Fields
    [SerializeField] protected int moveDistance = 1;
    [SerializeField] protected float moveDuration = 0.15f;
    protected float animationSpeed;

    protected NutMovedPayload payload = new();
    protected SmoothMover smoothMover;
    #endregion

    #region  nut events
    [SerializeField]  protected NutInGoalEvent nutInGoalEvent;
    [SerializeField]  protected NutSetEvent nutSetEvent;
    [SerializeField]  protected NutRemovedEvent nutRemovedEvent;
    #endregion

    #region Event handling
    /// <summary>
    /// on enable - add listeners
    /// </summary>
    protected void OnEnable()
    {
        settingDataBroadcastEvent.AddListener(OnSettingData);
        settingDataRequestEvent.Raise(GameSettingsEnum.AnimationSpeed);
    }
    /// <summary>
    /// remove listeners
    /// </summary>
    protected void OnDisable()
    {
        settingDataBroadcastEvent.RemoveListener(OnSettingData);
    }

    /// <summary>
    /// get animation speed from settings
    /// </summary>
    /// <param name="payload"></param>
    private void OnSettingData(SettingDataPayload payload)
    {
        if (payload.Setting == GameSettingsEnum.AnimationSpeed)
        {
            animationSpeed = payload.Value;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialize tile type on grid position, init smooth mover
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        // get the smooth mover component
        smoothMover = GetComponent<SmoothMover>();
        if (smoothMover == null)
        {
            Debug.LogError($"MovableTile: SmoothMover component is missing on {gameObject.name}");
            return;
        }

        // set configs
        moveDuration = gameplayConfig.moveTime;

        // base initialization
        base.Init(tileType, gridPosition);
        nutSetEvent.Raise(new NutSetPayload
        {
            Position = gridPosition,
            NutType = this,
        });
    }

    #endregion

    #region pushable methods
    /// <summary>
    /// Checks, whether tile can be pushed in the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CanBePushed(Direction direction)
    {
        Vector2Int targetPosition = GridUtils.GetPositionInDir(GridPosition, direction);

        TileQueryPayload query = new() { Position = targetPosition };
        tileQueryEvent.Raise(query);
        if (!query.IsInGrid) return false;
        return query.IsWalkable;
    }
    #endregion

    #region Move Methods
   

    /// <summary>
    /// Move of a object over a distance in set direction
    /// </summary>
    /// <param name="direction">direction of movemebt</param>
    /// <param name="distance">distance of movement in tiles</param>
    /// <param name="duration">duration of movement</param>
    public virtual void Move(Direction direction, int distance, float duration)
    {
        if (smoothMover.IsMoving) return;

        //Register turn record
        TurnRecordExpectSource();

        // calculate movement positions
        Vector3 currentPosition = transform.localPosition;
        Vector2Int targetPosVec2Int = GridUtils.GetPositionInDir(GridPosition, direction, moveDistance);
        Vector3 targetPosition = GridUtils.GridToWorld(targetPosVec2Int);
        float moveDuration = (duration * distance) / animationSpeed;

        // fill character moved payload for events - calling events in children classes 
        payload = new NutMovedPayload
        {
            NutType = this,
            Current = targetPosVec2Int,
            Previous = GridPosition,
         //   direction = direction,
        };

        // execute smooth movement
        smoothMover.Move(currentPosition, targetPosition, moveDuration, OnMoveStart, () => OnMoveComplete(targetPosVec2Int));
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
        nutSetEvent.Raise(new NutSetPayload
        {
            NutType = this,
            Position = targetPosition
        });

        GridPosition = targetPosition;
        //Complete turn record
        TurnRecordAddandComplete();
    }
    #endregion

    #region Undo record

    /// <summary>
    /// Undo move of movable tile
    /// </summary>
    /// <param name="previous"> previous position</param>
    /// <param name="current">current position</param>  
    /// <param name="duration"> duration of movement</param>
    public virtual void UndoAction(Vector2Int current, Vector2Int previous, float duration)
    {
        if (smoothMover.IsMoving) return;

        Vector3 fromWorld = GridUtils.GridToWorld(current);
        Vector3 toWorld = GridUtils.GridToWorld(previous);

        //Rotate(direction);
        smoothMover.Move(fromWorld, toWorld, duration,
            onStart: null, onComplete: null);

        Debug.Log($"Undo movable tile:  {previous} → {current}");
        GridPosition = previous;

        nutRemovedEvent.Raise(new NutRemovedPayload
        {
            Position = current
        });

        nutSetEvent.Raise(new NutSetPayload
        {
            NutType = this,
            Position = previous
        });
    }

    /// <summary>
    /// inform TurnBuilder, that this source will provide data for this turn record
    /// </summary>
    protected void TurnRecordExpectSource()
    {
        // UndoID for turn history record
        UndoId = $"{GetType().Name}-{GridPosition.x}x{GridPosition.y}";
        // inform turn builder, that this component is going to register a action into turn record
        TurnBuilder.Instance.ExpectSource(UndoId);
    }

    /// <summary>
    /// add action into the turn record and complete source for turn builder
    /// </summary>   
    protected void TurnRecordAddandComplete()
    {
        // register movement as action into turn record 
        TurnBuilder.Instance.AddAction(
         new UndoMovableAction(this, payload.Current, payload.Previous)
           );

        // inform turn builder, tht this component has registered an action into turn record
        TurnBuilder.Instance.NotifySourceComplete(UndoId);
        Debug.Log($"[{this.GetType().Name} move Complete] Added + Notified {UndoId}");
    }

    #endregion
}
