using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all grid-related events.
/// </summary>
[CreateAssetMenu(menuName = "Gameplay/Events/Grid")]
public class GridEvents : GameEventSO<GridEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(GridEventPayload payload, GridEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(GridEventPayload payload, params GridEventType[] eventTypes)
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
