using UnityEngine;

/// <summary>
/// goal tile abstract class
/// </summary>
public abstract class GoalTile : TileObject
{
    #region events
    [SerializeField] protected NutInGoalEvent nutInGoalEvent;
    [SerializeField] protected GoalSetEvent goalSetEvent;
    [SerializeField] protected GoalRemovedEvent goalRemovedEvent;
    #endregion

    #region Init
    /// <summary>
    /// Init goals and put the into dictionary in GameGrid
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        base.Init(tileType, gridPosition);
        goalSetEvent.Raise(new GoalSetPayload
        {
            Position = gridPosition,
            Goal = this,
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
