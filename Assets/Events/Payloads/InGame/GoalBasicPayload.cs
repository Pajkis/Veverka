using UnityEngine;

/// <summary>
/// Payload for set and remove a goal tile from the grid.
/// </summary>
public struct GoalBasicPayload
{
    public Vector2Int Position;
    public GoalType GoalType;
    public GoalTile GoalTile;
    public int GoalReduction;
}
