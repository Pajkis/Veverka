
/// <summary>
/// Helper structure used when parsing level data.
/// </summary>
public struct ParsedTile
{
    public TileType TileType;
    public GoalType GoalType;
    public WallType WallType;
    public NutType NutType;
    public RoadType RoadType;
}

/// <summary>
/// Static parser for translating string symbols into tile type data.
/// </summary>
public static class TileParser
{
    /// <summary>
    /// Parse grid symbols into tile types.
    /// </summary>
    /// <param name="symbol">String representation of a tile.</param>
    /// <returns>Parsed tile data.</returns>
    public static ParsedTile ParseTileType(string symbol)
    {
        var result = new ParsedTile
        {
            TileType = TileType.ErrorTile,
            GoalType = GoalType.BasicGoal,
            WallType = WallType.BasicWall,
            NutType = NutType.BasicNut,
            RoadType = RoadType.BasicRoad,
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
            case "R":
                result.TileType = TileType.Road;
                result.RoadType = RoadType.BasicRoad;
                break;
            case "RS":
                result.TileType = TileType.Road;
                result.RoadType = RoadType.StoneRoad;
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
