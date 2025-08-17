using UnityEngine;

/// <summary>
/// Payload for audio volume change events.
/// </summary>
public struct AudioVolumePayload
{
    public GameSettingsEnum Source;
    public int VolumeValue;
}
