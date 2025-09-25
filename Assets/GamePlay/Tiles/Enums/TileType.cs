/// <summary>
/// Enumeration of tile types
/// </summary>
public enum TileType
{
    Empty,
    Road,
    Wall,
    Nut,
    Veverka,
    Goal,
    ErrorTile
}

/// <summary>
/// Enumeration of nut types
/// </summary>
public enum NutType
{
    None,        // No nut (default/empty)
    BasicNut,
    StoneNut,
    WaterNut
}

/// <summary>
/// Enumeration of goal types
/// </summary>
public enum GoalType
{
    None,        // No goal (default/empty)
    BasicGoal,
    HoleGoal,
    WaterHoleGoal
}

/// <summary>
/// Enumeration of wall types
/// </summary>
public enum WallType
{
    None,        // No wall (default/empty)
    BasicWall,
    StoneWall
}

/// <summary>
/// Road types enumeration
/// </summary>
public enum RoadType
{
    None,           // No road (default/empty)
    BasicRoad,
    StoneRoad,
    StoneFilledHole,
    BranchRoad
}