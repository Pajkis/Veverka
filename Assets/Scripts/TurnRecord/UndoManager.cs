using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UndoManager - keeps stack of turn record history
/// </summary>
public class UndoManager
{
    // stack of turn records
    private Stack<SingleTurnRecord> history = new();

    /// <summary>
    /// Register one turn in turn record
    /// </summary>
    /// <param name="turn"></param>
    public void RegisterTurn(SingleTurnRecord turn)
    {
        if (turn != null && turn.ActionCount > 0)
        {
            Debug.Log($"[UndoManager] Registered turn with {turn.ActionCount} actions");
            history.Push(turn);
        }
        else 
        {
            Debug.Log("[UndoManager] Ignored empty or null turn");
        }
    }

    /// <summary>
    /// undo one turn from turn record
    /// </summary>
    public void Undo()
    {
        if (history.Count == 0)
        {
            Debug.Log("[UndoManager] Nothing to undo.");
            return;
        }

        var turn = history.Pop();
        turn.Undo();
    }

    /// <summary>
    /// Clear history in stack
    /// </summary>
    public void ClearHistory()
    { 
        history.Clear();
        Debug.Log("[UndoManager] turn history cleared");
    }
}