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
        playSfxEvent.Raise(SfxType.NutMove);
    }
 
    #endregion
}

