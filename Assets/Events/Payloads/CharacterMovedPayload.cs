using UnityEngine;

/// <summary>
/// Character moved payload - for register turn records and undo logic
/// </summary>
[System.Serializable]
public struct CharacterMovedPayload
{
    public Character character;
    public Vector2Int from;
    public Vector2Int to;
    public Direction direction;
}