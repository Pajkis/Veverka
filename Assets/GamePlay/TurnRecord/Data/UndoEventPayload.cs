using UnityEngine;

[System.Serializable]
public enum UndoEventType
{
    RequestCharacterUndo,
    RequestNutUndo,
    UndoCompleted
}

[System.Serializable]
public struct UndoEventPayload
{
    public UndoEventType EventType;
    public UndoData UndoData;
    public string RequestId; // For tracking completion
}