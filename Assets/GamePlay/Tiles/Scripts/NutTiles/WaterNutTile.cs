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
        base.OnMoveStart();
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.NutMove });
    }
    #endregion  
}

