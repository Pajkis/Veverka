using UnityEngine;

/// <summary>
/// Unified payload for road events.
/// </summary>
[System.Serializable]
public class RoadEventPayload
{
    public RoadEventType EventType;

    // Basic road data
    public Vector2Int Position;
    public RoadType RoadType;
    public TileObject RoadTile;

    public bool InstantiateTile;
}
