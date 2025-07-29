using UnityEngine;

/// <summary>
/// Removing/setting object tiles during game payload 
/// </summary>
public struct TileObjectAtPayload
{
    public TileObject TileObject;
    public TileType TileType;
    public Vector2Int GridPosition;
    public  GridObjectActionType GridObjectAction;
    public bool IsPushable;
}