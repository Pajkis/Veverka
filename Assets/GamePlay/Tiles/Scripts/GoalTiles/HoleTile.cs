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

        DebugLogger.Log(DebugLogCategory.TileInteraction, $"OnNutInGoal (HoleTile) - NutType: {payload.NutType}, Position: {payload.CurrentPosition}", this);
        
        // Raise goal reached event
        if (payload.NutType == NutType.StoneNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Stone nut in hole - filling hole at {payload.CurrentPosition}", this);

             // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                ScoreValue = 0,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Road,
                GoalMessageTime = 1f
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for stone-filled hole at {payload.CurrentPosition}", this);

            Destroy(gameObject);

            // Set wall in the position of the hole
            roadEvents.Raise(new RoadEventPayload
            {
                EventType = RoadEventType.RoadSet,
                Position = payload.CurrentPosition,
                RoadType = RoadType.StoneFilledHole,
                InstantiateTile = true,
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"RoadSet event sent for stone-filled hole at {payload.CurrentPosition}", this);
        }

        // basic nut falls into the hole, display "Ooops" message
        else if (payload.NutType == NutType.BasicNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Basic nut fell into hole at {payload.CurrentPosition} - showing Ooops message", this);

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

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for basic nut in hole (Ooops) at {payload.CurrentPosition}", this);
        }

        // water nut falls into the hole, create water hole
        else if (payload.NutType == NutType.WaterNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Water nut in hole - creating water hole at {payload.CurrentPosition}", this);

            // Play waterfill sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterFill });

            // remove the hole goal
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                ScoreValue = 0,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Lake,
                GoalMessageTime = 1f
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for water hole at {payload.CurrentPosition}", this);

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

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalSet event sent for water hole goal at {payload.CurrentPosition}", this);
        }

        //Call base - all goal actions settled
        base.OnNutInGoal(payload);
        
    }
    #endregion
}
