using UnityEngine;

/// <summary>
/// Payload for set and remove a goal tile from the grid.
/// </summary>
public struct GoalBasicPayload
{
    // Position of the goal tile on the grid
    public Vector2Int Position;
    public GoalType GoalType;
    public GoalTile GoalTile;

    // goal handled in the event 
    public int GoalReduction;
    public bool GoalRemove;

    // Message for Veverka to display when goal is reached
    public BubbleMessageType GoalMessage;
    public float GoalMessageTime;

}


