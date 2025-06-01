using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

/// <summary>
/// Tile types extension static class
/// </summary>
public static class TileTypeExtensions
{
    /// <summary>
    /// defines what tile types are considered as obstacle
    /// </summary>
    /// <param name="tiletype">type of the tile</param>
    /// <returns></returns>
    public static bool IsObstacle(TileType tileType)
    {
        return tileType == TileType.Wall;
    }

    // <summary>
    /// defines what tile types are considered as movable obstacles
    /// </summary>
    /// <param name="tiletype">type of the tile</param>
    /// <returns></returns>
    public static bool IsMovable(TileType tileType)
    {
        return tileType == TileType.Nut;
    }

    /// <summary>
    /// checks whether input tile types can be walked on
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public static bool IsWalkable(TileType tileType)
    {
        return tileType == TileType.Empty || tileType == TileType.Goal;
    }

    /// <summary>
    /// check whether tile is goal
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public static bool IsGoal(TileType tileType)
    { 
      return tileType == TileType.Goal;
    }

    /// <summary>
    /// parse grid symbols into tile types
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TileType ParseTileType(string symbol)
    {
        switch (symbol)
        {
            case "W": return TileType.Wall;
            case "V": return TileType.Veverka;
            case "N": return TileType.Nut;
            case "G": return TileType.Goal;
            case ".": return TileType.Empty;           
            default: return TileType.ErrorTile; 
        }   
    }
        

}
