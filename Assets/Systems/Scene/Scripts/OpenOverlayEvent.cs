using UnityEngine;

/// <summary>
/// Event raised to request opening a UI overlay.
/// </summary>
[CreateAssetMenu(menuName = "Events/OpenOverlayEvent")]
public class OpenOverlayEvent : GameEventSO<OverlayType> { }
