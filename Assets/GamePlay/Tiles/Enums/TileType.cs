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
    BasicNut,
    StoneNut,
    WaterNut
}

/// <summary>
/// Enumeration of goal types
/// </summary>
public enum GoalType
{
    BasicGoal,
    HoleGoal,
    WaterHoleGoal
}

/// <summary>
/// Enumeration of wall types
/// </summary>
public enum WallType
{
    BasicWall,
    StoneWall
}

/// <summary>
/// Road types enumeration
/// </summary>
public enum RoadType
{ 
 BasicRoad,
 StoneRoad,
 StoneFilledHole,
 BranchRoad
}