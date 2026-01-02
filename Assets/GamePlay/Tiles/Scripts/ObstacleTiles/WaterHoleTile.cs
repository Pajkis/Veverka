using UnityEngine;

/// <summary>
/// Water hole tile class
/// </summary>
public class WaterHoleTile : ObstacleTile, INutInteractive
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
    protected override void OnNutInObstacle(NutEventPayload payload)
    {
        // Check if the pushable is in the obstacle position
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
              
                // Play nut In Water sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.NutInWater });

                //fill obstacle payload - basic nut related actions
                obstaclePayload.ObstacleRemove = false;
                obstaclePayload.BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Ooops, gameplayConfig.goalBubbleTime);

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} fell into {this.obstacleType} at {payload.CurrentPosition} - showing Ooops message", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Obstacle Event: {payload.NutType} in {this.obstacleType} -> Nothing happens (Ooops) at {payload.CurrentPosition}", this);
                break;

            case NutType.StoneNut: // stone nut falls into the hole, splash around and fills it with road

                // Play build sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });
                // Play splash sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

                // splash water to move nuts around
                this.SplashWater();

                // fill obstacle payload - stone nut related actions
                obstaclePayload.ObstacleRemove = true;
                obstaclePayload.BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Splash, gameplayConfig.goalBubbleTime);
                obstaclePayload.CreateReplacement = true;
                obstaclePayload.ReplacementType = TileType.Road;
                obstaclePayload.ReplacementRoadType = RoadType.StoneFilledHole;

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"Stone nut in water hole - filling and splashing at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for stone in water hole (splash) at {payload.CurrentPosition}", this);

                break;

            case NutType.WaterNut: // Water nut splashes and moves other nuts around

                // Play splash sound effect
                audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

                // splash water to move nuts around
                this.SplashWater();

                // fill obstacle payload - water nut related actions
                obstaclePayload.ObstacleRemove = false;
                obstaclePayload.BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.Splash, gameplayConfig.goalBubbleTime);

                DebugLogger.Log(DebugLogCategory.TileInteraction, $"{payload.NutType} in {this.obstacleType} - splash at {payload.CurrentPosition}", this);
                DebugLogger.Log(DebugLogCategory.EventSystem, $"Obstacle Event: {payload.NutType} in {this.obstacleType} - splash at {payload.CurrentPosition}", this);
                break;

            default:
                DebugLogger.LogWarning(DebugLogCategory.TileInteraction, $"OnNutInObstacle (WaterHoleTile) - Invalid NutType: {payload.NutType} at Position: {payload.CurrentPosition}", this);
                return; // exit if invalid nut type
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