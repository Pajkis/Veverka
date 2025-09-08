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
            goalResolvedEvent.Raise(new GoalBasicPayload
            {
                Position = payload.Position,
                GoalReduction = 1,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Yatta,
                GoalMessageTime = 1f
            });
            Destroy(gameObject);

            // Build wall if stone nut reached the goal
            if (payload.NutType == NutType.StoneNut)
            {
                wallSetEvent.Raise(new WallBasicPayload
                {
                    Position = payload.Position,
                    WallType = WallType.StoneWall,
                    instantiateTile = true,
                });
            }
        }        
    }

    #endregion
}
