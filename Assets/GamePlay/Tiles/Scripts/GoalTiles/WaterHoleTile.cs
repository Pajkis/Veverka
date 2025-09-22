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
       
        // stone nut falls into the hole, splash around and fills it with road
        if (payload.NutType == NutType.StoneNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Stone nut in water hole - filling and splashing at {payload.CurrentPosition}", this);

            // Play build sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Build });
            // Play splash sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                GoalReduction = 0,
                GoalRemove = true,
                GoalMessage = BubbleMessageType.Splash,
                GoalMessageTime = 1f
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for stone in water hole (splash) at {payload.CurrentPosition}", this);

            // splash water to move nuts around
            this.SplashWater();

            // Set wall in the position of the hole
            roadEvents.Raise(new RoadEventPayload
            {
                EventType = RoadEventType.RoadSet,
                Position = payload.CurrentPosition,
                RoadType = RoadType.StoneFilledHole,
                InstantiateTile = true,
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"RoadSet event sent for filled water hole at {payload.CurrentPosition}", this);

            // Destroy with small delay to ensure all operations complete
            StartCoroutine(DestroyAfterDelay());
        }

        // basic nut falls into the hole with no effect
        else if (payload.NutType == NutType.BasicNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Basic nut fell into water hole at {payload.CurrentPosition} - showing Ooops", this);

            // Play nut In Water sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.NutInWater });

            // Display "Ooops" message if a non-stone nut falls into the hole
            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                GoalRemove = false,
                GoalMessage = BubbleMessageType.Ooops,
                GoalMessageTime = 1f
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for basic nut in water hole (Ooops) at {payload.CurrentPosition}", this);
        }

        // Water nut splashes and moves other nuts around
        else if (payload.NutType == NutType.WaterNut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"Water nut in water hole - splashing at {payload.CurrentPosition}", this);

            // Play splash sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.WaterSplash });

            goalEvents.Raise(new GoalEventPayload
            {
                EventType = GoalEventsType.GoalResolved,
                Position = payload.CurrentPosition,
                GoalReduction = 0,
                GoalRemove = false,
                GoalMessage = BubbleMessageType.Splash,
                GoalMessageTime = 1f
            });

            DebugLogger.Log(DebugLogCategory.EventSystem, $"GoalResolved event sent for water nut splash at {payload.CurrentPosition}", this);

            // splash water to move nuts around
            this.SplashWater();
        }

        //Call base - all goal actions settleted
        base.OnNutInGoal(payload);
    }

    /// <summary>
    /// Coroutine to destroy the waterhole after a small delay
    /// </summary>
    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return null; // Wait one frame
        Destroy(gameObject);
    }
    #endregion    
}