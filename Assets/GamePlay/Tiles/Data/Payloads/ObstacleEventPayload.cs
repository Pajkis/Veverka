using UnityEngine;

/// <summary>
/// Unified payload for obstacle events.
/// </summary>
[System.Serializable]
public class ObstacleEventPayload
{
    public ObstacleEventsType EventType;

    // Basic obstacle data
    public Vector2Int Position;
    public ObstacleType ObstacleType;
    public TileObject ObstacleTile;

    // Removal & Replacement (similar to GoalEventPayload)
    public bool ObstacleRemove;
    public bool CreateReplacement;
    public TileType ReplacementType;
    public ObstacleType ReplacementObstacleType;
    public RoadType ReplacementRoadType;
    public GoalType ReplacementGoalType;

    // Messages
    public BubbleMessageType ObstacleMessage;
    public float ObstacleMessageTime;

    // Legacy/compatibility
    public bool InstantiateTile;
}
