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
        Debug.Log("[GamePlay] FinalizeCallback set for TurnBuilder");
    }

    private void OnEnable()
    {
        // add listeners     
        pushableInGoalEvent.AddListener(UpdateGoalCount);
        levelInitEvent.AddListener(LevelInit);
        characterMoved.AddListener(OnCharacterMoved);
    }

    // Update is called once per frame
    void Update()
    {

        // undo last turn
        if (Input.GetKeyDown(KeyCode.B))
        {
            undoManager.Undo();
        }

        // open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindWithTag("PauseMenu") == null)
            {
                MenuManager.GoToMenu(MenuEnum.PauseMenu);
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
        characterMoved.AddListener(OnCharacterMoved);
    }

    /// <summary>
    /// Set level goal count for current level
    /// </summary>
    /// <param name="goalCount"></param>
    void LevelInit(LevelInitPayload payload)
    {
        turnCount = 0;
        levelGoalCount = payload.GoalCount;
        Debug.Log($"Left goals: {levelGoalCount}");
    }

    /// <summary>
    /// Update goal count, check for level complete condition
    /// </summary>
    void UpdateGoalCount(PushableInGoalPayload payload)
    {        
        levelGoalCount -= payload.goalReduction;
        Debug.Log($"Left goals: {levelGoalCount}");
        if (levelGoalCount <= 0) 
        {
            Debug.Log("Level completed");
            MenuManager.GoToMenu(MenuEnum.LevelFinishedMenu);
        }
    }

    /// <summary>
    /// On character moved method - counts number of moves and add each move into turn record
    /// </summary>
    /// <param name="payload"></param>
    private void OnCharacterMoved(CharacterMovedPayload payload)
    {
        turnCount++;
    }

    /// <summary>
    /// Undo move for character
    /// </summary>
    public void Undo() => undoManager.Undo();


}
