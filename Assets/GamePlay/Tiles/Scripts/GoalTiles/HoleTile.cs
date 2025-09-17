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
        
        // Play goal reached sound effect
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });
        // Raise goal reached event
        if (payload.NutType == NutType.StoneNut)
        {
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

        //Call base - all goal actions settleted
        base.OnNutInGoal(payload);
        
    }
    #endregion
}
