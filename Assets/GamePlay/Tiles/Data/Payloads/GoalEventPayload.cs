using UnityEngine;

/// <summary>
/// Unified payload for goal events.
/// </summary>
[System.Serializable]
public class GoalEventPayload
{
    public GoalEventsType EventType;

    // Basic goal data
    public Vector2Int Position;
    public GoalType GoalType;
    public GoalTile GoalTile;

    // Goal handling data
    public int GoalReduction;
    public bool GoalRemove;

    //Instantiate goal tile
    public bool InstantiateTile;

    // Message for Veverka to display when goal is reached
    public BubbleMessageType GoalMessage;
    public float GoalMessageTime;
}

