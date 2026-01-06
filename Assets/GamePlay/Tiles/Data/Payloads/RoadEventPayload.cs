using UnityEngine;

/// <summary>
/// Unified payload for road events.
/// </summary>
[System.Serializable]
public class RoadEventPayload
{    
    public RoadEventType EventType;

    [Header("Basic Road Data")]
    public Vector2Int Position;
    public RoadType RoadType;
    public TileObject RoadTile;
    public bool InstantiateTile;
}
