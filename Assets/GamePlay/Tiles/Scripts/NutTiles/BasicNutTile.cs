using UnityEngine;

/// <summary>
/// Class for the nut Tile - basic push to goal tile.
/// </summary>
public class BasicNutTile : NutTile
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

