using UnityEngine;

/// <summary>
/// Comprehensive payload for goal events that handles all goal interactions in a single transaction.
/// This replaces multiple separate events with one complete transaction.
/// </summary>
[System.Serializable]
public class GoalEventPayload
{
    [Header("Basic Goal Data")]
    public GoalEventsType EventType;
    public Vector2Int Position;
    public GoalType GoalType;
    public GoalTile GoalTile;
    public bool InstantiateTile; // instinatiate goal tile during runtime

    [Header("Replacement Creation")]
    public bool GoalRemove;
    public bool CreateReplacement; // create remplacement tile after goal reach
    public TileType ReplacementType;
    public RoadType ReplacementRoadType;
    public ObstacleType ReplacementObstacleType;
    public GoalType ReplacementGoalType;
    
    [Header("Special Effects")]
    public bool TriggerSplash;

    [Header("UI Effects")]
    public BubbleMessageEventPayload BubbleMessage;
    public PopoutImageEventPayload PopoutImage;

    [Header("Gameplay Impact")]
    public int ScoreValue; 

    [Header("Context Information")]
    public NutType TriggeringNut;

    /// <summary>
    /// Validates the payload data for consistency
    /// </summary>
    public bool IsValid()
    {
        // If creating replacement, should have valid replacement type
        if (CreateReplacement && ReplacementType == TileType.ErrorTile)
        {
            DebugLogger.LogWarning(DebugLogCategory.GoalSystem, "GoalEventPayload creating replacement but ReplacementType is ErrorTile");
        }

        return true;
    }

    /// <summary>
    /// Returns a human-readable description of this transaction
    /// </summary>
    public override string ToString()
    {
        var description = $"GoalEvent {EventType} at {Position}: ";

        if (GoalRemove)
            description += "Remove goal, ";
        if (CreateReplacement)
            description += $"Create {ReplacementType}, ";
        if (TriggerSplash)
            description += "Trigger splash, ";
        if (BubbleMessage != null && BubbleMessage.ShouldShow)
            description += $"Show {BubbleMessage.MessageType}, ";
        if (PopoutImage != null && PopoutImage.ShouldShow)
            description += $"Popout {PopoutImage.ImageType}, ";
        if (ScoreValue > 0)
            description += $"Score: {ScoreValue}";

        return description.TrimEnd(' ', ',');
    }
}


