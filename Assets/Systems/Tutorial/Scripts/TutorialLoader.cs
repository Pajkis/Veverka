using UnityEngine;

/// <summary>
/// Tutorial overlay controller - raises events to communicate with persistent TutorialManager.
/// Lives on TutorialOverlay prefab, sends commands via TutorialEvents.
/// </summary>
public class TutorialLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialSelectData tutorialSelection;
    [SerializeField] private TutorialEvents tutorialEvents;

    /// <summary>
    /// Load tutorial when overlay starts
    /// </summary>
    private void Start()
    {
        LoadCurrentTutorial();
    }

    /// <summary>
    /// Load the currently selected tutorial by raising event
    /// </summary>
    private void LoadCurrentTutorial()
    {
        if (tutorialSelection == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialSelection is not assigned!", this);
            return;
        }

        if (tutorialEvents == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialEvents is not assigned!", this);
            return;
        }

        // Read current selection
        TutorialSetType setType = tutorialSelection.TutorialSetType;
        int tutorialIndex = tutorialSelection.TutorialIndex;

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"TutorialLoader: Raising LoadTutorial event - Set={setType}, Index={tutorialIndex}", this);

        // Raise event for persistent TutorialManager to handle
        tutorialEvents.Raise(new TutorialEventPayload
        {
            EventType = TutorialEventType.LoadTutorial,
            SetType = setType,
            TutorialIndex = tutorialIndex
        });
    }

    /// <summary>
    /// Replay button handler - raises replay event
    /// </summary>
    public void OnReplayButtonClick()
    {
        if (tutorialSelection == null || tutorialEvents == null) return;

        DebugLogger.Log(DebugLogCategory.Tutorial, "Replay button clicked - raising replay event", this);

        tutorialEvents.Raise(new TutorialEventPayload
        {
            EventType = TutorialEventType.ReplayTutorial,
            SetType = tutorialSelection.TutorialSetType,
            TutorialIndex = tutorialSelection.TutorialIndex
        });
    }
}
