using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all road-related events.
/// </summary>
[CreateAssetMenu(menuName = "Gameplay/Events/Road")]
public class RoadEvents : GameEventSO<RoadEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(RoadEventPayload payload, RoadEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(RoadEventPayload payload, params RoadEventType[] eventTypes)
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
