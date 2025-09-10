/// <summary>
/// Enum defining different types of goal events.
/// </summary>
public enum GoalEventsType
{
    GoalSet,
    GoalResolved
}

/// <summary>
/// Enum defining different types of nut events.
/// </summary>
public enum NutEventType
{
    CanPushQuery,
    NutSet,
    NutRemoved,
    NutInGoal,
    NutMoved
}

/// <summary>
/// Enum defining different types of road events.
/// </summary>
public enum RoadEventType
{
    RoadSet
}

/// <summary>
/// Enum defining different types of wall events.
/// </summary>
public enum WallEventType
{
    WallSet
}
