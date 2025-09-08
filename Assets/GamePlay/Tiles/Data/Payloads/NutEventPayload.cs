using UnityEngine;

/// <summary>
/// Unified payload for nut events.
/// </summary>
[System.Serializable]
public class NutEventPayload
{
    public NutEventType EventType;

    // Basic nut data
    public Vector2Int Position;
    public NutType NutType;
    public NutTile NutTile;

    // Push query data
    public Direction Direction;
    public int Distance;
    public float Duration;
    public bool CanBePushed;
}
