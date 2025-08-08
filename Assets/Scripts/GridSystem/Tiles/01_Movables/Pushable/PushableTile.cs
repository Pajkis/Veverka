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
        Vector2Int targetPosition = GridUtils.GetPositionInDir(GridPosition, direction);
        if (!GameGrid.Instance.IsInGrid(targetPosition)) return false;

        return GameGrid.Instance.IsWalkableAt(targetPosition);
    }
    #endregion

    #region Methods   

    /// <summary>
    /// Executes base undo actions and updates game grid
    /// </summary>
    /// <param name="current"></param>
    /// <param name="previous"></param>
    /// <param name="duration"></param>
    public override void UndoAction(Vector2Int current, Vector2Int previous, float duration = 0.15f)
    { 
        base.UndoAction(current, previous, duration);

        GameGrid.Instance.RemovePushableAt(current);
        GameGrid.Instance.SetTileType(current, TileType.Empty);
        GameGrid.Instance.SetPushableAt(previous, this);
        GameGrid.Instance.SetTileType(previous, PosTileType);
    }


    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        // Clear old grid tile, remove from dictionary       
        GameGrid.Instance.SetTileType(GridPosition, TileType.Empty);
    }

    /// <summary>
    /// On move complete action 
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        GameGrid.Instance.RemovePushableAt(GridPosition);
        GameGrid.Instance.SetTileType(GridPosition, TileType.Empty);
        GameGrid.Instance.SetPushableAt(targetPosition, this);
        GameGrid.Instance.SetTileType(targetPosition, PosTileType);

        GridPosition = targetPosition;
        //Complete turn record
        TurnRecordAddandComplete();
    }

    #endregion
}

