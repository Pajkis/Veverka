using UnityEngine;

/// <summary>
/// Payload used with <see cref="AudioEvents"/> to unify all audio related events.
/// </summary>
[System.Serializable]
public struct AudioEventPayload
{
    /// <summary>
    /// Type of audio event that should be processed.
    /// </summary>
    public AudioEventType EventType;

    /// <summary>
    /// Music clip to play when <see cref="AudioEventType.PlayMusic"/> is raised.
    /// </summary>
    public MusicType Music;

    /// <summary>
    /// Sound effect clip to play when <see cref="AudioEventType.PlaySfx"/> is raised.
    /// </summary>
    public SfxType Sfx;

    /// <summary>
    /// UI clip to play when <see cref="AudioEventType.PlayUi"/> is raised.
    /// </summary>
    public UiType Ui;
}

