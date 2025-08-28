using UnityEngine;

/// <summary>
/// Payload for setting a pushable tile on the grid.
/// </summary>
public struct PushableSetPayload
{
    public Vector2Int Position;
    public NutTile Pushable;
}
