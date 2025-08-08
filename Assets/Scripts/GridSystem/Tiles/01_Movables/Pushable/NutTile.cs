using UnityEngine;
using Veverka.GridSystem.GameGrid;

/// <summary>
/// Class for the nut Tile - basic push to goal tile.
/// </summary>
public class NutTile : PushableTile
{   
    #region methods
    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.NutMove);
        base.OnMoveStart();
    }

    /// <summary>
    /// On move complete method
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        // Remove pushable at grid position and set it to Empty tile
     //   GameGrid.Instance.RemovePushableAt(gridPosition);
       // GameGrid.Instance.SetTileType(gridPosition, TileType.Empty);
        tileObjectAt.Raise(new TileObjectAtPayload
        {
            GridPosition = GridPosition,
            TileType = TileType.Empty,
            GridObjectAction = GridObjectActionType.ReplaceObject,
            TileObject = this,
            IsPushable = true,
        });


        // The nut does not reach goal
        if (!GameGrid.Instance.IsGoalAt(targetPosition))
        {
        //    GameGrid.Instance.SetPushableAt(targetPosition, this);
          //  GameGrid.Instance.SetTileType(targetPosition, tileType);
            tileObjectAt.Raise(new TileObjectAtPayload
            {
                GridPosition = targetPosition,
                TileType = PosTileType,
                GridObjectAction = GridObjectActionType.SetObject,
                TileObject = this,
                IsPushable = true,
            });
        }
        else
        {
            // nut enters goal event raise
            pushableInGoalEvent.Raise(new PushableInGoalPayload
            {
                Position = targetPosition,
                PushableTileType = this,
                GoalReduction = 1,
            });
           
            // replace goal with empty tile
          //  GameGrid.Instance.SetTileType(targetPosition, TileType.Empty);
            tileObjectAt.Raise(new TileObjectAtPayload
            {
                GridPosition = targetPosition,
                TileType = TileType.Empty,
                GridObjectAction = GridObjectActionType.SetObject,
                TileObject = null,
                IsPushable = false,
            });
            Destroy(gameObject);           

            AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.GoalReached);            
        }

        GridPosition = targetPosition;

        //Complete turn record
        TurnRecordAddandComplete();
    }
    #endregion
}

