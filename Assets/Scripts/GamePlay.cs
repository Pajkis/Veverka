using UnityEngine;

/// <summary>
/// Game play class
/// handles game play lost/win logic and events 
/// </summary>
public class GamePlay :  MonoBehaviour
{
    #region fields
    int levelGoalCount = 0;
    int turnCount = 0;

    [Header("UI display")]
    [SerializeField]
    private SpriteNumberDisplay goalCountDisplay;

    [SerializeField]
    private SpriteNumberDisplay turnCountDisplay;

    [SerializeField]
    private SpriteNumberDisplay levelDisplay;

    [SerializeField]
    private LevelDatabase levelDatabase;

    [Header("Events")]
    [SerializeField]
    private PushableInGoalEvent pushableInGoalEvent;

    [SerializeField]
    private LevelInitEvent levelInitEvent;

    [SerializeField]  
    private CharacterMovedEvent characterMoved;

    [Header("turn undo logic")]
    private UndoManager undoManager = new();
    #endregion   

    /// <summary>
    /// Awake is called at object creation
    /// </summary>
    void Awake()
    {
        undoManager = new UndoManager();
        TurnBuilder.Instance.SetFinalizeCallback(undoManager.RegisterTurn);
       // Debug.Log("[GamePlay] FinalizeCallback set for TurnBuilder");
    }

    private void OnEnable()
    {
        // add listeners     
        pushableInGoalEvent.AddListener(UpdateGoalCount);
        levelInitEvent.AddListener(LevelInit);
        characterMoved.AddListener(OnCharacterMoved);

        levelDisplay.SetNumber(levelDatabase.CurrentLevelIndex);
    }

    /// <summary>
    /// Handles frame-based input such as undo and opening the pause menu.
    /// </summary>
    void Update()
    {

        // undo last turn
        if (Input.GetKeyDown(KeyCode.B))
        {
            Undo();
        }

        // open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindWithTag("PauseMenu") == null)
            {
                SceneManager.GoToScene(SceneType.PauseMenu);
            }
        }
    }

    /// <summary>
    /// OnDisable method - called when object deactivate or before destroy
    /// </summary>
    private void OnDisable()
    {
        pushableInGoalEvent.RemoveListener(UpdateGoalCount);
        levelInitEvent.RemoveListener(LevelInit);
        characterMoved.RemoveListener(OnCharacterMoved);
    }

    /// <summary>
    /// Set level goal count for current level
    /// </summary>
    /// <param name="goalCount"></param>
    void LevelInit(LevelInitPayload payload)
    {
        turnCount = 0;
        turnCountDisplay.SetNumber(turnCount);
        levelGoalCount = payload.GoalCount;
        goalCountDisplay.SetNumber(levelGoalCount);
        Debug.Log($"Left goals: {levelGoalCount}");
    }

    /// <summary>
    /// Update goal count, check for level complete condition
    /// </summary>
    void UpdateGoalCount(PushableInGoalPayload payload)
    {        
        levelGoalCount -= payload.GoalReduction;
        goalCountDisplay.SetNumber(levelGoalCount);
        Debug.Log($"Left goals: {levelGoalCount}");
        if (levelGoalCount <= 0) 
        {
            Debug.Log("Level completed");
            SceneManager.GoToScene(SceneType.LevelFinishedMenu);
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
        turnCountDisplay.SetNumber(turnCount);
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
            turnCountDisplay.SetNumber(turnCount);
        }
    }
   


}
