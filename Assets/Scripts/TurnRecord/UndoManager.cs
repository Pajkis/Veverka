using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UndoManager - keeps stack of turn record history
/// </summary>
public class UndoManager
{
    // stack of turn records
    private Stack<TurnRecord> history = new();

    /// <summary>
    /// Register one turn in turn record
    /// </summary>
    /// <param name="turn"></param>
    public void RegisterTurn(TurnRecord turn)
    {
        history.Push(turn);
    }

    /// <summary>
    /// undo one turn from turn record
    /// </summary>
    public void Undo()
    {
        if (history.Count > 0)
        {
            TurnRecord turn = history.Pop();
            turn.Undo();
        }
    }
}