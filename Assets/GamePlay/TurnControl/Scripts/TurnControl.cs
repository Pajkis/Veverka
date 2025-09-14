using UnityEngine;
using System.Collections;

/// <summary>
/// Controls turn flow by tracking action starts/completions and managing input locking.
/// Handles chain reactions by counting active actions from Character, Nut, and Goal events.
/// Includes timeout mechanism for missing character start triggers.
/// </summary>
public class TurnControl : MonoBehaviour
{
    private void Awake()
    {
        if (debugConfig != null) DebugLogger.Initialize(debugConfig);

        // Reset state for fresh level start
        activeActionCount = 0;
        inputLocked = false;
        waitingForCharacterTrigger = false;
        timeoutCoroutine = null;

        DebugLogger.Log(DebugLogCategory.TurnControl, "TurnControl initialized with clean state", this);
    }

    #region Fields
    private int activeActionCount = 0;
    private bool inputLocked = false;
    private bool waitingForCharacterTrigger = false;
    private Coroutine timeoutCoroutine = null;

    [Header("Events")]
    [SerializeField] public CharacterEvents characterEvents;
    [SerializeField] public NutEvents nutEvents;
    [SerializeField] public GoalEvents goalEvents;
    
    [Header("Debug")]
    [SerializeField] private DebugLogConfig debugConfig;
    
    [Header("Settings")]
    [SerializeField] private float characterTriggerTimeout = 0.5f;
    #endregion

    #region Properties
    public bool IsInputLocked => inputLocked;
    public int ActiveActionCount => activeActionCount;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        DebugLogger.Log(DebugLogCategory.TurnControl, "OnEnable called - registering event listeners", this);
        DebugLogger.Log(DebugLogCategory.TurnControl, 
            $"characterEvents={characterEvents != null}, nutEvents={nutEvents != null}, goalEvents={goalEvents != null}", this);
        
        characterEvents?.AddListener(OnCharacterEvent);
        nutEvents?.AddListener(OnNutEvent);
        goalEvents?.AddListener(OnGoalEvent);
        
