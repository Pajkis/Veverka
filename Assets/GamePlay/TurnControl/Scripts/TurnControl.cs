using UnityEngine;

/// <summary>
/// Controls turn flow by tracking action starts/completions and managing input locking.
/// Handles chain reactions by counting active actions from Character, Nut, and Goal events.
/// </summary>
public class TurnControl : MonoBehaviour
{
    #region Singleton
    private static TurnControl instance;
    public static TurnControl Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Fields
    private int activeActionCount = 0;
    private bool inputLocked = false;

    [Header("Events")]
    [SerializeField] public CharacterEvents characterEvents;
    [SerializeField] public NutEvents nutEvents;
    [SerializeField] public GoalEvents goalEvents;
    #endregion

    #region Properties
    public bool IsInputLocked => inputLocked;
    public int ActiveActionCount => activeActionCount;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        characterEvents?.AddListener(OnCharacterEvent);
        nutEvents?.AddListener(OnNutEvent);
        goalEvents?.AddListener(OnGoalEvent);
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
        if (inputLocked) return;

        LockInput();
        Debug.Log("[TurnControl] Input locked - turn started");
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
        Debug.Log("[TurnControl] Input unlocked - turn complete");
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
    /// Handle character events for movement tracking
    /// </summary>
    private void OnCharacterEvent(CharacterEventPayload payload)
    {
        switch (payload.EventType)
        {
            case CharacterEventType.MoveStarted:
                StartAction("Character Move");
                break;
            case CharacterEventType.MoveCompleted:
                CompleteAction("Character Move");
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