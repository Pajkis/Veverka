/// <summary>
/// Payload for tutorial system events
/// </summary>
public struct TutorialEventPayload
{
    public TutorialEventType EventType;
    public TutorialSetType SetType;
    public int TutorialIndex;
}

/// <summary>
/// Types of tutorial events
/// </summary>
public enum TutorialEventType
{
    LoadTutorial,
    ClearTutorial,
    ReplayTutorial
}
