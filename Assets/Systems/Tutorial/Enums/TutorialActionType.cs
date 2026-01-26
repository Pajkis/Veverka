/// <summary>
/// Types of actions in tutorial sequence.
/// </summary>
public enum TutorialActionType
{
    Animation, // Direction input to move/rotate character.
    Message,  // Display text message in overlay UI.
    Undo,  // trigger undo action during tutorial.
    ActionImage, // Show action image in overlay UI.
}
