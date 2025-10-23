using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all obstacle-related events.
/// </summary>
[CreateAssetMenu(menuName = "Events/ObstacleEvents")]
public class ObstacleEvents : GameEventSO<ObstacleEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(ObstacleEventPayload payload, ObstacleEventsType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(ObstacleEventPayload payload, params ObstacleEventsType[] eventTypes)
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
