using UnityEngine;

/// <summary>
/// goal tile abstract class
/// </summary>
public abstract class GoalTile : TileObject
{
    protected GoalType goalType;

    #region events
    [SerializeField] protected NutEvents nutEvents;
    [SerializeField] protected GoalSetEvent goalSetEvent;
    [SerializeField] protected GoalResolvedEvent goalResolvedEvent;
    [SerializeField] protected WallSetEvent wallSetEvent;
    [SerializeField] protected RoadSetEvent roadSetEvent;
    #endregion
    
    #region Init
    /// <summary>
    /// Init goals and register them in GameGrid
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    /// <param name="goalType"></param>
    public virtual void Init(TileType tileType, Vector2Int gridPosition, GoalType goalType)
    {
        base.Init(tileType, gridPosition);
        this.goalType = goalType;
        goalSetEvent.Raise(new GoalBasicPayload
        {
            Position = gridPosition,
            GoalType = goalType,
            GoalTile = this,
        });
    }

    /// <summary>
    /// On enable add listener
    /// </summary>
    protected void OnEnable()
    {
        nutEvents.AddListener(OnNutEvent);
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    protected void OnDisable()
    {
        nutEvents.RemoveListener(OnNutEvent);
    }
    #endregion

    #region methods
    /// <summary>
    /// Recaction on nut reaching the goal
    /// </summary>
    /// <param name="payload"></param>
    private void OnNutEvent(NutEventPayload payload)
    {
        if (payload.EventType == NutEventType.NutInGoal)
        {
            OnNutInGoal(payload);
        }
    }

    protected virtual void OnNutInGoal(NutEventPayload payload)
    { }
    #endregion
}
