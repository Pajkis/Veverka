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

    [Header("Replacement Creation")]
    public bool GoalRemove; 
    public bool CreateReplacement; 
    public TileType ReplacementType;
    public RoadType ReplacementRoadType;
    public WallType ReplacementWallType;
    public GoalType ReplacementGoalType;

    //Instantiate replacement tile
    public bool InstantiateTile = true;

    [Header("Special Effects")]
    public bool TriggerSplash;

    [Header("UI Messages")]
    public BubbleMessageType GoalMessage;
    public float GoalMessageTime;
    public bool ShowBubbleMessage;

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
        if (CreateReplacement && ReplacementType == TileType.Empty)
        {
            DebugLogger.LogWarning(DebugLogCategory.GoalSystem, "GoalEventPayload creating replacement but ReplacementType is Empty");
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
        if (ShowBubbleMessage)
            description += $"Show {GoalMessage}, ";
        if (ScoreValue > 0)
            description += $"Score: {ScoreValue}";

        return description.TrimEnd(' ', ',');
    }
}


