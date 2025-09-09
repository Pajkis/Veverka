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
    protected override void OnNutInGoal(NutEventPayload payload)
    {
        // Check if the pushable is in the goal position
        if (GridPosition == payload.Position)
        {
            // Play goal reached sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });
            // Raise goal reached event
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
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
                wallEvents.Raise(new WallEventPayload
                {
                    EventType = WallEventType.WallSet,
                    Position = payload.Position,
                    WallType = WallType.StoneWall,
                    InstantiateTile = true,
                });
            }
        }        
    }

    #endregion
}
