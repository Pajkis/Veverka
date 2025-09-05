using UnityEngine;

/// <summary>
/// Basic payload for nut-related events.
/// </summary>
public struct WallBasicPayload
{
    public Vector2Int Position;
    public WallType WallType;  
    public TileObject WallTile;
    public bool instantiateTile;
}

