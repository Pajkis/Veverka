using System;

/// <summary>
/// Static extension methods for ISplashWater interface
/// </summary>
public static class SplashWaterExtension
{
    /// <summary>
    /// Creates water splash effect around the tile, pushing basic nuts away
    /// </summary>
    public static void SplashWater(this INutInteractive splasher)
    {
        int directionCount = Enum.GetNames(typeof(Direction)).Length;

        for (int i = 0; i < directionCount; i++)
        {
            // Check each direction around the splash position
            TileQueryPayload queryPayload = new TileQueryPayload
            {
                Position = GridUtils.GetPositionInDir(splasher.GridPos, (Direction)i)
            };

            // Query the tile at the position
            splasher.GridEvents.Raise(new GridEventPayload
            {
                Query = queryPayload,
            });

            // Check if there is a basic nut that can be pushed away
            if (queryPayload.TileType == TileType.Nut && queryPayload.NutType == NutType.BasicNut)
            {
                // Raise event to push the nut away from the splash
                splasher.NutEvents.Raise(new NutEventPayload
                {
                    EventType = NutEventType.CanPushQuery,
                    Position = queryPayload.Position,
                    Direction = (Direction)i,
                });
            }
        }
    }
}

