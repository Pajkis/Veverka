using UnityEngine;

/// <summary>
/// Undo movable tile action
/// </summary>
public class UndoMovableAction : IUndoableAction
{
    private NutTile pushableTile;
    private Vector2Int current;
    private Vector2Int previous;
    private float duration;

    /// <summary>
    /// move movable tile action from current to previous position - constructor
    /// </summary>
    /// <param name="movableTile"> movable tile class or its child</param>
    /// <param name="current">current position</param>
    /// <param name="previous">previous position</param>
    /// <param name="duration">duration of movement in seconds</param>
    public UndoMovableAction(NutTile pushableTile, Vector2Int current, Vector2Int previous, float duration)
    {
        this.pushableTile = pushableTile;
        this.current = current;
        this.previous = previous;
        this.duration = duration;   
    }

    /// <summary>
    /// IUndoableAction interface Undo method, executes undo move on movable tile
    /// </summary>
    public void Undo()
    {
        pushableTile.UndoAction(current, previous, duration);
    }
}
