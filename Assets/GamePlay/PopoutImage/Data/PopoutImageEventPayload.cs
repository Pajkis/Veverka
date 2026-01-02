using UnityEngine;

/// <summary>
/// Unified payload for popout images.
/// Can be used as sub-payload in Goal/Obstacle/Character payloads or as standalone event payload (future).
/// Sprite is expected to be pre-assigned in the prefab (not passed via payload).
/// </summary>
[System.Serializable]
public class PopoutImageEventPayload
{
    [Header("Type")]
    public PopoutImageType ImageType;        // Explicit type identification

    [Header("Position")]
    public Vector2Int Position;              // Grid position where image appears
    public Vector2 PositionOffset;           // Offset from grid position (in world units)

    [Header("Animation Parameters")]
    public float StartDelay;                 // Delay before animation starts (in seconds)
    public Direction AnimationDirection;     // Direction to move (Up, Down, Left, Right)
    public float AnimationDistance;          // Distance to travel in world units
    public float AnimationDuration;          // Total animation duration (move + fade)
    public float FadeStartTime;              // When to start fading (0 = immediately, duration = end)

    [Header("Visual Settings")]
    public float Scale;                      // Scale multiplier for the image (default 1.0)
    public Color TintColor;                  // Tint color (default white)

    /// <summary>
    /// Validates the payload data.
    /// Sprite is validated in the prefab (not in payload), so we only check animation parameters.
    /// </summary>
    public bool IsValid()
    {
        if (AnimationDuration <= 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay, "PopoutImageEventPayload: AnimationDuration must be > 0");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if this popout should be displayed
    /// </summary>
    public bool ShouldShow => ImageType != PopoutImageType.NoImage;

    /// <summary>
    /// Returns a "no popout" instance (for sub-payload use when popout is not needed)
    /// </summary>
    public static PopoutImageEventPayload None => new PopoutImageEventPayload
    {
        ImageType = PopoutImageType.NoImage
    };

    /// <summary>
    /// Creates instance for sub-payload use (without Position - will be set from parent payload).
    /// Sprite is expected to be pre-assigned in the prefab, so no sprite parameter.
    /// </summary>
    public static PopoutImageEventPayload CreateForSubPayload(PopoutImageType imageType, GameplayConfig config)
    {
        return new PopoutImageEventPayload
        {
            ImageType = imageType,
            Position = Vector2Int.zero, // Position will be taken from parent payload
            PositionOffset = Vector2.zero,
            StartDelay = 0f,
            AnimationDirection = config.popoutImageDefaultDirection,
            AnimationDistance = config.popoutImageDefaultDistance,
            AnimationDuration = config.popoutImageDefaultDuration,
            FadeStartTime = config.popoutImageDefaultFadeStartTime,
            Scale = 1.0f,
            TintColor = Color.white
        };
    }

    /// <summary>
    /// Creates a payload with default animation settings from config (for standalone use).
    /// Sprite is expected to be pre-assigned in the prefab, so no sprite parameter.
    /// </summary>
    public static PopoutImageEventPayload CreateDefault(PopoutImageType imageType, Vector2Int position, GameplayConfig config)
    {
        return new PopoutImageEventPayload
        {
            ImageType = imageType,
            Position = position,
            PositionOffset = Vector2.zero,
            StartDelay = 0f,
            AnimationDirection = config.popoutImageDefaultDirection,
            AnimationDistance = config.popoutImageDefaultDistance,
            AnimationDuration = config.popoutImageDefaultDuration,
            FadeStartTime = config.popoutImageDefaultFadeStartTime,
            Scale = 1.0f,
            TintColor = Color.white
        };
    }

    public override string ToString()
    {
        return $"PopoutImage ({ImageType}) at {Position}, direction {AnimationDirection}, distance {AnimationDistance}, duration {AnimationDuration}s";
    }
}
