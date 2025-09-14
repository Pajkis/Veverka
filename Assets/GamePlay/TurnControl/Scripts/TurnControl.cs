using UnityEngine;
using System.Collections;

/// <summary>
/// Controls turn flow by tracking action starts/completions and managing input locking.
/// Handles chain reactions by counting active actions from Character, Nut, and Goal events.
/// Includes timeout mechanism for missing character start triggers.
/// </summary>
public class TurnControl : MonoBehaviour
{
    #region Singleton
    private static TurnControl instance;
    public static TurnControl Instance => instance;

    private void Awake()
    {
        Debug.Log("[TurnControl DEBUG] Awake called");
        if (instance == null)
        {
            Debug.Log("[TurnControl DEBUG] Setting up singleton instance");
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[TurnControl DEBUG] TurnControl singleton created successfully");
        }
        else
        {
            Debug.Log("[TurnControl DEBUG] Instance already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }
    #endregion

    #region Fields
    private int activeActionCount = 0;
    private bool inputLocked = false;
    private bool waitingForCharacterTrigger = false;
    private Coroutine timeoutCoroutine = null;

    [Header("Events")]
    [SerializeField] public CharacterEvents characterEvents;
    [SerializeField] public NutEvents nutEvents;
    [SerializeField] public GoalEvents goalEvents;
    
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
        Debug.Log("[TurnControl DEBUG] OnEnable called - registering event listeners");
        Debug.Log($"[TurnControl DEBUG] characterEvents={characterEvents != null}, nutEvents={nutEvents != null}, goalEvents={goalEvents != null}");
        
        characterEvents?.AddListener(OnCharacterEvent);
        nutEvents?.AddListener(OnNutEvent);
        goalEvents?.AddListener(OnGoalEvent);
        
        Debug.Log("[TurnControl DEBUG] Event listeners registered");
    }

    private void OnDisable()
    {
        characterEvents?.RemoveListener(OnCharacterEvent);
        nutEvents?.RemoveListener(OnNutEvent);
        goalEvents?.RemoveListener(OnGoalEvent);
    }

    /// <summary>
    /// Refresh event listeners - call this after event references are assigned
    /// </summary>
    public void RefreshEventListeners()
    {
        Debug.Log("[TurnControl DEBUG] RefreshEventListeners called");
        Debug.Log($"[TurnControl DEBUG] After assignment: characterEvents={characterEvents != null}, nutEvents={nutEvents != null}, goalEvents={goalEvents != null}");
        
        // Remove any existing listeners first
        characterEvents?.RemoveListener(OnCharacterEvent);
        nutEvents?.RemoveListener(OnNutEvent);
        goalEvents?.RemoveListener(OnGoalEvent);
        
        // Add listeners with the new event references
        characterEvents?.AddListener(OnCharacterEvent);
        nutEvents?.AddListener(OnNutEvent);
        goalEvents?.AddListener(OnGoalEvent);
        
        Debug.Log("[TurnControl DEBUG] Event listeners refreshed successfully");
    }
    #endregion

    #region Input Control
    /// <summary>
    /// Called when input is triggered - starts turn if not already active
    /// </summary>
    public void OnInputTriggered()
    {
        Debug.Log($"[TurnControl DEBUG] OnInputTriggered called. Current state: inputLocked={inputLocked}, waitingForTrigger={waitingForCharacterTrigger}");
        
        if (inputLocked) 
        {
            Debug.Log("[TurnControl DEBUG] Input already locked, ignoring trigger");
            return;
        }

        Debug.Log("[TurnControl DEBUG] Locking input and starting timeout...");
        LockInput();
        StartWaitingForCharacterTrigger();
        Debug.Log("[TurnControl] Input locked - waiting for character action trigger");
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
        
        Debug.Log("[TurnControl] Input unlocked - turn complete");
    }
    #endregion

    #region Character Trigger Timeout
    /// <summary>
    /// Start waiting for character action trigger with timeout
    /// </summary>
    private void StartWaitingForCharacterTrigger()
    {
        Debug.Log("[TurnControl DEBUG] StartWaitingForCharacterTrigger called");
        waitingForCharacterTrigger = true;
        timeoutCoroutine = StartCoroutine(CharacterTriggerTimeoutCoroutine());
        Debug.Log($"[TurnControl DEBUG] Timeout coroutine started. waitingForTrigger={waitingForCharacterTrigger}, timeoutCoroutine={timeoutCoroutine != null}");
    }

    /// <summary>
    /// Stop waiting for character trigger
    /// </summary>
    private void StopWaitingForCharacterTrigger()
    {
        Debug.Log($"[TurnControl DEBUG] StopWaitingForCharacterTrigger called. Current state: waitingForTrigger={waitingForCharacterTrigger}, timeoutCoroutine={timeoutCoroutine != null}");
        
        waitingForCharacterTrigger = false;
        
        if (timeoutCoroutine != null)
        {
            Debug.Log("[TurnControl DEBUG] Stopping timeout coroutine");
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
            Debug.Log("[TurnControl DEBUG] Timeout coroutine stopped successfully");
        }
        else
        {
            Debug.Log("[TurnControl DEBUG] No timeout coroutine to stop");
        }
    }

    /// <summary>
    /// Timeout coroutine for character action triggers
    /// </summary>
    private IEnumerator CharacterTriggerTimeoutCoroutine()
    {
        Debug.Log($"[TurnControl DEBUG] Timeout coroutine started, waiting {characterTriggerTimeout}s...");
        yield return new WaitForSeconds(characterTriggerTimeout);
        
        Debug.Log($"[TurnControl DEBUG] Timeout coroutine finished waiting. Current state: waitingForTrigger={waitingForCharacterTrigger}");
        
        if (waitingForCharacterTrigger)
        {
            Debug.LogWarning("[TurnControl DEBUG] TIMEOUT TRIGGERED! No character action trigger received, unlocking input");
            UnlockInput();
        }
        else
        {
            Debug.Log("[TurnControl DEBUG] Timeout coroutine finished but waitingForCharacterTrigger=false, so no timeout action taken");
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
        Debug.Log($"[TurnControl] Action started: {actionType} (Active: {activeActionCount})");
    }

    /// <summary>
    /// Decrement active action count and check if turn is complete
    /// </summary>
    private void CompleteAction(string actionType)
    {
        activeActionCount--;
        Debug.Log($"[TurnControl] Action completed: {actionType} (Active: {activeActionCount})");

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
        Debug.Log($"[TurnControl DEBUG] OnCharacterEvent called! EventType={payload.EventType}, waitingForTrigger={waitingForCharacterTrigger}");
        
        switch (payload.EventType)
        {
            case CharacterEventType.MoveStarted:
                Debug.Log("[TurnControl DEBUG] Processing MoveStarted event");
                if (waitingForCharacterTrigger)
                {
                    Debug.Log("[TurnControl DEBUG] MoveStarted: Stopping timeout and starting action");
                    StopWaitingForCharacterTrigger();
                    StartAction("Character Move");
                }
                else
                {
                    Debug.Log("[TurnControl DEBUG] MoveStarted: Not waiting for trigger, ignoring");
                }
                break;
                
            case CharacterEventType.MoveCompleted:
                Debug.Log("[TurnControl DEBUG] Processing MoveCompleted event");
                CompleteAction("Character Move");
                break;
                
            case CharacterEventType.RotateStarted:
                Debug.Log("[TurnControl DEBUG] Processing RotateStarted event");
                if (waitingForCharacterTrigger)
                {
                    Debug.Log("[TurnControl DEBUG] RotateStarted: Stopping timeout and starting action");
                    StopWaitingForCharacterTrigger();
                    StartAction("Character Rotate");
                }
                else
                {
                    Debug.Log("[TurnControl DEBUG] RotateStarted: Not waiting for trigger, ignoring");
                }
                break;
                
            case CharacterEventType.RotateCompleted:
                Debug.Log("[TurnControl DEBUG] Processing RotateCompleted event");
                CompleteAction("Character Rotate");
                break;
                
            case CharacterEventType.MoveFailed:
                Debug.Log("[TurnControl DEBUG] Processing MoveFailed event");
                if (waitingForCharacterTrigger)
                {
                    Debug.Log("[TurnControl DEBUG] MoveFailed: Stopping timeout and unlocking input");
                    StopWaitingForCharacterTrigger();
                    Debug.Log("[TurnControl] Character move failed - immediately unlocking input");
                    UnlockInput();
                }
                else
                {
                    Debug.Log("[TurnControl DEBUG] MoveFailed: Not waiting for trigger, ignoring");
                }
                break;
                
            default:
                Debug.Log($"[TurnControl DEBUG] Unknown event type: {payload.EventType}");
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
                StartAction("Nut Push");
                break;
            case NutEventType.NutMoved:
                CompleteAction("Nut Push");
                break;
            case NutEventType.NutInGoal:
                StartAction("Goal Action");
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
                CompleteAction("Goal Action");
                break;
        }
    }
    #endregion
}