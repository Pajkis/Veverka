using UnityEngine;
using Veverka.GridSystem.GameGrid;

public class PushableTile : MovableTile
{

    #region fields
    [SerializeField] 
    protected PushableInGoalEvent pushableInGoalEvent;
    #endregion


    #region pushable methods
    /// <summary>
    /// Checks, whether tile can be pushed in the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CanBePushed(Direction direction)
    {
        Vector2Int targetPosition = GridUtils.GetPositionInDir(gridPosition, direction);
        if (!GameGrid.Instance.IsInGrid(targetPosition)) return false;

        return GameGrid.Instance.IsWalkableAt(targetPosition);
    }
    #endregion

    #region Methods   

    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        // Clear old grid tile, remove from dictionary       
        GameGrid.Instance.SetTileType(gridPosition, TileType.Empty);        
    }

    /// <summary>
    /// On move complete action 
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        GameGrid.Instance.RemovePushableAt(gridPosition);
        GameGrid.Instance.SetPushableAt(targetPosition, this);
        GameGrid.Instance.SetTileType(targetPosition, tileType);
       
        gridPosition = targetPosition;       
    }

    #endregion
}

