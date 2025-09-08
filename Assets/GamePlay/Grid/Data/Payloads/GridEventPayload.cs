using UnityEngine;

/// <summary>
/// Unified payload for grid events.
/// </summary>
[System.Serializable]
public struct GridEventPayload
{
    public GridEventType EventType;
    public TileType[,] Grid;
    public TileQueryPayload Query;
}