        DebugLogger.Log(DebugLogCategory.TurnControl, "Event listeners registered", this);
    }

    private void OnDisable()
    {
        characterEvents?.RemoveListener(OnCharacterEvent);
        nutEvents?.RemoveListener(OnNutEvent);
        goalEvents?.RemoveListener(OnGoalEvent);
    }

    #endregion

    #region Input Control
    /// <summary>
    /// Called when input is triggered - starts turn if not already active
    /// </summary>
    public void OnInputTriggered()
    {
        DebugLogger.Log(DebugLogCategory.TurnControl, 
            $"OnInputTriggered called. Current state: inputLocked={inputLocked}, waitingForTrigger={waitingForCharacterTrigger}", this);
        
        if (inputLocked) 
        {
            DebugLogger.Log(DebugLogCategory.TurnControl, "Input already locked, ignoring trigger", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.TurnControl, "Locking input and starting timeout...", this);
        LockInput();
        StartWaitingForCharacterTrigger();
        DebugLogger.Log(DebugLogCategory.TurnControl, "Input locked - waiting for character action trigger", this);
    }

    /// <summary>
    /// Lock input to prevent new actions
    /// </summary>
    private void LockInput()
    {
        inputLocked = true;
    }

    /// <summary>
    /// Unlock input when all actions complete
    /// </summary>
    private void UnlockInput()
    {
        inputLocked = false;
        waitingForCharacterTrigger = false;
        
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
        }
        
        DebugLogger.Log(DebugLogCategory.TurnControl, "Input unlocked - turn complete", this);
    }
    #endregion

    #region Character Trigger Timeout
    /// <summary>
    /// Start waiting for character action trigger with timeout
    /// </summary>
    private void StartWaitingForCharacterTrigger()
    {
        DebugLogger.Log(DebugLogCategory.TurnControl, "StartWaitingForCharacterTrigger called", this);
        waitingForCharacterTrigger = true;
        timeoutCoroutine = StartCoroutine(CharacterTriggerTimeoutCoroutine());
        DebugLogger.Log(DebugLogCategory.TurnControl, 
            $"Timeout coroutine started. waitingForTrigger={waitingForCharacterTrigger}, timeoutCoroutine={timeoutCoroutine != null}", this);
    }

    /// <summary>
    /// Stop waiting for character trigger
    /// </summary>
    private void StopWaitingForCharacterTrigger()
    {
        DebugLogger.Log(DebugLogCategory.TurnControl, 
            $"StopWaitingForCharacterTrigger called. Current state: waitingForTrigger={waitingForCharacterTrigger}, timeoutCoroutine={timeoutCoroutine != null}", this);
        
        waitingForCharacterTrigger = false;
        
        if (timeoutCoroutine != null)
        {
            DebugLogger.Log(DebugLogCategory.TurnControl, "Stopping timeout coroutine", this);
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
            DebugLogger.Log(DebugLogCategory.TurnControl, "Timeout coroutine stopped successfully", this);
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.TurnControl, "No timeout coroutine to stop", this);
        }
    }

    /// <summary>
    /// Timeout coroutine for character action triggers
    /// </summary>
    private IEnumerator CharacterTriggerTimeoutCoroutine()
    {
        DebugLogger.Log(DebugLogCategory.TurnControl, $"Timeout coroutine started, waiting {characterTriggerTimeout}s...", this);
        yield return new WaitForSeconds(characterTriggerTimeout);
        
        DebugLogger.Log(DebugLogCategory.TurnControl, 
            $"Timeout coroutine finished waiting. Current state: waitingForTrigger={waitingForCharacterTrigger}", this);
        
        if (waitingForCharacterTrigger)
        {
            DebugLogger.LogWarning(DebugLogCategory.TurnControl, "TIMEOUT TRIGGERED! No character action trigger received, unlocking input", this);
            UnlockInput();
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.TurnControl, "Timeout coroutine finished but waitingForCharacterTrigger=false, so no timeout action taken", this);
        }
    }
    #endregion

    #region Action Tracking
    /// <summary>
    /// Increment active action count
    /// </summary>
    private void StartAction(string actionType)
    {
        activeActionCount++;
        DebugLogger.Log(DebugLogCategory.TurnControl, $"Action started: {actionType} (Active: {activeActionCount})", this);
    }

    /// <summary>
    /// Decrement active action count and check if turn is complete
    /// </summary>
    private void CompleteAction(string actionType)
    {
        activeActionCount--;
        DebugLogger.Log(DebugLogCategory.TurnControl, $"Action completed: {actionType} (Active: {activeActionCount})", this);

        if (activeActionCount <= 0)
        {
            activeActionCount = 0; // Safety clamp
            UnlockInput();
        }
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handle character events for movement, rotation, and failed actions
    /// </summary>
    private void OnCharacterEvent(CharacterEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.TurnControl, 
            $"OnCharacterEvent called! EventType={payload.EventType}, waitingForTrigger={waitingForCharacterTrigger}", this);
        
        switch (payload.EventType)
        {
            case CharacterEventType.MoveStarted:
                DebugLogger.Log(DebugLogCategory.TurnControl, "Processing MoveStarted event", this);
                if (waitingForCharacterTrigger)
                {
                    DebugLogger.Log(DebugLogCategory.TurnControl, "MoveStarted: Stopping timeout and starting action", this);
                    StopWaitingForCharacterTrigger();
                    StartAction($"Character Move ({payload.PreviousPosition.x},{payload.PreviousPosition.y})");
                }
                else
                {
                    DebugLogger.Log(DebugLogCategory.TurnControl, "MoveStarted: Not waiting for trigger, ignoring", this);
                }
                break;

            case CharacterEventType.MoveCompleted:
                DebugLogger.Log(DebugLogCategory.TurnControl, "Processing MoveCompleted event", this);
                if (payload.PreviousPosition == payload.CurrentPosition)
                {
                    CompleteAction($"Character Move ({payload.PreviousPosition.x},{payload.PreviousPosition.y})");
                }
                else
                {
                    CompleteAction($"Character Move ({payload.PreviousPosition.x},{payload.PreviousPosition.y}) → ({payload.CurrentPosition.x},{payload.CurrentPosition.y})");
                }
                break;
                
            case CharacterEventType.RotateStarted:
                DebugLogger.Log(DebugLogCategory.TurnControl, "Processing RotateStarted event", this);
                if (waitingForCharacterTrigger)
                {
                    DebugLogger.Log(DebugLogCategory.TurnControl, "RotateStarted: Stopping timeout and starting action", this);
                    StopWaitingForCharacterTrigger();
                    StartAction($"Character Rotate ({payload.CurrentPosition.x},{payload.CurrentPosition.y})");
                }
                else
                {
                    DebugLogger.Log(DebugLogCategory.TurnControl, "RotateStarted: Not waiting for trigger, ignoring", this);
                }
                break;

            case CharacterEventType.RotateCompleted:
                DebugLogger.Log(DebugLogCategory.TurnControl, "Processing RotateCompleted event", this);
                CompleteAction($"Character Rotate ({payload.CurrentPosition.x},{payload.CurrentPosition.y})");
                break;
                
            case CharacterEventType.MoveFailed:
                DebugLogger.Log(DebugLogCategory.TurnControl, "Processing MoveFailed event", this);
                if (waitingForCharacterTrigger)
                {
                    DebugLogger.Log(DebugLogCategory.TurnControl, "MoveFailed: Stopping timeout and unlocking input", this);
                    StopWaitingForCharacterTrigger();
                    DebugLogger.Log(DebugLogCategory.TurnControl, "Character move failed - immediately unlocking input", this);
                    UnlockInput();
                }
                else
                {
                    DebugLogger.Log(DebugLogCategory.TurnControl, "MoveFailed: Not waiting for trigger, ignoring", this);
                }
                break;
                
            default:
                DebugLogger.Log(DebugLogCategory.TurnControl, $"Unknown event type: {payload.EventType}", this);
                break;
        }
    }

    /// <summary>
    /// Handle nut events for movement and goal interaction tracking
    /// </summary>
    private void OnNutEvent(NutEventPayload payload)
    {
        switch (payload.EventType)
        {
            case NutEventType.NutPush:
                StartAction($"Nut Push ({payload.PreviousPosition.x},{payload.PreviousPosition.y})");
                break;
            case NutEventType.NutMoved:
                if (payload.PreviousPosition == payload.CurrentPosition)
                {
                    CompleteAction($"Nut Push ({payload.PreviousPosition.x},{payload.PreviousPosition.y})");
                }
                else
                {
                    CompleteAction($"Nut Push ({payload.PreviousPosition.x},{payload.PreviousPosition.y}) → ({payload.CurrentPosition.x},{payload.CurrentPosition.y})");
                }
                break;
            case NutEventType.NutEnteringGoal:
                StartAction($"Goal Action ({payload.CurrentPosition.x},{payload.CurrentPosition.y})");
                break;
            case NutEventType.NutInGoal:
                // NutInGoal is processed by GoalTile, no action needed in TurnControl
                break;
        }
    }

    /// <summary>
    /// Handle goal events for goal action completion tracking
    /// </summary>
    private void OnGoalEvent(GoalEventPayload payload)
    {
        switch (payload.EventType)
        {
            case GoalEventsType.NutInGoalDone:
                CompleteAction($"Goal Action ({payload.Position.x},{payload.Position.y})");
                break;
        }
    }
    #endregion
}