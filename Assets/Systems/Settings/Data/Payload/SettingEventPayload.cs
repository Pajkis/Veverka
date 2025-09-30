/// <summary>
/// Payload for settings events.
/// </summary>
[System.Serializable]
public struct SettingEventPayload
{
    public SettingsEventType EventType;
    public GameSettingsEnum Setting;
    public float Value;
}