using UnityEngine;
using Veverka.Characters.Veverka;

/// <summary>
/// undo move character action 
/// </summary>
public class UndoCharacterAction : IUndoableAction
{
    private Character character;
    private Vector2Int from;
    private Vector2Int to;
    private float duration;
    private Direction direction;

    /// <summary>
    /// move character action from current to previous position - constructor
    /// </summary>
    /// <param name="character"> character class or its child</param>
    /// <param name="from">current position</param>
    /// <param name="to">previous position</param>
    /// <param name="direction">direction of movement</param>
    /// <param name="duration">duration of movement in seconds</param>
    public UndoCharacterAction(Character character, Vector2Int from, Vector2Int to, Direction direction, float duration = 0.15f)
    {
        this.character = character;
        this.from = from;
        this.to = to;
        this.duration = duration;
        this.direction = direction;
    }

    /// <summary>
    /// IUndoableAction interface Undo method, executes undo move on character
    /// </summary>
    public void Undo()
    {
        character.UndoMove(from, to, direction, duration);
    }
}
