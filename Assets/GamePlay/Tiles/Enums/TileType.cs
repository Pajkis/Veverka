/// <summary>
/// Enumeration of tile types
/// </summary>
public enum TileType
{
    ErrorTile,  // Default value (0) - uninitialized or invalid tile
    Road,
    Obstacle,
    Nut,
    Veverka,
    Goal
}

/// <summary>
/// Enumeration of nut types
/// </summary>
public enum NutType
{
    None,        // No nut (default/empty)
    BasicNut,
    StoneNut,
    WaterNut,
    GoldenNut
}

/// <summary>
/// Enumeration of goal types
/// </summary>
public enum GoalType
{
    None,        // No goal (default/empty)
    BasicGoal,
    GoldenGoal
}

/// <summary>
/// Enumeration of obstacle types
/// </summary>
public enum ObstacleType
{
    None,           // No obstacle (default/empty)
    BasicWall,
    StoneWall,
    Hole,
    WaterHole,
    GoldenStatue,
}

/// <summary>
/// Road types enumeration
/// </summary>
public enum RoadType
{
    None,           // No road (default/empty)
    Empty,          // Empty walkable tile (background only)
    BasicRoad,
    StoneRoad,
    StoneFilledHole,
    BranchRoad
}