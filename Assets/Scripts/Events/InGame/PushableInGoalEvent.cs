using UnityEngine;
/// <summary>
/// Event raised when a pushable object reaches a goal.
/// </summary>
[CreateAssetMenu(menuName = "Events/PushableInGoalEvent")]
public class PushableInGoalEvent : GameEventSO<PushableInGoalPayload> { }