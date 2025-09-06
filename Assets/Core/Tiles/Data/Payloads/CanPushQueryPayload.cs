using UnityEngine;

/// <summary>
/// Payload for querying if a nut can be pushed in a specific direction
/// </summary>
[System.Serializable]
public class CanPushQueryPayload
{
    // Input parameters
    public Vector2Int Position;
    public Direction Direction;
    public int Distance;
    public float Duration;

    // Result - filled by the nut tile that responds
    public bool CanBePushed;
}
