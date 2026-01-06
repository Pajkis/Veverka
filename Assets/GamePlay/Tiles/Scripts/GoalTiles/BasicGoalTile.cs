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
            CreateReplacement = false,
            BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Yatta, gameplayConfig.goalBubbleTime),
            PopoutImage = PopoutImageEventPayload.None,
        });

        // handle nut type specific actions to resolve the goal
        switch (payload.NutType)
        {            
            case NutType.BasicNut: // basic nut falls into the goal, just remove the goal
                
                // Play goal reached sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

                 // Configure popout image sub-payload - SPECIFY IMAGE TYPE
                goalPayload.PopoutImage = PopoutImageEventPayload.CreateForSubPayload(
                    PopoutImageType.BasicGoal,
                    gameplayConfig
                );

                // Apply custom settings from inspector (per-tile overrides)
                goalPayload.PopoutImage.PositionOffset = popoutPositionOffset;
                goalPayload.PopoutImage.StartDelay = popoutStartDelay;
                goalPayload.PopoutImage.Scale = popoutScale;
                goalPayload.PopoutImage.TintColor = popoutTintColor;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} -> remove goal at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> remove goal at  {payload.CurrentPosition}", this);
                break;
                  
            case NutType.StoneNut: // Build wall if stone nut reached the goal

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

                // fill goal payload - stone nut related actions
                goalPayload.CreateReplacement = true;            
                goalPayload.ReplacementType = TileType.Obstacle;
                goalPayload.ReplacementObstacleType = ObstacleType.StoneWall;

                // debug log
                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} at {payload.CurrentPosition} -> remove goal and build wall", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in basic {this.goalType} at {payload.CurrentPosition} -> replacement with stone wall", this);
                break;

            case NutType.WaterNut: // water nut falls into the goal, create splash effect and splash around

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

                // splash water to move nuts around
                this.SplashWater();

                // debug logger
                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} at {payload.CurrentPosition} -> remove goal and splash", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in basic {this.goalType} at {payload.CurrentPosition} -> remove goal and splash", this);
                break;
            
            case NutType.GoldenNut: // golden nut falls into the goal, make the goal golden statue
                
                // Play goal reached sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });
                
                // debug logger
                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} at {payload.CurrentPosition} -> Remove goal and Create golden statue", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} at {payload.CurrentPosition} -> Remove goal and Create golden statue", this);
                break;

            default:
                DebugLogger.LogWarning(DebugLogCategory.TileInteraction, $"Unhandled NutType {payload.NutType} in BasicGoalTile", this);
                break;
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
