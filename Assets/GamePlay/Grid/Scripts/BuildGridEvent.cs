using UnityEngine;

/// <summary>
/// Event raised to build the game grid from a level layout.
/// </summary>
[CreateAssetMenu(menuName = "Events/BuildGridEvent")]
public class BuildGridEvent : GameEventSO<TileType[,]> { }