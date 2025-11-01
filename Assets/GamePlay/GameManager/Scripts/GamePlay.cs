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
    [SerializeField] private LevelSelection levelSelection;
    [SerializeField] private LevelInitData levelInitData;

    [Header("Turn Undo Logic")]
    [SerializeField] private UndoController undoController;
    #endregion   

    /// <summary>
    /// Awake is called at object creation
    /// </summary>
    void Awake()
    {
        DebugLogger.Log(DebugLogCategory.Gameplay, "GamePlay Awake called", this);
        // Undo controller will be set via inspector
    }

    private void Start()
    {
        DebugLogger.Log(DebugLogCategory.Gameplay, "GamePlay Start called - initializing level", this);
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
        DebugLogger.Log(DebugLogCategory.Gameplay, "GamePlay OnEnable - adding event listeners", this);
        // add listeners
        goalEvents.AddListener(UpdateGoalCount);
        characterEvents.AddListener(OnCharacterEvent);
        DebugLogger.Log(DebugLogCategory.Gameplay, "Event listeners added for goal and character events", this);
    }
    /// <summary>
    /// OnDisable method - called when object deactivate or before destroy
    /// </summary>
    private void OnDisable()
    {
        DebugLogger.Log(DebugLogCategory.Gameplay, "GamePlay OnDisable - removing event listeners", this);
        goalEvents.RemoveListener(UpdateGoalCount);
        characterEvents.RemoveListener(OnCharacterEvent);
    }

    /// <summary>
    /// Handles frame-based input such as undo and opening the pause menu.
    /// </summary>
    void Update()
    {
        // undo last turn only if undo is available
        if (Input.GetKeyDown(KeyCode.B) && undoController != null && undoController.CanUndo())
        {
            DebugLogger.Log(DebugLogCategory.Gameplay, "Undo key (B) pressed - executing undo", this);
            Undo();
        }

        // open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindWithTag("PauseMenu") == null)
            {
                DebugLogger.Log(DebugLogCategory.Gameplay, "Escape key pressed - opening pause menu", this);
                sceneNavigationEvents.Raise(new SceneNavigationEventPayload
                {
                    EventType = SceneNavigationEventType.OpenOverlay,
                    Overlay = OverlayType.PauseMenu
                });
            }
            else
            {
                DebugLogger.Log(DebugLogCategory.Gameplay, "Escape key pressed but pause menu already exists", this);
            }
        }
    }

    /// <summary>
    /// Set level goal count for current level
    /// </summary>
    /// <param name="goalCount"></param>
    void LevelInit() // LevelInitPayload payload
    {
        DebugLogger.Log(DebugLogCategory.Gameplay, "LevelInit called - initializing level data", this);

        //Set turn count and goal count
        turnCount = 0;
        turnsDisplay.DisplayNumber(turnCount);
        DebugLogger.Log(DebugLogCategory.Gameplay, $"Turn count initialized to {turnCount}", this);

        if (levelInitData == null || levelInitData.StartGoalsCount == 0)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "LevelInitData is null or no goals set - cannot initialize level", this);
            return;
        }

        if (levelSelection == null)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "LevelSelection is null - cannot initialize level display", this);
            return;
        }

        levelNumDisplay.DisplayNumber(levelSelection.CurrentLevelIndex + 1);
        levelTypeDisplay.DisplayEnum<LevelSetType>(levelSelection.LevelSetType);
        levelGoalCount = levelInitData.StartGoalsCount;
        goalDisplay.DisplayNumber(levelGoalCount);

        DebugLogger.Log(DebugLogCategory.Gameplay, $"Level initialized - Level: {levelSelection.CurrentLevelIndex + 1}, Type: {levelSelection.LevelSetType}, Goals remaining: {levelGoalCount}", this);
    }

    /// <summary>
    /// Update goal count, check for level complete condition
    /// </summary>
    void UpdateGoalCount(GoalEventPayload payload)
    {
        if (payload.EventType != GoalEventsType.GoalResolve) return;

        int previousGoalCount = levelGoalCount;
        levelGoalCount -= payload.ScoreValue;
        goalDisplay.DisplayNumber(levelGoalCount);

        DebugLogger.Log(DebugLogCategory.Gameplay, $"Goal resolved - Goals: {previousGoalCount} → {levelGoalCount} (reduction: {payload.ScoreValue})", this);

        if (levelGoalCount <= 0)
        {
            DebugLogger.Log(DebugLogCategory.Gameplay, "All goals completed - level finished! Opening completion menu", this);

            // Play level finished sound
            if (audioEvents != null)
            {
                audioEvents.Raise(new AudioEventPayload
                {
                    EventType = AudioEventType.PlaySfx,
                    Sfx = SfxType.LevelFinished
                });
            }

            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.OpenOverlay,
                Overlay = OverlayType.LevelFinishedMenu
            });
        }

        // reset history recording
        if (undoController != null)
        {
            DebugLogger.Log(DebugLogCategory.Gameplay, "Clearing undo history after goal resolution", this);
            undoController.ClearHistory();
        }
    }

      /// <summary>
      /// Handles character events and counts completed moves.
      /// </summary>
      /// <param name="payload"></param>
        private void OnCharacterEvent(CharacterEventPayload payload)
        {
            DebugLogger.Log(DebugLogCategory.Gameplay, $"OnCharacterEvent received - EventType: {payload.EventType}", this);

            if (payload.EventType == CharacterEventType.MoveCompleted)
            {
                turnCount++;
                turnsDisplay.DisplayNumber(turnCount);
                DebugLogger.Log(DebugLogCategory.Gameplay, $"Character move completed - Turn count: {turnCount}", this);
            }
        }

    /// <summary>
    /// Reverts the last recorded turn and updates the turn counter.
    /// </summary>
    public void Undo()
    {
        if (undoController != null && undoController.CanUndo())
        {
            DebugLogger.Log(DebugLogCategory.Gameplay, "Undo requested - executing undo operation", this);
            undoController.RequestUndo();
            turnCount++;
            turnsDisplay.DisplayNumber(turnCount);
            DebugLogger.Log(DebugLogCategory.Gameplay, $"Undo completed - Turn count: {turnCount}", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay, "Cannot undo - no controller available or undo not possible", this);
        }
    }
}
