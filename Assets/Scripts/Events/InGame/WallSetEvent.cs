using UnityEngine;

/// <summary>
/// Event raised when a goal tile is placed on the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/WallSetEvent")]
public class WallSetEvent : GameEventSO<WallBasicPayload> { }
