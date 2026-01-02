using UnityEngine;

/// <summary>
/// Extension methods for Direction enum
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Converts Direction to Vector2Int for grid-based movement
    /// </summary>
    public static Vector2Int ToVector2Int(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2Int(0, 1),
            Direction.Right => new Vector2Int(1, 0),
            Direction.Down => new Vector2Int(0, -1),
            Direction.Left => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };
    }

    /// <summary>
    /// Converts Direction to Vector2 for animation/movement
    /// </summary>
    public static Vector2 ToVector2(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2(0, 1),
            Direction.Right => new Vector2(1, 0),
            Direction.Down => new Vector2(0, -1),
            Direction.Left => new Vector2(-1, 0),
            _ => Vector2.zero
        };
    }

    /// <summary>
    /// Gets the opposite direction
    /// </summary>
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            _ => direction
        };
    }
}
