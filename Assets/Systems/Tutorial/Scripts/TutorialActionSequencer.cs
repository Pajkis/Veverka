using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Plays tutorial action sequences - animations and messages.
/// Supports autoplay, skip, and replay functionality.
/// Sequence data is loaded via TutorialSetManager using SetType and Index.
/// </summary>
public class TutorialActionSequencer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridBuilder gridBuilder;
    [SerializeField] private DirectionEvent tutorialDirectionEvent;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TutorialSetManager tutorialSetManager;
    [SerializeField] private TutorialSelectData tutorialSelection;
    [SerializeField] private TurnControlEvents tutorialTurnControlEvents;
    [SerializeField] private UndoController undoController;

    // Runtime loaded sequence data
    private TutorialSequenceData sequenceData;

    // State
    private int currentActionIndex;
    private bool isPlaying;
    private bool isAutoplay = true;
    private bool isSkipRequested;
    private Coroutine playbackCoroutine;
    private CharVeverka tutorialCharacter;
    private bool turnCompleted;

    // Properties
    public bool IsPlaying => isPlaying;
    public bool IsAutoplay
    {
        get => isAutoplay;
        set => isAutoplay = value;
    }

    #region Unity Lifecycle

    private void OnEnable()
    {
        tutorialTurnControlEvents?.AddListener(OnTurnControlEvent);
    }

    private void OnDisable()
    {
        tutorialTurnControlEvents?.RemoveListener(OnTurnControlEvent);
    }

    private void OnTurnControlEvent(TurnControlEventPayload payload)
    {
        if (payload.EventType == TurnControlEventType.TurnCompleted)
        {
            turnCompleted = true;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start playing the sequence after grid is built.
    /// Loads sequence data from TutorialSetManager using current selection.
    /// </summary>
    public void StartSequence()
    {
        // Load sequence data from TutorialSetManager
        if (tutorialSetManager == null || tutorialSelection == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialSetManager or TutorialSelectData not assigned!", this);
            return;
        }

        TutorialSetType setType = tutorialSelection.TutorialSetType;
        int tutorialIndex = tutorialSelection.TutorialIndex;

        sequenceData = tutorialSetManager.GetTutorialSequence(setType, tutorialIndex);

        if (sequenceData == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                $"No sequence data for Set={setType}, Index={tutorialIndex}", this);
            return;
        }

        if (sequenceData.actions == null || sequenceData.actions.Count == 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "No actions in sequence", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"Loaded sequence '{sequenceData.tutorialId}' for Set={setType}, Index={tutorialIndex}", this);

        isAutoplay = sequenceData.autoplayEnabled;
        currentActionIndex = 0;
        isSkipRequested = false;

        // Find tutorial character
        tutorialCharacter = gridBuilder.FindCharacter();

        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
        }

        playbackCoroutine = StartCoroutine(PlaySequence());
    }

    /// <summary>
    /// Skip current message action.
    /// </summary>
    public void Skip()
    {
        isSkipRequested = true;
    }

    /// <summary>
    /// Skip all remaining actions.
    /// </summary>
    public void SkipAll()
    {
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
        }

        HideMessage();
        isPlaying = false;
        OnSequenceComplete();
    }

    /// <summary>
    /// Replay the sequence from beginning.
    /// </summary>
    public void Replay()
    {
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
        }

        HideMessage();
        StartSequence();
    }

    /// <summary>
    /// Toggle autoplay mode.
    /// </summary>
    public void ToggleAutoplay()
    {
        isAutoplay = !isAutoplay;
        DebugLogger.Log(DebugLogCategory.Tutorial, $"Autoplay: {isAutoplay}", this);
    }

    #endregion

    #region Playback Coroutines

    private IEnumerator PlaySequence()
    {
        isPlaying = true;

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"Starting sequence '{sequenceData.tutorialId}' with {sequenceData.actions.Count} actions", this);

        while (currentActionIndex < sequenceData.actions.Count)
        {
            TutorialAction action = sequenceData.actions[currentActionIndex];

            yield return ExecuteAction(action);

            currentActionIndex++;
        }

        isPlaying = false;
        OnSequenceComplete();
    }

    private IEnumerator ExecuteAction(TutorialAction action)
    {
        switch (action.ActionType)
        {
            case TutorialActionType.Animation:
                yield return ExecuteAnimation(action.AnimationDirection);
                break;

            case TutorialActionType.Message:
                float duration = action.MessageDisplayTime > 0
                    ? action.MessageDisplayTime
                    : GetDefaultMessageTime();
                yield return ExecuteMessage(action.MessageText, duration);
                break;

            case TutorialActionType.Undo:
                yield return ExecuteUndo();
                break;
        }
    }

    private IEnumerator ExecuteAnimation(Direction direction)
    {
        if (tutorialDirectionEvent == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialDirectionEvent is not assigned!", this);
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial, $"Animation action: {direction}", this);

        // Reset turn completed flag before raising direction event
        turnCompleted = false;

        // Raise direction event - this triggers the character movement/rotation
        // which will be tracked by TurnControl
        tutorialDirectionEvent.Raise(direction);

        // Wait for TurnCompleted event from TurnControl
        // This handles all chain reactions (character move, nut push, goal animation, etc.)
        if (tutorialTurnControlEvents != null)
        {
            yield return new WaitUntil(() => turnCompleted);
            DebugLogger.Log(DebugLogCategory.Tutorial, "Turn completed (via TurnControlEvents)", this);
        }
        else
        {
            // Fallback - wait for character animation only (no chain reaction support)
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "TutorialTurnControlEvents not assigned - falling back to direct animation wait", this);

            if (tutorialCharacter != null &&
                tutorialCharacter.SmoothMover != null &&
                tutorialCharacter.SmoothRotate != null)
            {
                yield return null; // Wait one frame for animation to start

                yield return new WaitUntil(() =>
                    !tutorialCharacter.SmoothMover.IsMoving &&
                    !tutorialCharacter.SmoothRotate.IsRotating);
            }
        }

        // Delay between actions
        yield return new WaitForSeconds(GetActionDelay());
    }

    private IEnumerator ExecuteUndo()
    {
        if (undoController == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "UndoController is not assigned!", this);
            yield break;
        }

        if (!undoController.CanUndo())
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "Cannot undo - no turn history available", this);
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial, "Undo action triggered", this);

        // Reset turn completed flag before requesting undo
        turnCompleted = false;

        // Request undo - this triggers the undo process
        // which will be tracked by TurnControl
        undoController.RequestUndo();

        // Wait for TurnCompleted event from TurnControl
        // UndoController handles unlocking input after completion
        if (tutorialTurnControlEvents != null)
        {
            yield return new WaitUntil(() => turnCompleted);
            DebugLogger.Log(DebugLogCategory.Tutorial, "Undo completed (via TurnControlEvents)", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "TutorialTurnControlEvents not assigned - cannot wait for undo completion", this);
        }

        // Delay between actions
        yield return new WaitForSeconds(GetActionDelay());
    }

    private IEnumerator ExecuteMessage(string text, float duration)
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, $"Message action: {text}", this);

        // Show message with fade in
        yield return ShowMessageWithFade(text);

        if (isAutoplay)
        {
            // Wait for duration or skip
            float elapsed = 0f;
            while (elapsed < duration && !isSkipRequested)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Wait for manual skip
            yield return new WaitUntil(() => isSkipRequested);
        }

        isSkipRequested = false;

        // Hide message with fade out
        yield return HideMessageWithFade();

        // Delay between actions
        yield return new WaitForSeconds(GetActionDelay());
    }

    #endregion

    #region Message Display

    private IEnumerator ShowMessageWithFade(string text)
    {
        if (messageText == null) yield break;

        messageText.text = text;
        messageText.gameObject.SetActive(true);

        // Fade in using text alpha
        float fadeIn = GetMessageFadeIn();
        SetTextAlpha(0f);

        float elapsed = 0f;
        while (elapsed < fadeIn)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(Mathf.Clamp01(elapsed / fadeIn));
            yield return null;
        }

        SetTextAlpha(1f);
    }

    private IEnumerator HideMessageWithFade()
    {
        if (messageText == null) yield break;

        // Fade out using text alpha
        float fadeOut = GetMessageFadeOut();

        float elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(Mathf.Clamp01(1f - (elapsed / fadeOut)));
            yield return null;
        }

        SetTextAlpha(0f);
        HideMessage();
    }

    private void SetTextAlpha(float alpha)
    {
        if (messageText == null) return;
        Color color = messageText.color;
        color.a = alpha;
        messageText.color = color;
    }

    private void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Config Getters

    private float GetDefaultMessageTime() =>
        DisplaySettings.Instance.ActiveProfile.tutorialDefaultMessageTime;

    private float GetActionDelay() =>
        DisplaySettings.Instance.ActiveProfile.tutorialActionDelay;

    private float GetMessageFadeIn() =>
        DisplaySettings.Instance.ActiveProfile.tutorialMessageFadeIn;

    private float GetMessageFadeOut() =>
        DisplaySettings.Instance.ActiveProfile.tutorialMessageFadeOut;

    #endregion

    #region Events

    private void OnSequenceComplete()
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, "Sequence complete", this);
    }

    #endregion
}
