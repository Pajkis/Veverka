using UnityEngine;

/// <summary>
/// stone nut tile class - nut tile type that can be pushed into goals or holes
/// </summary>
public class StoneNutTile : NutTile
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
        // remove nut from previous position
        nutRemovedEvent.Raise(new NutBasicPayload
        {
            Position = GridPosition,
        });

        // Check if the nut reached the goal
        TileQueryPayload query = new() { Position = targetPosition };
        tileQueryEvent.Raise(query);
        if (query.TileType != TileType.Goal)
        {
            nutSetEvent.Raise(new NutBasicPayload
            {
                Position = targetPosition,
                NutType = this.nutType,
                NutTile = this,
            });
        }
        else
        {
            // nut enters goal event raise
            nutInGoalEvent.Raise(new NutBasicPayload
            {
                Position = targetPosition,
                NutType = this.nutType,
                NutTile = this,
            });
            Destroy(gameObject);
        }

        GridPosition = targetPosition;

        //Complete turn record
        TurnRecordAddandComplete();
    }
    #endregion  
}

