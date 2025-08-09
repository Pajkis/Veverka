using UnityEngine;

/// <summary>
/// Payload for setting a goal tile on the grid.
/// </summary>
public struct GoalSetPayload
{
    public Vector2Int Position;
    public GoalTile Goal;
}
