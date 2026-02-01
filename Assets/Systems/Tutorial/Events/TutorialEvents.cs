using UnityEngine;

/// <summary>
/// ScriptableObject event channel for tutorial system communication
/// Allows overlay (UI) to communicate with persistent TutorialGrid
/// </summary>
[CreateAssetMenu(menuName = "Tutorial/Events/Main")]
public class TutorialEvents : ScriptableObject
{
    private System.Action<TutorialEventPayload> listeners;

    /// <summary>
    /// Raise tutorial event
    /// </summary>
    public void Raise(TutorialEventPayload payload)
    {
        listeners?.Invoke(payload);
    }

    /// <summary>
    /// Add listener for tutorial events
    /// </summary>
    public void AddListener(System.Action<TutorialEventPayload> listener)
    {
        listeners += listener;
    }

    /// <summary>
    /// Remove listener for tutorial events
    /// </summary>
    public void RemoveListener(System.Action<TutorialEventPayload> listener)
    {
        listeners -= listener;
    }
}
