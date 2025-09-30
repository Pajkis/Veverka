/// <summary>
/// stone nut tile class - nut tile type that can be pushed into goals or holes
/// </summary>
public class WaterNutTile : NutTile
{
    #region methods
   
    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        DebugLogger.Log(DebugLogCategory.NutMovement, $"WaterNut move started from position: {GridPosition}", this);
        DebugLogger.Log(DebugLogCategory.Audio, "Playing WaterNut move sound", this);
        base.OnMoveStart();
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.NutMove });
    }
    #endregion  
}

