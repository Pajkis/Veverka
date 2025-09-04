/// <summary>
/// Hole tile class
/// </summary>
public class HoleTile : GoalTile
{
    #region methods
    /// <summary>
    /// On Nut in hole event settlement
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
            if (payload.NutType == NutType.StoneNut)
            {
                goalResolvedEvent.Raise(new GoalBasicPayload
                {
                    Position = payload.Position,
                    GoalReduction = 0,
                    GoalRemove = true,
                    GoalMessage = BubbleMessageType.Yatta,
                    GoalMessageTime = 1f
                });
                Destroy(gameObject);

                // Set wall in the position of the hole
                roadSetEvent.Raise(new RoadBasicPayload
                {
                    Position = payload.Position,
                    RoadType = RoadType.StoneFilledHole,
                    instantiateTile = true,
                });
            }
            else
            {
                // Display "Ooops" message if a non-stone nut falls into the hole
                goalResolvedEvent.Raise(new GoalBasicPayload
                {
                    Position = payload.Position,
                    GoalRemove = false,
                    GoalMessage = BubbleMessageType.Ooops,
                    GoalMessageTime = 1f                    
                });
            }
                
        }
    }
    #endregion
}
