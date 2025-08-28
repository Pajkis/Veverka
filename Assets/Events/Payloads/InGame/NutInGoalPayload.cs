using UnityEngine;

/// <summary>
/// nut in goal event payload structure
/// </summary>
[System.Serializable]
public struct NutInGoalPayload
{
    public Vector2Int Position;
    public NutTile NutType;
}
