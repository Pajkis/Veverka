using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all nut-related events.
/// </summary>
[CreateAssetMenu(menuName = "Events/NutEvents")]
public class NutEvents : GameEventSO<NutEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(NutEventPayload payload, NutEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(NutEventPayload payload, params NutEventType[] eventTypes)
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
