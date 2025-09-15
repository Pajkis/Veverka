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
         
        // Play goal reached sound effect
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.GoalReached });

        // Build wall if stone nut reached the goal
        if (payload.NutType == NutType.StoneNut)
        {
            wallEvents.Raise(new WallEventPayload
            {
                EventType = WallEventType.WallSet,
                Position = payload.CurrentPosition,
                WallType = WallType.StoneWall,
                InstantiateTile = true,
            });
        }

        // water nut falls into the goal, create splash effect and splash around
        else if (payload.NutType == NutType.WaterNut)
        {
            // splash water to move nuts around
            this.SplashWater();
        }

        // Raise goal reached event
        goalEvents.Raise(new GoalEventPayload
        {
            EventType = GoalEventsType.GoalResolved,
            Position = payload.CurrentPosition,
            GoalReduction = 1,
            GoalRemove = true,
            GoalMessage = BubbleMessageType.Yatta,
            GoalMessageTime = gameplayConfig.goalBubbleTime
        });
        Destroy(gameObject);

        //Call base - all goal actions settleted
        base.OnNutInGoal(payload);
               
    }

    #endregion
}
