using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Singleton turn recorder that tracks undoable actions and waits for all involved sources to complete
/// before finalizing a turn.
/// </summary>
public class TurnBuilder
{
    // Singleton instance
    private static TurnBuilder instance;
    public static TurnBuilder Instance => instance ??= new TurnBuilder();

    // Private constructor prevents external instantiation
    private TurnBuilder() { }

    private List<IUndoableAction> actions = new();
    private HashSet<string> expectedSources = new();
    private HashSet<string> completedSources = new();
    private bool isActive = true;
    // Callback to be set by GamePlay or UndoManager
    private Action<SingleTurnRecord> onTurnFinalized;

    /// <summary>
    /// Sets the method to be called when a turn is finalized.
    /// </summary>
    public void SetFinalizeCallback(Action<SingleTurnRecord> callback)
    {
        onTurnFinalized = callback;
        isActive = true;
        Debug.Log($"[TurnBuilder] recording is Active");

    }
    /// <summary>
    /// Start new turn - reset all actions and sources, sets active
    /// </summary>
    public void StartNewTurn()
    {
        isActive = true;
        actions.Clear();
        expectedSources.Clear();
        completedSources.Clear();
        Debug.Log("[TurnBuilder] start turn");
    }

    /// <summary>
    /// Register an undoable action to be included in this turn.
    /// </summary>
    public void AddAction(IUndoableAction action)
    {
        if (!isActive) return;
        actions.Add(action);
    }

    /// <summary>
    /// Mark an object (by ID) that will participate in this turn and must call NotifySourceComplete.
    /// </summary>
    public void ExpectSource(string sourceId)
    {
        if (!isActive) return;
        expectedSources.Add(sourceId);
    }

    /// <summary>
    /// Notify that a source has finished its animation/effect.
    /// </summary>
    public void NotifySourceComplete(string sourceId)
    {
        if (!isActive) return;
        completedSources.Add(sourceId);
        TryFinalizeTurn();
    }

    /// <summary>
    /// Tries to finalize the turn if all expected sources are done.
    /// </summary>
    private void TryFinalizeTurn()
    {

        Debug.Log($"[TurnBuilder] Expecting: {string.Join(", ", expectedSources)}");
        Debug.Log($"[TurnBuilder] Completed: {string.Join(", ", completedSources)}");

        if (expectedSources.SetEquals(completedSources))
        {
            var turn = new SingleTurnRecord();
            foreach (var action in actions)
                turn.AddAction(action);

            Debug.Log($"[TurnBuilder] Finalizing turn with {actions.Count} actions");
            onTurnFinalized?.Invoke(turn);

            // Reset for next turn
            actions.Clear();
            expectedSources.Clear();
            completedSources.Clear();
        }
    }

    /// <summary>
    /// Clear everything without saving turn, disables add actions and sources
    /// </summary>
    public void CancelTurn()
    {
        isActive = false;
        actions.Clear();
        expectedSources.Clear();
        completedSources.Clear();
        Debug.Log("[TurnBuilder] Turn cancelled");
    }
}
