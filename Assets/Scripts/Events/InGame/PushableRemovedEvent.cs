using UnityEngine;

/// <summary>
/// Event raised when a pushable tile is removed from the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/PushableRemovedEvent")]
public class PushableRemovedEvent : GameEventSO<PushableRemovedPayload> { }
