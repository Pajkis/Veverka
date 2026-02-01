using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all setting-related events.
/// </summary>
[CreateAssetMenu(menuName = "Systems/Events/Settings")]
public class SettingEvents : GameEventSO<SettingEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific event type.
    /// </summary>
    public void RaiseForEventType(SettingEventPayload payload, SettingsEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types.
    /// </summary>
    public void RaiseForEventTypes(SettingEventPayload payload, params SettingsEventType[] eventTypes)
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
