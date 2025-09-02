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
    [SerializeField] private GoalRemovedEvent goalRemovedEvent;   
    [SerializeField] private CharacterMovedEvent characterMoved;
    [SerializeField] private PlaySfxEvent playSfxEvent;
    [SerializeField] private OpenOverlayEvent openOverlayEvent;

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
        undoManager = new UndoManager(playSfxEvent);
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
    /// <remarks>This method adds listeners to the <see cref="pushableInGoalEvent"/>, <see
    /// cref="characterMoved"/>,  and other relevant events to handle game logic. It also updates the level display to
    /// reflect the  current level index.</remarks>
    private void OnEnable()
    {
        // add listeners     
        goalRemovedEvent.AddListener(UpdateGoalCount);     
        characterMoved.AddListener(OnCharacterMoved);

       
    }

    /// <summary>
    /// Handles frame-based input such as undo and opening the pause menu.
    /// </summary>
    void Update()
    {
        // undo last turn only if no object is currently moving
        if (Input.GetKeyDown(KeyCode.B) && !SmoothMover.AnyMoving)
        {
            Undo();
        }

        // open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindWithTag("PauseMenu") == null)
            {
                openOverlayEvent.Raise(OverlayType.PauseMenu);
            }
        }
    }

    /// <summary>
    /// OnDisable method - called when object deactivate or before destroy
    /// </summary>
    private void OnDisable()
    {
        goalRemovedEvent.RemoveListener(UpdateGoalCount);     
        characterMoved.RemoveListener(OnCharacterMoved);
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
    void UpdateGoalCount(GoalBasicPayload payload)
    {        
        levelGoalCount -= payload.GoalReduction;
        goalDisplay.DisplayNumber(levelGoalCount);
        Debug.Log($"Left goals: {levelGoalCount}");
        if (levelGoalCount <= 0) 
        {
            Debug.Log("Level completed");
            openOverlayEvent.Raise(OverlayType.LevelFinishedMenu);
        }

        // reset history recording
        TurnBuilder.Instance.CancelTurn();     
        undoManager.ClearHistory();
    }

    /// <summary>
    /// On character moved method - counts number of moves and add each move into turn record
    /// </summary>
    /// <param name="payload"></param>
    private void OnCharacterMoved(CharacterMovedPayload payload)
    {
        turnCount++;
        turnsDisplay.DisplayNumber(turnCount);
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
