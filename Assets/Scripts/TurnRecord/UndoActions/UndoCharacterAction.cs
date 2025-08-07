using UnityEngine;

/// <summary>
/// undo move character action 
/// </summary>
public class UndoCharacterAction : IUndoableAction
{
    private Character character;
    private Vector2Int current;
    private Vector2Int previous;
    private float duration;
    private Direction direction;

    /// <summary>
    /// move character action from current to previous position - constructor
    /// </summary>
    /// <param name="character"> character class or its child</param>
    /// <param name="current">current position</param>
    /// <param name="previous">previous position</param>
    /// <param name="direction">direction of movement</param>
    /// <param name="duration">duration of movement in seconds</param>
    public UndoCharacterAction(Character character, Vector2Int current, Vector2Int previous, Direction direction, float duration = 0.15f)
    {
        this.character = character;
        this.current = current;
        this.previous = previous;
        this.duration = duration;
        this.direction = direction;
    }

    /// <summary>
    /// IUndoableAction interface Undo method, executes undo move on character
    /// </summary>
    public void Undo()
    {
        character.UndoMove(current, previous, direction, duration);
    }
}
