using UnityEngine;

/// <summary>
/// Scriptable object event used for querying tile information from the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/TileQueryEvent")]
public class TileQueryEvent : GameEventSO<TileQueryPayload> { }
