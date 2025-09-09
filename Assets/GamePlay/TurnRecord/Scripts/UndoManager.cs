using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UndoManager - keeps stack of turn record history
/// </summary>
public class UndoManager
{
    // stack of turn records
    private Stack<SingleTurnRecord> history = new();
    private readonly AudioEvents audioEvents;

    public UndoManager(AudioEvents audioEvents)
    {
        this.audioEvents = audioEvents;
    }

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
    public bool Undo()
    {
        if (history.Count == 0)
        {
            Debug.Log("[UndoManager] Nothing to undo.");
            return false;
        }

        audioEvents?.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Undo });
        var turn = history.Pop();
        turn.Undo();
        return true;
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