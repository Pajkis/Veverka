using UnityEngine;

/// <summary>
/// Payload for pushing nut objects
/// </summary>
[System.Serializable]
public struct PushNutPayload
{
    public Vector2Int Position;
    public Direction Direction;
    public int Distance;
    public float Duration;
}

