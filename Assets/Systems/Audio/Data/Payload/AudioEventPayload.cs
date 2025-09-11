using UnityEngine;

/// <summary>
/// Payload used with <see cref="AudioEvents"/> to unify all audio related events.
/// </summary>
[System.Serializable]
public struct AudioEventPayload
{
    [Header("Basic audio parameters")]
    public AudioEventType EventType;  
    public MusicType Music; 
    public SfxType Sfx;   
    public UiType Ui;  
}