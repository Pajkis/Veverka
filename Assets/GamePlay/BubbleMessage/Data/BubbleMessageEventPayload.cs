using UnityEngine;

/// <summary>
/// Unified payload for bubble messages.
/// Can be used as sub-payload in Goal/Obstacle/Character payloads or as standalone event payload (future).
/// </summary>
[System.Serializable]
public class BubbleMessageEventPayload
{
    [Header("Message")]
    public BubbleMessageType MessageType;
    public float DisplayTime;

    /// <summary>
    /// Creates bubble message payload with specified type and time
    /// </summary>
    public static BubbleMessageEventPayload Create(BubbleMessageType type, float time)
    {
        return new BubbleMessageEventPayload
        {
            MessageType = type,
            DisplayTime = time
        };
    }

    /// <summary>
    /// Returns a "no message" instance
    /// </summary>
    public static BubbleMessageEventPayload None => new BubbleMessageEventPayload { MessageType = BubbleMessageType.NoMessage };

    /// <summary>
    /// Checks if this message should be displayed
    /// </summary>
    public bool ShouldShow => MessageType != BubbleMessageType.NoMessage;
}
