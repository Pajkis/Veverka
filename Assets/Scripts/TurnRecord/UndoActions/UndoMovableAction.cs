using UnityEngine;

/// <summary>
/// Undo movable tile action
/// </summary>
public class UndoMovableAction : IUndoableAction
{
    private MovableTile movableTile;
    private Vector2Int from;
    private Vector2Int to;
    private float duration;

    /// <summary>
    /// move movable tile action from current to previous position - constructor
    /// </summary>
    /// <param name="movableTile"> movable tile class or its child</param>
    /// <param name="from">current position</param>
    /// <param name="to">previous position</param>
    /// <param name="direction">direction of movement</param>
    /// <param name="duration">duration of movement in seconds</param>
    public UndoMovableAction(MovableTile movableTile, Vector2Int from, Vector2Int to, float duration = 0.15f)
    {
        this.movableTile = movableTile;
        this.from = from;
        this.to = to;
        this.duration = duration;   
    }

    /// <summary>
    /// IUndoableAction interface Undo method, executes undo move on movable tile
    /// </summary>
    public void Undo()
    {
        movableTile.UndoAction(from, to, duration);
    }
}
