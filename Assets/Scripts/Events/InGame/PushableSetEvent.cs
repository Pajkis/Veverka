using UnityEngine;

/// <summary>
/// Event raised when a pushable tile is placed on the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/PushableSetEvent")]
public class PushableSetEvent : GameEventSO<PushableSetPayload> { }
