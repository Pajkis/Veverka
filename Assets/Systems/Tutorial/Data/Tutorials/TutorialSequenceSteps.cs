using UnityEngine;

/// <summary>
/// Single movement step in a MovementSequence.
/// Represents a direction to move/turn and how many times to repeat that action.
/// </summary>
[System.Serializable]
public struct MovementStep
{
    [Tooltip("Direction to move/turn")]
    public Direction Direction;

    [Tooltip("Number of direction actions to perform (e.g., 5 = call DirectionEvent 5 times)")]
    [Min(1)]
    public int RepeatCount;
}

/// <summary>
/// Single message step in a MessageSequence.
/// Represents a text message to display and its duration.
/// </summary>
[System.Serializable]
public struct MessageStep
{
    [Tooltip("Text message to display")]
    [TextArea(2, 6)]
    public string MessageText;

    [Tooltip("Duration to display message (0 = use default)")]
    [Min(0)]
    public float DisplayTime;
}

/// <summary>
/// Single image action in an ImageSequence.
/// Currently used for single images, but designed to be extendable to lists in the future.
/// </summary>
[System.Serializable]
public struct ImageStep
{
    [Tooltip("Image sprite to display")]
    public Sprite Image;

    [Tooltip("Action to perform with the image")]
    public ActionImageType ActionType;

    [Tooltip("Display duration (0 = use default)")]
    [Min(0)]
    public float DisplayTime;

    [Tooltip("Execute this image action simultaneously with a message")]
    public bool ExecuteWithMessage;
}
