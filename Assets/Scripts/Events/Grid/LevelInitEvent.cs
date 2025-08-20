using UnityEngine;

/// <summary>
/// Event fired when a level begins initialization.
/// </summary>
[CreateAssetMenu(menuName = "Events/LevelInitEvent")]
public class LevelInitEvent : GameEventSO<LevelInitPayload> { }