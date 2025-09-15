using UnityEngine;

/// <summary>
/// goal tile abstract class
/// </summary>
public abstract class GoalTile : TileObject
{
    protected GoalType goalType;
    
    #region events
    [SerializeField] protected NutEvents nutEvents;
    [SerializeField] protected GoalEvents goalEvents;
    [SerializeField] protected WallEvents wallEvents;
    [SerializeField] protected RoadEvents roadEvents;
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
        goalEvents.Raise(new GoalEventPayload
        {
            EventType = GoalEventsType.GoalSet,
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
    {
        // Raise nut in goal done event to notify that all interaction with goal are complete
        goalEvents.Raise(new GoalEventPayload
        {
            EventType = GoalEventsType.NutInGoalDone,
            Position = payload.CurrentPosition,
            GoalReduction = 0,
            GoalRemove = false,
        });
    }
    #endregion
}
