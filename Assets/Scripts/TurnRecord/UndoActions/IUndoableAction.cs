/// <summary>
/// For undoing moves, effects etc.
/// </summary>
public interface IUndoableAction
{
    // Undo move/effect
    void Undo();
}