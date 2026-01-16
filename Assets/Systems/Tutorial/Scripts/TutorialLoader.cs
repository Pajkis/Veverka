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
    [SerializeField] private TutorialActionSequencer actionSequencer;

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

        // Start action sequence after grid delay
        StartActionSequence();
    }

    /// <summary>
    /// Start action sequence with delay to wait for grid spawn
    /// </summary>
    private void StartActionSequence()
    {
        Debug.Log($"[TUTORIAL] TutorialLoader.StartActionSequence called, actionSequencer={actionSequencer}");

        if (actionSequencer == null)
        {
            Debug.LogWarning("[TUTORIAL] ActionSequencer not assigned - skipping sequence playback");
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "ActionSequencer not assigned - skipping sequence playback", this);
            return;
        }

        float gridDelay = DisplaySettings.Instance.ActiveProfile.tutorialGridShowDelay;
        float buffer = DisplaySettings.Instance.ActiveProfile.tutorialSequenceStartBuffer;
        float totalDelay = gridDelay + buffer;

        Debug.Log($"[TUTORIAL] Starting sequence with delay: {totalDelay}s (gridDelay={gridDelay}, buffer={buffer})");
        Invoke(nameof(StartSequenceDelayed), totalDelay);
    }

    private void StartSequenceDelayed()
    {
        Debug.Log("[TUTORIAL] StartSequenceDelayed called - invoking actionSequencer.StartSequence()");
        actionSequencer.StartSequence();
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
