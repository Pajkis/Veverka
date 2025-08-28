using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GoalTile : StaticTile
{
    #region events
    [SerializeField] protected NutInGoalEvent nutInGoalEvent;
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
        nutInGoalEvent.AddListener(OnPushableInGoal);
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    private void OnDisable()
    {
        nutInGoalEvent.RemoveListener(OnPushableInGoal);
    }

    /// <summary>
    /// On pushable in goal event settlement
    /// </summary>
    /// <param name="payload"></param>
      private void OnPushableInGoal(NutBasicPayload payload)
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
                GoalReduction = 1,
            });
            Destroy(gameObject);
        }
    }

    #endregion
}
