using System.Collections;
using UnityEngine;

/// <summary>
/// goal tile abstract class
/// </summary>
public abstract class GoldenGoalTile : GoalTile, INutInteractive
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
            ScoreValue = 0,
            GoalRemove = false,
            CreateReplacement = false,
            GoalMessage = BubbleMessageType.Ooops,
            GoalMessageTime = gameplayConfig.goalBubbleTime,
        });

        // handle nut type specific actions to resolve the goal
        switch (payload.NutType)
        {
            case NutType.BasicNut: // basic nut falls into the goal, just remove the goal

                // Play goal reached sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} -> remove nut onlyat {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> remove nut only at {payload.CurrentPosition}", this);
                break;

            case NutType.StoneNut: // Build wall if stone nut reached the goal

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

                // fill goal payload - stone nut related actions
                goalPayload.CreateReplacement = true;
                goalPayload.InstantiateTile = true;
                goalPayload.ReplacementType = TileType.Obstacle;
                goalPayload.ReplacementObstacleType = ObstacleType.StoneWall;

                // debug log
                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} -> remove nut only at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> remove nut only at {payload.CurrentPosition}", this);
                break;

            case NutType.WaterNut: // water nut falls into the goal, create splash effect and splash around

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

                // splash water to move nuts around
                this.SplashWater();

                // debug log
                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} - creating splash at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> remove nut only and splash {payload.CurrentPosition}", this);
                break;

            case NutType.GoldenNut: // golden nut falls into the goal, open the way
               
                // Play special goal reached sound effect
                 audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });
                
                // fill goal payload - golden nut related actions               
                goalPayload.GoalMessage = BubbleMessageType.Yatta;
                goalPayload.ScoreValue = 1;
                goalPayload.GoalRemove = true;

                // debug log
                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} reached {this.goalType} -> remove both {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Goal Event: {payload.NutType} in {this.goalType} -> remove both {payload.CurrentPosition}", this);
                break;

            default:
                DebugLogger.LogWarning(DebugLogCategory.TileInteraction, $"Unhandled NutType {payload.NutType} in {this.goalType}", this);
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



