using System;
using UnityEngine;

/// <summary>
/// Static extension methods for ISplashWater interface
/// </summary>
public static class SplashWaterExtension
{
    /// <summary>
    /// Creates water splash effect around the tile, pushing basic nuts away
    /// </summary>
    /// <param name="speedMultiplier">Current gameplay speed multiplier</param>
    /// <param name="splashSpeedFactor">Speed factor for splash pushed nuts (e.g., 0.5 = half speed)</param>
    public static void SplashWater(this INutInteractive splasher, float splashSpeedFactor)
    {
        int directionCount = Enum.GetNames(typeof(Direction)).Length;

        for (int i = 0; i < directionCount; i++)
        {
            // Check each direction around the splash position
            Vector2Int NutQueryPosition = GridUtils.GetPositionInDir(splasher.GridPos, (Direction)i);
            TileQueryPayload queryPayload = new TileQueryPayload
            {
                Position = NutQueryPosition
            };

            // Query the tile at the position
            splasher.GridEvents.Raise(new GridEventPayload
            {
                EventType = GridEventType.TileQuery,
                Query = queryPayload,
            });

            // Check if there is a basic nut that can be pushed away
            if (queryPayload.TileType == TileType.Nut && ( queryPayload.NutType == NutType.BasicNut || queryPayload.NutType == NutType.WaterNut))
            {
                //check if the tile in the direction is pushable
               Vector2Int PushQueryPosition = GridUtils.GetPositionInDir(queryPayload.Position, (Direction)i);
               queryPayload.Position = PushQueryPosition;

                splasher.GridEvents.Raise(new GridEventPayload
                {
                    EventType = GridEventType.TileQuery,
                    Query = queryPayload,
                });

                if (!queryPayload.IsPushable) continue;

                // Raise event to push the nut away from the splash with splash effect
                splasher.NutEvents.Raise(new NutEventPayload
                {
                    EventType = NutEventType.NutPush,
                    PreviousPosition = NutQueryPosition,
                    CurrentPosition = PushQueryPosition,
                    Direction = (Direction)i,
                    Distance = 1,                   
                    IsSplash = true,
                    SplashSpeedFactor = splashSpeedFactor,
                });
            }
        }
    }
}

