using UnityEngine;

/// <summary>
/// Basic Goal type class
/// </summary>
public class BasicGoalTile : GoalTile, INutInteractive
{
    #region INutInteractive
    public Vector2Int GridPos => base.GridPosition;
    public NutEvents NutEvents => nutEvents;
    public GridEvents GridEvents => gridEvents;
    #endregion

    #region methods
    /// <summary>
    /// On nut in goal event settlement
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
            ScoreValue = 1,
            GoalRemove = true,
            GoalMessage = BubbleMessageType.Yatta,
            GoalMessageTime = gameplayConfig.goalBubbleTime,
        });
        
        // Build wall if stone nut reached the goal
        if (payload.NutType == NutType.StoneNut)
        {
            // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });                      
         
            // fill goal payload - stone nut related actions
            goalPayload.CreateReplacement = true;
            goalPayload.ReplacementType = TileType.Wall;
            goalPayload.ReplacementWallType = WallType.StoneWall;                  

            // debug log
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} - creating wall at {payload.CurrentPosition}", this);
            DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in basic {this.goalType} -> replacement with stone wall at {payload.CurrentPosition}", this);        
        }

        // water nut falls into the goal, create splash effect and splash around
        else if (payload.NutType == NutType.WaterNut)
        {
            // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

            // splash water to move nuts around
            this.SplashWater();

            // debug log
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} - creating splash at {payload.CurrentPosition}", this);
            DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in basic {this.goalType} -> remove and splash {payload.CurrentPosition}", this);           
        }
        // basic nut falls into the goal, just remove the goal
        else if (payload.NutType == NutType.BasicNut)
        {
            // Play goal reached sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} at {payload.CurrentPosition}", this);
            DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> remove {payload.CurrentPosition}", this);            
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
