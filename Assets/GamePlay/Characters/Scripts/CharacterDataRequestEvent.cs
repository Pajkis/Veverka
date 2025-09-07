using UnityEngine;

/// <summary>
/// Event triggered whenever a character moves.
/// </summary>
[CreateAssetMenu(menuName = "Events/CharacterDataRequestEvent")]
public class CharacterDataRequestEvent : GameEventSO<CharacterBasicPayload> { }