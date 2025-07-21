using UnityEngine;
using UnityEngine.Events;
using Veverka.GridSystem.GameGrid;

/// <summary>
/// Class for the nut Tile
/// 
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
        GameGrid.Instance.RemovePushableAt(gridPosition);      

        //update dictionary
        if (!GameGrid.Instance.IsGoalAt(targetPosition))
        {
            GameGrid.Instance.SetPushableAt(targetPosition, this);
            GameGrid.Instance.SetTileType(targetPosition, tileType);
        }
        else
        {
            // nut enters goal event raise
            pushableInGoalEvent.Raise(new PushableInGoalPayload
            {
                Position = targetPosition,
                PushableTileType = this,
                goalReduction = 1,
            });
           
            GameGrid.Instance.SetTileType(targetPosition, TileType.Empty);
            Destroy(gameObject);           

            AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.GoalReached);            
        }

        gridPosition = targetPosition;       
    }
    #endregion
}

