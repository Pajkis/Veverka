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
    protected override void OnNutInGoal(NutEventPayload payload)
    {
        // Check if the pushable is in the goal position
        if (GridPosition != payload.CurrentPosition) return;
        
        // Raise goal reached event
        if (payload.NutType == NutType.StoneNut)
        {
             // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                GoalReduction = 0,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Road,
                GoalMessageTime = 1f
            });
            Destroy(gameObject);

            // Set wall in the position of the hole
            roadEvents.Raise(new RoadEventPayload
            {
                EventType = RoadEventType.RoadSet,
                Position = payload.CurrentPosition,
                RoadType = RoadType.StoneFilledHole,
                InstantiateTile = true,
            });
        }

        // basic nut falls into the hole, display "Ooops" message
        else if (payload.NutType == NutType.BasicNut)
        {
            // Play sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

            // Display "Ooops" message if a non-stone nut falls into the hole
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                GoalRemove = false,
                GoalMessage = BubbleMessageType.Ooops,
                GoalMessageTime = 1f
            });
        }

        // water nut falls into the hole, create water hole
        else if (payload.NutType == NutType.WaterNut)
        {

            // Play waterfill sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterFill });

            // remove the hole goal
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                GoalReduction = 0,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Lake,
                GoalMessageTime = 1f
            });

            // Destroy with small delay to ensure all operations complete
            Destroy(gameObject);

            // Create water hole goal instead
            goalEvents.Raise(new GoalEventPayload
            {
                EventType =  GoalEventsType.GoalSet,
                Position = payload.CurrentPosition,
                GoalType = GoalType.WaterHoleGoal,
                InstantiateTile = true,
            });
        }

        //Call base - all goal actions settled
        base.OnNutInGoal(payload);
        
    }
    #endregion
}
