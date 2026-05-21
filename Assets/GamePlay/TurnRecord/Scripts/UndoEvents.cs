using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Events/Undo")]
public class UndoEvents : GameEventSO<UndoEventPayload>
{
    public void RaiseForEventType(UndoEventPayload payload, UndoEventType eventType)
    {
        if (payload.EventType == eventType)
        {
            Raise(payload);
        }
    }

    public void RaiseForEventTypes(UndoEventPayload payload, params UndoEventType[] eventTypes)
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