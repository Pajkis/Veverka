using UnityEngine;

public class GoalTile : StaticTile
{
    #region events
    [SerializeField] protected PushableInGoalEvent pushableInGoalEvent;
    [SerializeField] private GoalSetEvent goalSetEvent;
    [SerializeField] private GoalRemovedEvent goalRemovedEvent;
    #endregion

    #region methods
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
    private void OnEnable()
    {
        pushableInGoalEvent.AddListener(OnPushableInGoal);
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    private void OnDisable()
    {
        pushableInGoalEvent.RemoveListener(OnPushableInGoal);
    }

    /// <summary>
    /// On pushable in goal event settlement
    /// </summary>
    /// <param name="payload"></param>
    private void OnPushableInGoal(PushableInGoalPayload payload)
    {
        if (GridPosition == payload.Position)
        {
            goalRemovedEvent.Raise(new GoalRemovedPayload
            {
                Position = payload.Position,
            });
            Destroy(gameObject);
        }
    }

    #endregion
}
