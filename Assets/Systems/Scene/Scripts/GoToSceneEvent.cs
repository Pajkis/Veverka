using UnityEngine;

/// <summary>
/// Event raised to request a scene change.
/// </summary>
[CreateAssetMenu(menuName = "Events/GoToSceneEvent")]
public class GoToSceneEvent : GameEventSO<SceneType> { }
