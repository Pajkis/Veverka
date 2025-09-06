using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Scriptable object game events with payload
/// </summary>
/// <typeparam name="T">event parameters</typeparam>
public class GameEventSO<T> : ScriptableObject
{

    [SerializeField] private UnityEvent<T> onEvent = new();

    /// <summary>
    /// Invoke event with payload
    /// </summary>
    /// <param name="payload"></param>
    public void Raise(T payload)
    { 
       onEvent?.Invoke(payload);
    }

    /// <summary>
    /// add listener to event
    /// </summary>
    /// <param name="listener"> listener to add for event</param>
    public void AddListener(UnityAction<T> listener)
    { 
       onEvent.AddListener(listener);
    }

    /// <summary>
    /// remover listener for event
    /// </summary>
    /// <param name="listener">listener to remove from event</param>
    public void RemoveListener(UnityAction<T> listener)
    { 
        onEvent.RemoveListener(listener);
    }

}
