using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Game play class
/// handles game play lost/win logic and events 
/// </summary>
public class GamePlay :  IntEventInvoker
{
    #region fields
    [SerializeField]
    private PushableInGoalEvent pushableInGoalEvent;

    int levelGoalCount = 0;
    #endregion

    /// <summary>
    /// Awake is called at object creation
    /// </summary>
    void Awake()
    {      
        // add listeners
        EventManager.AddListener(EventEnum.LevelGoalCountSetEvent, SetLevelGoalCount);
        pushableInGoalEvent.AddListener(UpdateGoalCount);
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    /// <summary>
    /// Set level goal count for current level
    /// </summary>
    /// <param name="goalCount"></param>
    void SetLevelGoalCount(int goalCount)
    {       
        levelGoalCount = goalCount;
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
    
}
