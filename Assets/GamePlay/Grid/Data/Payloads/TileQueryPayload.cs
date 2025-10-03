using UnityEngine;

/// <summary>
/// Payload used for querying information about a tile on the grid.
/// GameGrid will fill in the result fields synchronously when the event is raised.
/// </summary>
[System.Serializable]
public class TileQueryPayload
{
    // position to query
    public Vector2Int Position;

    // results populated by GameGrid
    public bool IsWalkable;
    public bool IsPushable;
    public bool IsInGrid;

    public TileType TileType;
    public GoalType GoalType;
    public ObstacleType ObstacleType;
    public NutType NutType;
    public RoadType RoadType;
}
