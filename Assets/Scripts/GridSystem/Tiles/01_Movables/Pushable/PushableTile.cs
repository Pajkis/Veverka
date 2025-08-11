using UnityEngine;
using Veverka.GridSystem.GameGrid;

public class PushableTile : MovableTile
{

    #region fields
    [SerializeField] 
    protected PushableInGoalEvent pushableInGoalEvent;
    [SerializeField]
    protected PushableSetEvent pushableSetEvent;
    [SerializeField]
    protected PushableRemovedEvent pushableRemovedEvent;

    #endregion


    #region Initialization
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        base.Init(tileType, gridPosition);
        pushableSetEvent.Raise(new PushableSetPayload
        {
            Position = gridPosition,
            Pushable = this,
        });
    }
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

        TileQueryPayload query = new() { Position = targetPosition };
        tileQueryEvent.Raise(query);
        if (!query.IsInGrid) return false;
        return query.IsWalkable;
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

        pushableRemovedEvent.Raise(new PushableRemovedPayload
        {         
            Position = current
        });

        pushableSetEvent.Raise(new PushableSetPayload
        {
            Pushable = this,
            Position = previous
        });
    }


    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        // Clear old grid tile, remove from dictionary       
        pushableRemovedEvent.Raise(new PushableRemovedPayload
        {            
            Position = GridPosition
        });
    }

    /// <summary>
    /// On move complete action 
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        pushableSetEvent.Raise(new PushableSetPayload
        {
            Pushable = this,
            Position = targetPosition
        });

        GridPosition = targetPosition;
        //Complete turn record
        TurnRecordAddandComplete();
    }

    #endregion
}

