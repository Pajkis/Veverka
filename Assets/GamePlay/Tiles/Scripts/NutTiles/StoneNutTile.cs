using UnityEngine;

/// <summary>
/// stone nut tile class - nut tile type that can be pushed into goals or holes
/// </summary>
public class StoneNutTile : NutTile
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

