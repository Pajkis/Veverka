/// <summary>
/// Basic Goal type class
/// </summary>
public class BasicGoalTile : GoalTile
{ 
    #region methods
    /// <summary>
    /// On nut in goal event settlement
    /// </summary>
    /// <param name="payload"></param>
    protected override void OnNutInGoal(NutBasicPayload payload)
    {
        // Check if the pushable is in the goal position
        if (GridPosition == payload.Position)
        {
            // Play goal reached sound effect
            playSfxEvent.Raise(SfxType.GoalReached);
            // Raise goal reached event
            goalRemovedEvent.Raise(new GoalBasicPayload
            {
                Position = payload.Position,
                GoalReduction = 1,
            });
            Destroy(gameObject);
        }

        // Build wall if stone nut reached the goal
        if (payload.NutType == NutType.StoneNut)
        { 
            wallSetEvent.Raise(new WallBasicPayload
            {
                Position = payload.Position,
                WallType = WallType.StoneWall,
            });
        }
    }

    #endregion
}
