using UnityEngine;

/// <summary>
/// Event emitted when a level is selected.
/// </summary>
[CreateAssetMenu(menuName = "Events/LevelSelectEvent")]
public class LevelSelectEvent : GameEventSO<LevelSelectPayload> { }