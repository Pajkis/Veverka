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
        DebugLogger.Log(DebugLogCategory.TileInteraction, $"OnNutInGoal: {this.goalType} - NutType: {payload.NutType}, Position: {payload.CurrentPosition}, GoalPosition: {GridPosition}", this);

        // goal payload
        GoalEventPayload goalPayload = (new GoalEventPayload
        {
            // fill goal payload - common goal related actions
            GoalTile = this,
            GoalType = this.goalType,
            EventType = GoalEventsType.GoalResolve,
            Position = payload.CurrentPosition,
            GoalRemove = false,
            CreateReplacement = false,
            ScoreValue = 0,
            GoalMessageTime = gameplayConfig.goalBubbleTime,
        });

        // handle nut type specific actions to resolve the goal
        switch (payload.NutType)
        {
            case NutType.BasicNut:  // basic nut falls into the hole with no effect

                // Play sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

                //fill goal payload - basic nut related actions
                goalPayload.GoalRemove = false;
                goalPayload.GoalMessage = BubbleMessageType.Ooops;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} fell into {this.goalType} at {payload.CurrentPosition} - showing Ooops message", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> Nothin happens (Ooops) at {payload.CurrentPosition}", this);
                break;
            case NutType.StoneNut: // stone nut falls into the hole, splash around and fills it with road

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

                // fill goal payload - road related actions
                goalPayload.GoalRemove = true;
                goalPayload.GoalMessage = BubbleMessageType.Road;
                goalPayload.CreateReplacement = true;
                goalPayload.ReplacementType = TileType.Road;
                goalPayload.ReplacementRoadType = RoadType.StoneFilledHole;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} in {this.goalType} - filling hole at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> replacement with {RoadType.StoneFilledHole} at {payload.CurrentPosition}", this);
                break;

            case NutType.WaterNut: // water nut falls into the hole, create water hole

                // Play waterfill sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterFill });

                goalPayload.GoalRemove = true;
                goalPayload.GoalMessage = BubbleMessageType.Lake;
                goalPayload.CreateReplacement = true;
                goalPayload.ReplacementType = TileType.Goal;
                goalPayload.ReplacementGoalType = GoalType.WaterHoleGoal;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} in hole - creating water hole at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> fill and make {GoalType.WaterHoleGoal} at {payload.CurrentPosition}", this);
                break;

            default:
                DebugLogger.LogWarning(DebugLogCategory.TileInteraction, $"OnNutInGoal: {this.goalType} - Unhandled NutType: {payload.NutType}, Position: {payload.CurrentPosition}", this);
                return; // unhandled nut type
        }
       
        //raise goal event with goal payload
        goalEvents.Raise(goalPayload);

        // Set flag for goal removal (will be handled in base class)
        shouldRemoveGoal = goalPayload.GoalRemove;

        //Call base - all goal actions settled
        base.OnNutInGoal(payload);
        
    }
    #endregion
}
