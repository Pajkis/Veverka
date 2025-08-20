using UnityEngine;

/// <summary>
/// Character moved payload - for register turn records and undo logic
/// </summary>
[System.Serializable]
public struct MovableMovedPayload
{
    public MovableTile movable;
    public Vector2Int current;
    public Vector2Int previous;
    public float duration;
}
