/// <summary>
/// Types of tutorial actions (all sequence-based).
/// Each type supports multiple operations in a single action element.
/// </summary>
public enum TutorialActionType
{
    MovementSequence,  // Execute multiple movements in sequence
    MessageSequence,   // Display multiple messages in sequence
    UndoSequence,      // Execute multiple undos
    ImageSequence,     // Show/hide/replace images (single for now, extendable to list)
    ResetSequence,     // Reset level to initial state mid-sequence
}
