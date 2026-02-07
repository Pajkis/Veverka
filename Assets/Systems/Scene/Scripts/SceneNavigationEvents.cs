using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for scene navigation (scene transitions and overlay openings).
/// </summary>
[CreateAssetMenu(menuName = "Systems/Events/SceneNavigation")]
public class SceneNavigationEvents : GameEventSO<SceneNavigationEventPayload>
{
    /// <summary>
    /// Raises the event only for a specific navigation event type.
    /// </summary>
    public void RaiseForEventType(SceneNavigationEventPayload payload, SceneNavigationEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple navigation event types.
    /// </summary>
    public void RaiseForEventTypes(SceneNavigationEventPayload payload, params SceneNavigationEventType[] eventTypes)
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
