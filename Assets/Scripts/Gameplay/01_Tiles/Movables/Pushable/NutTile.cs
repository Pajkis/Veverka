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

    /// <summary>
    /// On move complete method
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        GameGrid.Instance.Pushables.Remove(gridPosition);

        if (!GameGrid.Instance.IsGoalAt(targetPosition))
        {
            GameGrid.Instance.Pushables[targetPosition] = this;
            GameGrid.Instance.SetTileType(targetPosition, tileType);
        }
        else
        {

            GameGrid.Instance.RemoveTileObject(GameGrid.Instance.Goals, targetPosition);
            Destroy(gameObject);
            GameGrid.Instance.SetTileType(targetPosition, TileType.Empty);
            unityEvents[EventEnum.NutInGoalEvent]?.Invoke(1);
        }

        gridPosition = targetPosition;
        isMoving = false;
    }
}
   
