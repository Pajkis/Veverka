/// <summary>
/// Enum defining different types of goal events.
/// </summary>
public enum GoalEventsType
{
    GoalSet,
    GoalResolve,
    NutInGoalDone
}

/// <summary>
/// Enum defining different types of nut events.
/// </summary>
public enum NutEventType
{
    NutPush,
    NutSet,
    NutRemoved,
    NutEnteringGoal,
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
