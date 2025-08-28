using UnityEngine;

/// <summary>
/// Event raised when a nut should be pushed
/// </summary>
[CreateAssetMenu(menuName = "Events/PushNutEvent")]
public class PushNutEvent : GameEventSO<PushNutPayload> { }
