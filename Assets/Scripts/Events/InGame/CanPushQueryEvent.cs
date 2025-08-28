using UnityEngine;

/// <summary>
/// Event raised to query if a nut can be pushed
/// </summary>
[CreateAssetMenu(menuName = "Events/CanPushQueryEvent")]
public class CanPushQueryEvent : GameEventSO<CanPushQueryPayload> { }