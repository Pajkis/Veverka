using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all wall-related events.
/// </summary>
[CreateAssetMenu(menuName = "Events/WallEvents")]
public class WallEvents : GameEventSO<WallEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(WallEventPayload payload, WallEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(WallEventPayload payload, params WallEventType[] eventTypes)
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
