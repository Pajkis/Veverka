using UnityEngine;

/// <summary>
/// Event raised when a goal tile is removed from the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/GoalResolvedEvent")]
public class GoalResolvedEvent : GameEventSO<GoalBasicPayload> { }

/// <summary>
/// Event raised when a goal tile is placed on the grid.
/// </summary>
[CreateAssetMenu(menuName = "Events/GoalSetEvent")]
public class GoalSetEvent : GameEventSO<GoalBasicPayload> { }
