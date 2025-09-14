using UnityEngine;

/// <summary>
/// Game play class
/// handles game play lost/win logic and events 
/// </summary>
public class GamePlay : MonoBehaviour
{
    #region fields
    int levelGoalCount = 0;
    int turnCount = 0;

    [Header("Display Components")]
    [SerializeField] private UniversalTextDisplay levelTypeDisplay;
    [SerializeField] private UniversalTextDisplay levelNumDisplay;
    [SerializeField] private UniversalTextDisplay goalDisplay;
    [SerializeField] private UniversalTextDisplay turnsDisplay;

    [Header("Events")]
    [SerializeField] private GoalEvents goalEvents;
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;

    [Header("Level Data")]
    [SerializeField] private LevelDatabase levelDatabase;

    [Header("turn undo logic")]
    private UndoManager undoManager;
    #endregion   

    /// <summary>
    /// Awake is called at object creation
    /// </summary>
    void Awake()
    {
        //undo manager initialization
        undoManager = new UndoManager(audioEvents);
        TurnBuilder.Instance.SetFinalizeCallback(undoManager.RegisterTurn);         
        
    }

    private void Start()
    {
        // initialize level goal count and turn count
        LevelInit();
    }

    /// <summary>
    /// Subscribes to relevant events and initializes the level display when the object is enabled.
    /// </summary>
    /// <remarks>This method adds listeners to the <see cref="goalEvents"/>, <see
    /// cref="characterEvents"/>, and other relevant events to handle game logic. It also updates the level display to
    /// reflect the current level index.</remarks>
    private void OnEnable()
    {
        // add listeners
        goalEvents.AddListener(UpdateGoalCount);
        characterEvents.AddListener(OnCharacterEvent);
    }
    /// <summary>
    /// OnDisable method - called when object deactivate or before destroy
    /// </summary>
    private void OnDisable()
    {
        goalEvents.RemoveListener(UpdateGoalCount);
        characterEvents.RemoveListener(OnCharacterEvent);
    }

    /// <summary>
    /// Handles frame-based input such as undo and opening the pause menu.
    /// </summary>
    void Update()
    {
        // undo last turn only if turn is not active
        if (Input.GetKeyDown(KeyCode.B) && FindObjectOfType<TurnControl>()?.IsInputLocked != true)
        {
            Undo();
        }

        // open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindWithTag("PauseMenu") == null)
            {
                sceneNavigationEvents.Raise(new SceneNavigationEventPayload
                {
                    EventType = SceneNavigationEventType.OpenOverlay,
                    Overlay = OverlayType.PauseMenu
                });
            }
        }
    }

    /// <summary>
    /// Set level goal count for current level
    /// </summary>
    /// <param name="goalCount"></param>
    void LevelInit() // LevelInitPayload payload
    {
        //Set turn count and goal count
        turnCount = 0;
        turnsDisplay.DisplayNumber(turnCount);

        if (levelDatabase == null || levelDatabase.GoalsCount == 0)
        {
            Debug.LogError("LevelDatabase is null or no goals set");
            return;
        }
        
        levelNumDisplay.DisplayNumber(levelDatabase.CurrentLevelIndex + 1);
        levelTypeDisplay.DisplayEnum<LevelSetType>(levelDatabase.LevelSetType);
        levelGoalCount = levelDatabase.GoalsCount; // payload.GoalCount;
        goalDisplay.DisplayNumber(levelGoalCount);
        Debug.Log($"Left goals: {levelGoalCount}");
    }

    /// <summary>
    /// Update goal count, check for level complete condition
    /// </summary>
    void UpdateGoalCount(GoalEventPayload payload)
    {
        if (payload.EventType != GoalEventsType.GoalResolved) return;
        levelGoalCount -= payload.GoalReduction;
        goalDisplay.DisplayNumber(levelGoalCount);
        Debug.Log($"Left goals: {levelGoalCount}");
        if (levelGoalCount <= 0) 
        {
            Debug.Log("Level completed");
            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.OpenOverlay,
                Overlay = OverlayType.LevelFinishedMenu
            });
        }

        // reset history recording
        TurnBuilder.Instance.CancelTurn();     
        undoManager.ClearHistory();
    }

      /// <summary>
      /// Handles character events and counts completed moves.
      /// </summary>
      /// <param name="payload"></param>
        private void OnCharacterEvent(CharacterEventPayload payload)
        {
            if (payload.EventType == CharacterEventType.MoveCompleted)
            {
                turnCount++;
                turnsDisplay.DisplayNumber(turnCount);
            }
        }

    /// <summary>
    /// Reverts the last recorded turn and updates the turn counter.
    /// </summary>
    public void Undo()
    {
        bool undoDone = undoManager.Undo();

        if (undoDone)
        {
            turnCount++;
            turnsDisplay.DisplayNumber(turnCount);
        }
    }
}
