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
                goalRemovedEvent.Raise(new GoalBasicPayload
                {
                    Position = payload.Position,
                    GoalReduction = 0,
                });
                Destroy(gameObject);

                // Set wall in the position of the hole
                roadSetEvent.Raise(new RoadBasicPayload
                {
                    Position = payload.Position,
                    RoadType = RoadType.StoneRoad,
                });
            }                    
        }
    }
    #endregion
}
