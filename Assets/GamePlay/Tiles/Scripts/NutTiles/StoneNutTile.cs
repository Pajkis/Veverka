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
        DebugLogger.Log(DebugLogCategory.NutMovement, $"StoneNut move started from position: {GridPosition}", this);
        DebugLogger.Log(DebugLogCategory.Audio, "Playing StoneNut move sound", this);
        base.OnMoveStart();
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.NutMove });
    }
    #endregion  
}

