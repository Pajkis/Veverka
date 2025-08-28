using UnityEngine;

public class HoleTile : StaticTile
{
    #region events
    [SerializeField] protected NutInGoalEvent nutInGoalEvent;
 //   [SerializeField] private GoalSetEvent goalSetEvent;
    [SerializeField] private GoalRemovedEvent goalRemovedEvent;  
    #endregion

    #region methods
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        base.Init(tileType, gridPosition);
       
        //goalSetEvent.Raise(new GoalSetPayload
        //{
        //    Position = gridPosition,
        //    Goal = this,
        //});
    }

    /// <summary>
    /// On enable add listener
    /// </summary>
    private void OnEnable()
    {
        nutInGoalEvent.AddListener(OnPushableInHole);
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    private void OnDisable()
    {
        nutInGoalEvent.RemoveListener(OnPushableInHole);
    }

    /// <summary>
    /// On pushable in goal event settlement
    /// </summary>
    /// <param name="payload"></param>
      private void OnPushableInHole(NutBasicPayload payload)
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
