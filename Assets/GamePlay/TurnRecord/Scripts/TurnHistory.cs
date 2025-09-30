using System.Collections.Generic;

public class TurnHistory
{
    private readonly Stack<List<UndoData>> history = new();

    public int TurnCount => history.Count;

    public void RegisterTurn(List<UndoData> turnData)
    {
        if (turnData != null && turnData.Count > 0)
        {
            history.Push(new List<UndoData>(turnData)); // Create copy for safety
            DebugLogger.Log(DebugLogCategory.TurnRecord,
                $"Registered turn with {turnData.Count} actions. Total turns: {history.Count}");
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.TurnRecord,
                "Ignored empty or null turn");
        }
    }

    public List<UndoData> GetLastTurn()
    {
        if (history.Count == 0)
        {
            DebugLogger.Log(DebugLogCategory.UndoLogic, "No turns to undo");
            return null;
        }

        var turn = history.Pop();
        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Retrieved turn with {turn.Count} actions. Remaining turns: {history.Count}");
        return turn;
    }

    public void ClearHistory()
    {
        history.Clear();
        DebugLogger.Log(DebugLogCategory.TurnRecord, "Turn history cleared");
    }
}