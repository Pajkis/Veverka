using UnityEngine;

/// <summary>
/// Unified payload for wall events.
/// </summary>
[System.Serializable]
public class WallEventPayload
{
    public WallEventType EventType;

    // Basic wall data
    public Vector2Int Position;
    public WallType WallType;
    public TileObject WallTile;

    public bool InstantiateTile;
}
