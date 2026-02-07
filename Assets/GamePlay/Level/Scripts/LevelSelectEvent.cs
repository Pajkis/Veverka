using UnityEngine;

/// <summary>
/// Event emitted when a level is selected.
/// </summary>
[CreateAssetMenu(menuName = "Level/Events/Select")]
public class LevelSelectEvent : GameEventSO<LevelSelectPayload> { }