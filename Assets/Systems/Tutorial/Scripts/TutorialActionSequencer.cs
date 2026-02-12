using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TutorialSettingsConfig tutorialSettingsConfig;

    [Header("Runtime Turn State")]
    [SerializeField] private TurnState tutorialTurnState;

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

    // Speed control
    private TutorialAnimSpeedType textSpeedIndex = TutorialAnimSpeedType.Normal;
    private TutorialAnimSpeedType animSpeedIndex = TutorialAnimSpeedType.Normal;
    private bool isWaitingForNext;
    private bool currentMessageFadedIn;

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

        // Load tutorial settings from config (user preferences)
        isAutoplay = tutorialSettingsConfig.AutoplayEnable;
        textSpeedIndex = tutorialSettingsConfig.TextSpeedIndex;
        animSpeedIndex = tutorialSettingsConfig.AnimSpeedIndex;

        // Initialize TurnState with saved speed from settings
        if (tutorialTurnState != null && tutorialSettingsConfig != null)
        {
            float savedSpeed = (float)tutorialSettingsConfig.AnimSpeedIndex;
            tutorialTurnState.CurrentSpeedMultiplier = savedSpeed;

            DebugLogger.Log(DebugLogCategory.Tutorial,
                $"TutorialTurnState initialized with saved speed: {savedSpeed}", this);
        }

        currentActionIndex = 0;
        isSkipRequested = false;
        isWaitingForNext = false;

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
    /// Replay the sequence from beginning with fade out transition.
    /// </summary>
    public void Replay()
    {
        StartCoroutine(ReplayCoroutine());
    }

    private IEnumerator ReplayCoroutine()
    {
        // Stop current playback
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
        }

        // Hide message immediately
        HideMessage();

        // Fade out grid
        yield return StartCoroutine(gridBuilder.FadeOutGrid());

        // Get CSV data and parse grid for rebuild
        TutorialSetType setType = tutorialSelection.TutorialSetType;
        int tutorialIndex = tutorialSelection.TutorialIndex;
        TextAsset csvData = tutorialSetManager.GetTutorialCsv(setType, tutorialIndex);

        if (csvData != null)
        {
            TileType[,] grid = TileParser.LoadGridFromTextAsset(csvData);
            if (grid != null)
            {
                // Build level (fade-in handled by GameObjectAnimator automatically)
                gridBuilder.BuildLevel(grid);
            }
        }

        // Wait a frame for build to complete
        yield return null;

        // Restart sequence
        StartSequence();
    }

    /// <summary>
    /// Toggle autoplay mode and save to config.
    /// </summary>
    public void ToggleAutoplay()
    {
        isAutoplay = !isAutoplay;
        tutorialSettingsConfig.TutorialSetAutoplay(isAutoplay);
        DebugLogger.Log(DebugLogCategory.Tutorial, $"Autoplay: {isAutoplay}", this);
    }

    /// <summary>
    /// Request next action (called by Next button in manual mode).
    /// </summary>
    public void RequestNext()
    {
        isSkipRequested = true;
    }

    /// <summary>
    /// Set text speed and save to config.
    /// </summary>
    public void SetTextSpeed(TutorialAnimSpeedType speed)
    {
        textSpeedIndex = speed;
        tutorialSettingsConfig.TutorialSetTextSpeed(speed);
        DebugLogger.Log(DebugLogCategory.Tutorial, $"Text speed: {speed}", this);
    }

    /// <summary>
    /// Set animation speed and save to config.
    /// </summary>
    public void SetAnimSpeed(TutorialAnimSpeedType speed)
    {
        animSpeedIndex = speed;
        tutorialSettingsConfig.TutorialSetAnimSpeed(speed);

        // Update TurnState SO
        if (tutorialTurnState != null)
        {
            tutorialTurnState.CurrentSpeedMultiplier = (float)speed;

            DebugLogger.Log(DebugLogCategory.Tutorial,
                $"TutorialTurnState updated: speedMultiplier = {(float)speed}", this);
        }

        DebugLogger.Log(DebugLogCategory.Tutorial, $"Animation speed: {speed}", this);
    }

    /// <summary>
    /// Get current text speed setting.
    /// </summary>
    public TutorialAnimSpeedType GetCurrentTextSpeed() => textSpeedIndex;

    /// <summary>
    /// Get current animation speed setting.
    /// </summary>
    public TutorialAnimSpeedType GetCurrentAnimSpeed() => animSpeedIndex;

    /// <summary>
    /// Check if Next button should be active (manual mode and message is visible or action complete).
    /// </summary>
    public bool IsNextButtonActive()
    {
        return !isAutoplay && (currentMessageFadedIn || !isPlaying);
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
            case TutorialActionType.MovementSequence:
                yield return ExecuteMovementSequence(action.MovementSteps);
                break;

            case TutorialActionType.MessageSequence:
                yield return ExecuteMessageSequence(action.MessageSteps);
                break;

            case TutorialActionType.UndoSequence:
                yield return ExecuteUndoSequence(action.UndoCount);
                break;

            case TutorialActionType.ImageSequence:
                yield return ExecuteImageAction(action.ImageAction);
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

        // Raise direction event with tutorial speedMultiplier
        // This triggers the character movement/rotation which will be tracked by TurnControl
        float tutorialAnimSpeed = (float)animSpeedIndex;
        tutorialDirectionEvent.Raise(new DirectionPayload
        {
            direction = direction,          
        });

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

        // Delay between actions (adjusted by animation speed)
        float actionDelay = GetActionDelay() / tutorialAnimSpeed;
        yield return new WaitForSeconds(actionDelay);
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

        // Delay between actions (adjusted by animation speed)
        float animMultiplier = (float)animSpeedIndex;
        float actionDelay = GetActionDelay() / animMultiplier;
        yield return new WaitForSeconds(actionDelay);
    }

    private IEnumerator ExecuteMovementSequence(List<MovementStep> steps)
    {
        if (steps == null || steps.Count == 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "MovementSequence has no steps", this);
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"MovementSequence: {steps.Count} steps", this);

        foreach (var step in steps)
        {
            // Execute direction action repeatCount times
            for (int i = 0; i < step.RepeatCount; i++)
            {
                yield return ExecuteAnimation(step.Direction);
            }
        }
    }

    private IEnumerator ExecuteMessageSequence(List<MessageStep> steps)
    {
        if (steps == null || steps.Count == 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "MessageSequence has no steps", this);
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"MessageSequence: {steps.Count} messages", this);

        foreach (var step in steps)
        {
            float duration = step.DisplayTime > 0
                ? step.DisplayTime
                : GetDefaultMessageTime();
            yield return ExecuteMessage(step.MessageText, duration);
        }
    }

    private IEnumerator ExecuteUndoSequence(int count)
    {
        if (count <= 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "UndoSequence has invalid count", this);
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"UndoSequence: {count} undos", this);

        for (int i = 0; i < count; i++)
        {
            if (!undoController.CanUndo())
            {
                DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                    $"Undo stopped at {i + 1}/{count} - no more history", this);
                yield break;
            }

            yield return ExecuteUndo();
        }
    }

    private IEnumerator ExecuteImageAction(ImageStep imageStep)
    {
        if (imageStep.Image == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "ImageAction has no sprite assigned", this);
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"ImageAction: {imageStep.ActionType}", this);

        // TODO: Implement image display logic
        // - Show/hide/replace image in tutorial overlay
        // - Handle ExecuteWithMessage flag
        // - Apply display duration
        // - Respect text speed multiplier for timings

        // Placeholder for future implementation
        float duration = imageStep.DisplayTime > 0
            ? imageStep.DisplayTime
            : GetDefaultMessageTime();
        yield return new WaitForSeconds(duration);
    }

    private IEnumerator ExecuteMessage(string text, float duration)
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, $"Message action: {text}", this);

        // Apply text speed to all timings
        float textMultiplier = (float)textSpeedIndex;
        float fadeInDuration = GetMessageFadeIn() / textMultiplier;
        float fadeOutDuration = GetMessageFadeOut() / textMultiplier;
        float displayTime = duration / textMultiplier;

        // Show message with fade in
        yield return ShowMessageWithFade(text, fadeInDuration);

        if (isAutoplay)
        {
            // Autoplay mode: wait for duration or skip
            float elapsed = 0f;
            while (elapsed < displayTime && !isSkipRequested)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Manual mode: message is faded in, activate Next button
            currentMessageFadedIn = true;

            // Wait for manual skip OR autoplay toggle
            yield return new WaitUntil(() => isSkipRequested || isAutoplay);

            currentMessageFadedIn = false;
        }

        isSkipRequested = false;

        // Hide message with fade out
        yield return HideMessageWithFade(fadeOutDuration);

        // Delay between actions (adjusted by text speed)
        float actionDelay = GetActionDelay() / textMultiplier;
        yield return new WaitForSeconds(actionDelay);
    }

    #endregion

    #region Message Display

    private IEnumerator ShowMessageWithFade(string text, float fadeInDuration)
    {
        if (messageText == null) yield break;

        messageText.text = text;
        messageText.gameObject.SetActive(true);

        // Fade in using text alpha (duration adjusted by text speed)
        SetTextAlpha(0f);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(Mathf.Clamp01(elapsed / fadeInDuration));
            yield return null;
        }

        SetTextAlpha(1f);
    }

    private IEnumerator HideMessageWithFade(float fadeOutDuration)
    {
        if (messageText == null) yield break;

        // Fade out using text alpha (duration adjusted by text speed)
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            SetTextAlpha(Mathf.Clamp01(1f - (elapsed / fadeOutDuration)));
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
