using UnityEngine;

/// <summary>
/// Unified payload for nut events.
/// </summary>
[System.Serializable]
public class NutEventPayload
{
    // Basic nut data 
    [Header("Basic nut data")]
    public NutEventType EventType;
    public NutType NutType;
    public NutTile NutTile;

    // Movement data
    [Header("Nut movement data")]
    public Vector2Int CurrentPosition;
    public Vector2Int PreviousPosition;

    // Push query data
    public Direction Direction;
    public int Distance;
    public float SpeedMultiplier;

    // Splash data
    [Header("Splash effect data")]
    public bool IsSplash;
    public float SplashSpeedFactor;    
}
