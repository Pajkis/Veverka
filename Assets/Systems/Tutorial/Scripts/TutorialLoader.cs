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
    [SerializeField] private Transform gridRoot;

    // Cached reference to animation controller
    private OverlayAnimationController animController;

    private void Awake()
    {
        // Cache reference to root overlay animation controller
        animController = transform.root.GetComponent<OverlayAnimationController>();

        if (animController == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "OverlayAnimationController not found on root - close animation won't work!", this);
        }
    }

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
        DebugLogger.Log(DebugLogCategory.Tutorial, $"StartActionSequence called, actionSequencer={actionSequencer != null}", this);

        if (actionSequencer == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "ActionSequencer not assigned - skipping sequence playback", this);
            return;
        }

        float gridDelay = DisplaySettings.Instance.ActiveProfile.tutorialGridShowDelay;
        float buffer = DisplaySettings.Instance.ActiveProfile.tutorialSequenceStartBuffer;
        float totalDelay = gridDelay + buffer;

        DebugLogger.Log(DebugLogCategory.Tutorial, $"Starting sequence with delay: {totalDelay}s (gridDelay={gridDelay}s, buffer={buffer}s)", this);
        Invoke(nameof(StartSequenceDelayed), totalDelay);
    }

    private void StartSequenceDelayed()
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, "StartSequenceDelayed - invoking actionSequencer.StartSequence()", this);
        actionSequencer.StartSequence();
    }

    /// <summary>
    /// Close button handler - uses coroutine sequence for explicit control flow
    /// </summary>
    public void OnCloseButtonClick()
    {
        if (tutorialSelection == null || tutorialEvents == null) return;

        DebugLogger.Log(DebugLogCategory.Tutorial, "Close button clicked - starting close sequence", this);

        if (animController == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "OverlayAnimationController not found - cannot animate close", this);
            return;
        }

        StartCoroutine(CloseSequence());
    }

    /// <summary>
    /// Close sequence: Fade out tiles + UI → Raise event (camera moves) → Wait → Destroy
    /// TutorialLoader coordinates tile fade-out to ensure proper event sequencing.
    /// </summary>
    private System.Collections.IEnumerator CloseSequence()
    {
        // Get fade parameters from TransitionConfig
        float fadeOutDuration = animController.GetCloseAnimationDuration();
        UnityEngine.AnimationCurve fadeOutCurve = TransitionConfigProvider.Instance?.GetSettingsForOverlay(OverlayType.Tutorial).fadeOutCurve
            ?? UnityEngine.AnimationCurve.Linear(0, 0, 1, 1);

        DebugLogger.Log(DebugLogCategory.Tutorial, $"Starting close sequence - fade out duration: {fadeOutDuration}s", this);

        // 1. Find all GameObjectAnimator components under GridRoot (tiles, character)
        GameObjectAnimator[] tileAnimators = null;
        if (gridRoot != null)
        {
            tileAnimators = gridRoot.GetComponentsInChildren<GameObjectAnimator>();
            DebugLogger.Log(DebugLogCategory.Tutorial, $"Found {tileAnimators.Length} GameObjectAnimators to fade out", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "GridRoot not assigned - tiles won't fade out!", this);
        }

        // 2. Start fade-out for tiles and UI in parallel
        if (tileAnimators != null && tileAnimators.Length > 0)
        {
            // Start tile fade-out coroutines (don't wait yet)
            foreach (var animator in tileAnimators)
            {
                if (animator != null)
                {
                    StartCoroutine(animator.FadeOut(fadeOutDuration, fadeOutCurve));
                }
            }
        }

        // Start UI fade-out (FadeOutAsync waits for completion)
        yield return animController.FadeOutAsync();

        // 3. Tiles and UI are now invisible - raise ClearTutorial event (camera returns)
        DebugLogger.Log(DebugLogCategory.Tutorial, "Fade-out complete - raising ClearTutorial event for camera transition", this);
        tutorialEvents.Raise(new TutorialEventPayload
        {
            EventType = TutorialEventType.ClearTutorial,
            SetType = tutorialSelection.TutorialSetType,
            TutorialIndex = tutorialSelection.TutorialIndex
        });

        // 4. Small delay to let camera move smoothly
        DebugLogger.Log(DebugLogCategory.Tutorial, "Waiting for camera movement", this);
        yield return new WaitForSecondsRealtime(0.1f);

        // 5. Destroy overlay
        DebugLogger.Log(DebugLogCategory.Tutorial, "Destroying tutorial overlay", this);
        animController.DestroyImmediately();
    }
}
