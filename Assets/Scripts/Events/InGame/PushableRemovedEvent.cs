using UnityEngine;

/// <summary>
/// Event raised when a Nut tile is removed from the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/NutRemovedEvent")]
public class NutRemovedEvent : GameEventSO<NutRemovedPayload> { }
