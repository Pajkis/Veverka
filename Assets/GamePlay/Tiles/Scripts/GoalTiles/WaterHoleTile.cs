using UnityEngine;

/// <summary>
/// Hole tile class
/// </summary>
public class WaterHoleTile : GoalTile, INutInteractive
{
    #region INutInteractive
    public Vector2Int GridPos => base.GridPosition;
    public NutEvents NutEvents => nutEvents;
    public GridEvents GridEvents => gridEvents;
    #endregion

    #region methods
    /// <summary>
    /// On Nut in hole event settlement
    /// </summary>
    /// <param name="payload"></param>
    protected override void OnNutInGoal(NutEventPayload payload)
    {
        // Check if the pushable is in the goal position
        if (GridPosition != payload.CurrentPosition) return;
        DebugLogger.Log(DebugLogCategory.TileInteraction, $"OnNutInGoal (WaterHoleTile) - NutType: {payload.NutType}, Position: {payload.CurrentPosition}", this);

        // goal payload
        GoalEventPayload goalPayload = (new GoalEventPayload
        {
            // fill goal payload - common goal related actions
            GoalTile = this,
            GoalType = this.goalType,
            EventType = GoalEventsType.GoalResolve,
            Position = payload.CurrentPosition,
            ScoreValue = 0,          
            GoalMessageTime = gameplayConfig.goalBubbleTime,
        });

        // stone nut falls into the hole, splash around and fills it with road
        if (payload.NutType == NutType.StoneNut)
        {
            // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });
            // Play splash sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

            // splash water to move nuts around
            this.SplashWater();

            // fill goal payload - stone nut related actions
            goalPayload.GoalRemove = true;
            goalPayload.GoalMessage = BubbleMessageType.Splash;
            goalPayload.CreateReplacement = true;
            goalPayload.ReplacementType = TileType.Road;
            goalPayload.ReplacementRoadType = RoadType.StoneFilledHole;
            
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Stone nut in water hole - filling and splashing at {payload.CurrentPosition}", this);
            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for stone in water hole (splash) at {payload.CurrentPosition}", this);
        }

        // basic nut falls into the hole with no effect
        else if (payload.NutType == NutType.BasicNut)
        {
            // Play nut In Water sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.NutInWater });

            //fill goal payload - basic nut related actions
            goalPayload.GoalRemove = false;
            goalPayload.GoalMessage = BubbleMessageType.Ooops;

            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} fell into {this.goalType} at {payload.CurrentPosition} - showing Ooops message", this);
            DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> Nothin happens (Ooops) at {payload.CurrentPosition}", this);
        }

        // Water nut splashes and moves other nuts around
        else if (payload.NutType == NutType.WaterNut)
        {
            // Play splash sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

            // splash water to move nuts around
            this.SplashWater();

            // fill goal payload - water nut related actions
            goalPayload.GoalRemove = false;
            goalPayload.GoalMessage = BubbleMessageType.Splash;
                        
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} in {this.goalType} - splash at {payload.CurrentPosition}", this);
            DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} - splash at {payload.CurrentPosition}", this);
        }

        //raise goal event with goal payload
        goalEvents.Raise(goalPayload);

        // Set flag for goal removal (will be handled in base class)
        shouldRemoveGoal = goalPayload.GoalRemove;

        //Call base - all goal actions settleted
        base.OnNutInGoal(payload);
    }
   
    #endregion    
}