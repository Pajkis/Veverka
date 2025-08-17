using UnityEngine;

public class GoalTile : StaticTile
{
    #region events
    [SerializeField] protected PushableInGoalEvent pushableInGoalEvent;
    [SerializeField] private GoalSetEvent goalSetEvent;
    [SerializeField] private GoalRemovedEvent goalRemovedEvent;
    [SerializeField] private PlaySfxEvent playSfxEvent;
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
        // Check if the pushable is in the goal position
        if (GridPosition == payload.Position)
        {
            // Play goal reached sound effect
            playSfxEvent.Raise(SfxType.GoalReached);
            // Raise goal reached event
            goalRemovedEvent.Raise(new GoalRemovedPayload
            {
                Position = payload.Position,
            });
            Destroy(gameObject);
        }
    }

    #endregion
}
