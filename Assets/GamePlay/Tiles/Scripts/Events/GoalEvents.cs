using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all goal-related events.
/// </summary>
[CreateAssetMenu(menuName = "Events/GoalEvents")]
public class GoalEvents : GameEventSO<GoalEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(GoalEventPayload payload, GoalEventsType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(GoalEventPayload payload, params GoalEventsType[] eventTypes)
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

