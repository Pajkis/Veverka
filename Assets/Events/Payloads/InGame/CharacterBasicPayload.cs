using UnityEngine;

/// <summary>
/// Character basic payload - for register turn records and undo logic
/// </summary>
[System.Serializable]
public struct CharacterBasicPayload
{
    // character data
    public Character Character;

    // position and velocity data
    public Vector2Int Current;
    public Vector2Int Previous;
    public Direction Direction;
    public float Duration;

    // data request and response flags
    public bool RequestData;
    public bool ResponseData;
}