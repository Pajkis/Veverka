/// <summary>
/// Hole tile class
/// </summary>
public class HoleTile : ObstacleTile
{
    #region methods
    /// <summary>
    /// On Nut in hole event settlement
    /// </summary>
    /// <param name="payload"></param>
    protected override void OnNutInObstacle(NutEventPayload payload)
    {
        // Check if the pushable is in the goal position
        if (GridPosition != payload.CurrentPosition) return;
        DebugLogger.Log(DebugLogCategory.TileInteraction, $"OnNutInObstacle: {this.obstacleType} - NutType: {payload.NutType}, Position: {payload.CurrentPosition}", this);

        // obstacle payload
        ObstacleEventPayload obstaclePayload = (new ObstacleEventPayload
        {
            // fill obstacle payload - common obstacle related actions
            ObstacleTile = this,
            ObstacleType = this.obstacleType,
            EventType = ObstacleEventsType.ObstacleResolve,
            Position = payload.CurrentPosition,
            ObstacleRemove = false,
            CreateReplacement = false,
            BubbleMessage = BubbleMessageEventPayload.None,
            PopoutImage = PopoutImageEventPayload.None,
        });

        // handle nut type specific actions to resolve the goal
        switch (payload.NutType)
        {
            case NutType.BasicNut:  // basic nut falls into the hole with no effect

                // Play sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

                //fill obstacle payload - basic nut related actions
                obstaclePayload.ObstacleRemove = false;
                obstaclePayload.BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Ooops, gameplayConfig.goalBubbleTime);

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} fell into {this.obstacleType} at {payload.CurrentPosition} - showing Ooops message", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Obstacle Event: {payload.NutType} in {this.obstacleType} -> Nothing happens (Ooops) at {payload.CurrentPosition}", this);
                break;
            case NutType.StoneNut: // stone nut falls into the hole, splash around and fills it with road

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });

                // fill obstacle payload - road related actions
                obstaclePayload.ObstacleRemove = true;
                obstaclePayload.BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Road, gameplayConfig.goalBubbleTime);
                obstaclePayload.CreateReplacement = true;
                obstaclePayload.ReplacementType = TileType.Road;
                obstaclePayload.ReplacementRoadType = RoadType.StoneFilledHole;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} in {this.obstacleType} - filling hole at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Obstacle Event: {payload.NutType} in {this.obstacleType} -> replacement with {RoadType.StoneFilledHole} at {payload.CurrentPosition}", this);
                break;

            case NutType.WaterNut: // water nut falls into the hole, create water hole

                // Play waterfill sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterFill });

                obstaclePayload.ObstacleRemove = true;
                obstaclePayload.BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Lake, gameplayConfig.goalBubbleTime);
                obstaclePayload.CreateReplacement = true;
                obstaclePayload.ReplacementType = TileType.Obstacle;
                obstaclePayload.ReplacementObstacleType = ObstacleType.WaterHole;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} in hole - creating water hole at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Obstacle Event: {payload.NutType} in {this.obstacleType} -> fill and make {ObstacleType.WaterHole} at {payload.CurrentPosition}", this);
                break;

            default:
                DebugLogger.LogWarning(DebugLogCategory.TileInteraction, $"OnNutInObstacle: {this.obstacleType} - Unhandled NutType: {payload.NutType}, Position: {payload.CurrentPosition}", this);
                return; // unhandled nut type
        }

        //raise obstacle event with obstacle payload
        obstacleEvents.Raise(obstaclePayload);

        // Set flag for obstacle removal (will be handled in base class)
        shouldRemoveObstacle = obstaclePayload.ObstacleRemove;

        //Call base - all obstacle actions settled
        base.OnNutInObstacle(payload);
        
    }
    #endregion
}
