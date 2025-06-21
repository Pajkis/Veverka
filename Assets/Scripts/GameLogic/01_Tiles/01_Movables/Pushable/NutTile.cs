using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Class for the nut Tile
/// 
/// </summary>
public class NutTile : PushableTile
{
    #region init methods
    /// <summary>
    /// Initialize tile type on grid position
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        // Add invokers for events
        if (!unityEvents.ContainsKey(EventEnum.NutInGoalEvent))
        {
            unityEvents.Add(EventEnum.NutInGoalEvent, new UnityEvent<int>());
        }
        EventManager.AddInvoker(EventEnum.NutInGoalEvent, this);

        this.tileType = tileType;
        this.gridPosition = gridPosition;
    }

    #endregion

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
            // destroy goal nad nut
            GameGrid.Instance.RemoveTileObject(GameGrid.Instance.Goals, targetPosition);
            Destroy(gameObject);
            GameGrid.Instance.SetTileType(targetPosition, TileType.Empty);

            AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.GoalReached);
            unityEvents[EventEnum.NutInGoalEvent]?.Invoke(1);            
        }

        gridPosition = targetPosition;
        isMoving = false;
    }
    #endregion
}

