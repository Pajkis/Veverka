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

    // Movement data
    public Vector2Int CurrentPosition;
    public Vector2Int PreviousPosition;

    // Push query data
    public Direction Direction;
    public int Distance;
    public float Duration;    
}
