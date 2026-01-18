using UnityEngine;

/// <summary>
/// ScriptableObject event for TurnControl state changes.
/// Used to notify listeners when a turn starts or completes.
/// </summary>
[CreateAssetMenu(menuName = "Events/TurnControlEvents")]
public class TurnControlEvents : GameEventSO<TurnControlEventPayload>
{
}
