using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// scriptable object event without parameters(payload)
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Event")]
public class GameEventSOVoid : ScriptableObject
{
    [SerializeField] private UnityEvent onEvent = new();

    /// <summary>
    /// Invoke event
    /// </summary>
    public void Raise()
    {
        onEvent?.Invoke();
    }
    /// <summary>
    /// add listnere to the event
    /// </summary>
    /// <param name="listener">listener to add</param>
    public void AddListener(UnityAction listener)
    {
        onEvent.AddListener(listener);
    }
    /// <summary>
    /// remove listener from event
    /// </summary>
    /// <param name="listener">listener to remove</param>
    public void RemoveListener(UnityAction listener)
    {
        onEvent.RemoveListener(listener);
    }
}