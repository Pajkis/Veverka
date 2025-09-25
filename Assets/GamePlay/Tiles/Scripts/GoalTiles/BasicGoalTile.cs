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

        DebugLogger.Log(DebugLogCategory.TileInteraction, $"OnNutInGoal - NutType: {payload.NutType}, Position: {payload.CurrentPosition}, GoalPosition: {GridPosition}", this);

        // Build wall if stone nut reached the goal
        if (payload.NutType == NutType.StoneNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Stone nut reached goal - creating wall at {payload.CurrentPosition}", this);

            // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

            // FIRST: Raise goal resolved event (like HoleTile does)
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                ScoreValue = 1,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Yatta,
                GoalMessageTime = gameplayConfig.goalBubbleTime
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for stone nut at {payload.CurrentPosition}", this);

            // Destroy goal tile
            Destroy(gameObject);

            // THEN: Create wall (after goal is resolved and removed)
            wallEvents.Raise(new WallEventPayload
            {
                EventType = WallEventType.WallSet,
                Position = payload.CurrentPosition,
                WallType = WallType.StoneWall,
                InstantiateTile = true,
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"WallSet event sent for stone wall at {payload.CurrentPosition}", this);
        }

        // water nut falls into the goal, create splash effect and splash around
        else if (payload.NutType == NutType.WaterNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Water nut reached goal - creating splash at {payload.CurrentPosition}", this);

            // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

            // splash water to move nuts around
            this.SplashWater();

            // Raise goal reached event
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                ScoreValue = 1,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Yatta,
                GoalMessageTime = gameplayConfig.goalBubbleTime
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for water nut at {payload.CurrentPosition}", this);

            // Destroy goal tile
            Destroy(gameObject);
        }

        else if (payload.NutType == NutType.BasicNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Basic nut reached goal at {payload.CurrentPosition}", this);

            // Play goal reached sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

            // Raise goal reached event
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                ScoreValue = 1,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Yatta,
                GoalMessageTime = gameplayConfig.goalBubbleTime
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for basic nut at {payload.CurrentPosition}", this);

            // Destroy goal tile
            Destroy(gameObject);
        }

        //Call base - all goal actions settled
        base.OnNutInGoal(payload);
               
    }

    #endregion
}
