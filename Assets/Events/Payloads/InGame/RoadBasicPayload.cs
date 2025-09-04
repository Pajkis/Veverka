using UnityEngine;

/// <summary>
/// Basic payload for road tile operations.
/// </summary>
public struct RoadBasicPayload
{
    public Vector2Int Position;
    public RoadType RoadType;
    public TileObject RoadTile;
    public bool instantiateTile;
}

