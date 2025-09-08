using UnityEngine;

/// <summary>
/// Event triggered whenever a character moves.
/// </summary>
[CreateAssetMenu(menuName = "Events/CharacterMovedEvent")]
public class CharacterMovedEvent : GameEventSO<CharacterBasicPayload> { }