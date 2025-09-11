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
        if (GridPosition == payload.Position)
        {
            // Play goal reached sound effect
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

            // stone nut falls into the hole, splash around and fills it with road
            if (payload.NutType == NutType.StoneNut)
            {
                goalEvents.Raise(new GoalEventPayload
                {
                    EventType = GoalEventsType.GoalResolved,
                    Position = payload.Position,
                    GoalReduction = 0,
                    GoalRemove = true,
                    GoalMessage = BubbleMessageType.Splash,
                    GoalMessageTime = 1f
                });
                Destroy(gameObject);

                // splash water to move nuts around
                this.SplashWater();

                // Set wall in the position of the hole
                roadEvents.Raise(new RoadEventPayload
                {
                    EventType = RoadEventType.RoadSet,
                    Position = payload.Position,
                    RoadType = RoadType.StoneFilledHole,
                    InstantiateTile = true,
                });
            }

            // basic nut falls into the hole with no effect
            else if (payload.NutType == NutType.BasicNut)
            {
                // Display "Ooops" message if a non-stone nut falls into the hole
                goalEvents.Raise(new GoalEventPayload
                {
                    EventType = GoalEventsType.GoalResolved,
                    Position = payload.Position,
                    GoalRemove = false,
                    GoalMessage = BubbleMessageType.Ooops,
                    GoalMessageTime = 1f
                });
            }

            // Water nut splashes and moves other nuts around
            else if (payload.NutType == NutType.WaterNut)
            {
                goalEvents.Raise(new GoalEventPayload
                {
                    EventType = GoalEventsType.GoalResolved,
                    Position = payload.Position,
                    GoalReduction = 0,
                    GoalRemove = false,
                    GoalMessage = BubbleMessageType.Splash,
                    GoalMessageTime = 1f
                });

                // splash water to move nuts around
                this.SplashWater();
            }
        }
    }
    #endregion    
}