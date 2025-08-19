using UnityEngine;

/// <summary>
/// pushable in goal event payload structure
/// </summary>
[System.Serializable]
public struct PushableInGoalPayload
{
    public Vector2Int Position;
    public PushableTile PushableTileType;
    public int GoalReduction;
}
