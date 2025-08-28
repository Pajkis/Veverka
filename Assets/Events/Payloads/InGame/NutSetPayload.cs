using UnityEngine;

/// <summary>
/// Payload for setting a pushable tile on the grid.
/// </summary>
public struct NutSetPayload
{
    public Vector2Int Position;
    public NutTile NutType;
}
