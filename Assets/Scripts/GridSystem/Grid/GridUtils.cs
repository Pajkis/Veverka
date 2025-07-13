using UnityEngine;

public static class GridUtils
{
    /// <summary>
    /// Gets position of adjacent (or further - based on distance) tile based on direction
    /// </summary>
    /// <param name="currentPosition">current position of object</param>
    /// <param name="direction">In which direction is the position checked</param>
    /// <param name="distance">Amount tiles in the direction</param>
    /// <returns>position of adjacent tile</returns>
    public static Vector2Int GetPositionInDir(Vector2Int currentPosition, Direction direction, int distance = 1)
    {
        if (distance < 1)  distance = 1;

        switch (direction)
        {
            case Direction.Up: return currentPosition + Vector2Int.up * distance; 
            case Direction.Down: return currentPosition + Vector2Int.down * distance; 
            case Direction.Left: return currentPosition + Vector2Int.left * distance;
            default: return currentPosition + Vector2Int.right * distance;
        }
    }

    /// <summary>
    /// Converts a grid position to a world space position.
    /// </summary>
    /// <param name="gridPosistion">Tile grid position (x, y)</param>
    /// <returns>World space position (Vector3)</returns>
    public static Vector3 GridToWorld(Vector2Int gridPosistion, float tileSize = 1f)
    {
        return new Vector3(gridPosistion.x * tileSize, gridPosistion.y * tileSize, 0);           
    }
}