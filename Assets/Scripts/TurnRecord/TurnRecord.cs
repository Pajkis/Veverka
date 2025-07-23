using System.Collections.Generic;
/// <summary>
/// keeps records of actions in a one turn and executes undos in current turn
/// </summary>
public class TurnRecord
{
    readonly List<IUndoableAction> actions = new();

    /// <summary>
    /// Add new action into list of undoable actions
    /// </summary>
    /// <param name="action"></param>
    public void AddAction(IUndoableAction action)
    {
        actions.Add(action);
    }

    /// <summary>
    /// Undo all moves, effects etc. in all undoable components
    /// </summary>
    public void Undo()
    {
        for (int i = actions.Count - 1; i >= 0; i--)
            actions[i].Undo();
    }
}