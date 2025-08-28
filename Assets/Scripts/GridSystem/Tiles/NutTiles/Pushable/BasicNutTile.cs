using UnityEngine;

/// <summary>
/// Class for the nut Tile - basic push to goal tile.
/// </summary>
public class BasicNutTile : NutTile
{
    #region methods
    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {        
        base.OnMoveStart();
        playSfxEvent.Raise(SfxType.NutMove);
    }

    /// <summary>
    /// On move complete method
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        // remove pushable from previous position
        pushableRemovedEvent.Raise(new PushableRemovedPayload
        {
            Position = GridPosition,
        });

        // Check if the nut reached the goal
        TileQueryPayload query = new() { Position = targetPosition };
        tileQueryEvent.Raise(query);
        if (!(query.IsGoal))
        {
            pushableSetEvent.Raise(new PushableSetPayload
            {
                Position = targetPosition,
                Pushable = this,
            });
        }
        else
        {
            // nut enters goal event raise
            pushableInGoalEvent.Raise(new PushableInGoalPayload
            {
                Position = targetPosition,
                PushableTileType = this,
               // GoalReduction = 1,
            });

            Destroy(gameObject);            
        }

        GridPosition = targetPosition;

        //Complete turn record
        TurnRecordAddandComplete();
    }
    #endregion
}

