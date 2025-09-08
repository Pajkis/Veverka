using UnityEngine;

/// <summary>
/// Unified ScriptableObject event system for all character-related events
/// Replaces CharacterDataRequestEvent and CharacterMovedEvent
/// </summary>
[CreateAssetMenu(menuName = "Events/CharacterEvents")]
public class CharacterEvents : GameEventSO<CharacterEventPayload>
{
    #region Event Filtering Methods
    /// <summary>
    /// Raises the event only for specific event types
    /// </summary>
    public void RaiseForEventType(CharacterEventPayload payload, CharacterEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for specific character
    /// </summary>
    public void RaiseForCharacter(CharacterEventPayload payload, Character character)
    {
        if (payload.Character == character)
        {
            Raise(payload);
        }
    }

    /// <summary>
    /// Raises the event only for multiple event types
    /// </summary>
    public void RaiseForEventTypes(CharacterEventPayload payload, params CharacterEventType[] eventTypes)
    {
        foreach (var eventType in eventTypes)
        {
            if (payload.EventType == eventType)
            {
                Raise(payload);
                return;
            }
        }
    }
    #endregion
}