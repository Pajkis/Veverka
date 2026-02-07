using UnityEngine;

/// <summary>
/// ScriptableObject event system for popout image events
/// </summary>
[CreateAssetMenu(menuName = "Gameplay/Events/PopoutImage")]
public class PopoutImageEvents : GameEventSO<PopoutImageEventPayload>
{
    // Inherits all functionality from GameEventSO<T>
    // No additional methods needed - keeps it simple like GoalEvents
}
