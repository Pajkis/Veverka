using UnityEngine;

/// <summary>
/// Unified payload for obstacle events.
/// </summary>
[System.Serializable]
public class ObstacleEventPayload
{
    public ObstacleEventsType EventType;

    [Header("Basic Obstacle Data")]
    public Vector2Int Position;
    public ObstacleType ObstacleType;
    public TileObject ObstacleTile;
    public bool InstantiateTile;

    // Removal & Replacement (similar to GoalEventPayload)
    [Header("Replacement Creation")]
    public bool ObstacleRemove;
    public bool CreateReplacement;
    public TileType ReplacementType;
    public ObstacleType ReplacementObstacleType;
    public RoadType ReplacementRoadType;
    public GoalType ReplacementGoalType;

    [Header("Bubble Messages")]
    public BubbleMessageType ObstacleMessage;
    public float ObstacleMessageTime;
    public bool ShowBubbleMessage;
}
