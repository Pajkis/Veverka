
/// <summary>
/// Tile types extension static class
/// </summary>
public struct ParsedTile
{
    public TileType TileType;
    public GoalType GoalType;
    public WallType WallType;
    public NutType NutType;
    public RoadType RoadType;
}

public static class TileTypeExtensions
{
    /// <summary>
    /// defines what tile types are considered as obstacle
    /// </summary>
    /// <param name="tiletype">type of the tile</param>
    /// <returns></returns>
    public static bool IsObstacle(TileType tileType)
    {
        return tileType == TileType.Wall || tileType == TileType.Nut;
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
        return tileType == TileType.Empty || tileType == TileType.Goal || tileType == TileType.Road;
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
    public static ParsedTile ParseTileType(string symbol)
    {
        var result = new ParsedTile
        {
            TileType = TileType.ErrorTile,
            GoalType = GoalType.BasicGoal,
            WallType = WallType.BasicWall,
            NutType = NutType.BasicNut
        };

        switch (symbol)
        {
            case "W":
                result.TileType = TileType.Wall;
                result.WallType = WallType.BasicWall;
                break;
            case "WS":
                result.TileType = TileType.Wall;
                result.WallType = WallType.StoneWall;
                break;
            case "V":
                result.TileType = TileType.Veverka;
                break;
            case "N":
                result.TileType = TileType.Nut;
                result.NutType = NutType.BasicNut;
                break;
            case "NS":
                result.TileType = TileType.Nut;
                result.NutType = NutType.StoneNut;
                break;
            case "G":
                result.TileType = TileType.Goal;
                result.GoalType = GoalType.BasicGoal;
                break;
            case "H":
                result.TileType = TileType.Goal;
                result.GoalType = GoalType.HoleGoal;
                break;
            case ".":
                result.TileType = TileType.Empty;
                break;
            default:
                result.TileType = TileType.ErrorTile;
                break;
        }

        return result;
    }
}
