using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all audio related events
/// (initialization, music, sound effects and UI sounds).
/// </summary>
[CreateAssetMenu(menuName = "Events/AudioEvents")]
public class AudioEvents : GameEventSO<AudioEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific audio event type.
    /// </summary>
    public void RaiseForEventType(AudioEventPayload payload, AudioEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple audio event types.
    /// </summary>
    public void RaiseForEventTypes(AudioEventPayload payload, params AudioEventType[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (payload.EventType == type)
            {
                Raise(payload);
                return;
            }
        }
    }
}

