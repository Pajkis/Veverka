using UnityEngine;

/// <summary>
/// goal tile abstract class
/// </summary>
public abstract class GoalTile : TileObject
{
    protected GoalType goalType;

    #region events
    [SerializeField] protected NutInGoalEvent nutInGoalEvent;
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
        nutInGoalEvent.AddListener(OnNutInGoal);
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    protected void OnDisable()
    {
        nutInGoalEvent.RemoveListener(OnNutInGoal);
    }
    #endregion

    #region methods
    /// <summary>
    /// Recaction on nut reaching the goal
    /// </summary>
    /// <param name="payload"></param>
    protected virtual void OnNutInGoal(NutBasicPayload payload)
    { }
    #endregion
}
