using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Character moved payload - for register turn records and undo logic
/// </summary>
[System.Serializable]
public struct CharacterMovedPayload
{
    public Character character;
    public Vector2Int current;
    public Vector2Int previous;
    public Direction direction;
    public float duration;
}