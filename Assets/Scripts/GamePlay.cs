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
    int levelGoalCount = 0;
    #endregion

    /// <summary>
    /// Awake is called at object creation
    /// </summary>
    void Awake()
    {      
        // add listeners
        EventManager.AddListener(EventEnum.LevelGoalCountSetEvent, SetLevelGoalCount);
        EventManager.AddListener(EventEnum.NutInGoalEvent, UpdateGoalCount);
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
    void UpdateGoalCount(int goalReduction)
    {
        levelGoalCount -= goalReduction;
        Debug.Log($"Left goals: {levelGoalCount}");
        if (levelGoalCount <= 0) 
        {
            Debug.Log("Level completed");
            MenuManager.GoToMenu(MenuEnum.LevelFinishedMenu);
        }
    }
    
}
