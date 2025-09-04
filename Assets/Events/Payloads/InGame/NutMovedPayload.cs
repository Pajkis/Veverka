using UnityEngine;

/// <summary>
/// Nut moved payload - for register turn records and undo logic
/// </summary>
[System.Serializable]
public struct NutMovedPayload
{
    public NutType NutType;
    public NutTile NutTile;
    public Vector2Int Current;
    public Vector2Int Previous;
    public float Duration;
}
