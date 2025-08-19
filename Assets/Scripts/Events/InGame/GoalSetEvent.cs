using UnityEngine;

/// <summary>
/// Event raised when a goal tile is placed on the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/GoalSetEvent")]
public class GoalSetEvent : GameEventSO<GoalSetPayload> { }
