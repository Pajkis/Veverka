using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Pause Menu handling class
/// </summary>
public class PauseMenu : MonoBehaviour
{
    #region fields
    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

    [SerializeField]
    private ResetGridEvent resetGridEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0; //  *not necessary in logic games, where time does not matter*         
    }

    /// <summary>
    /// handles on click resume button event
    /// </summary>
    public void HandleResumeButtonOnClickEvent()
    {
        Time.timeScale = 1;
        Destroy(gameObject);    
    }

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        Time.timeScale = 1;       
       
        //Raise level start event
        levelSelectEvent.Raise(new LevelSelectPayload
        {
            levelNumber = levelDatabase.CurrentLevelIndex,
            resetRequested = true,
        });
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles game settings menu on click event
    /// </summary>
    public void HandleGameSettingsButtonOnClickEvent()
    {
        MenuManager.GoToMenu(MenuEnum.SettingsMenu);
    }

    /// <summary>
    /// Handles on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        Time.timeScale = 1;
        
        // raise event to reset grid
        resetGridEvent.Raise();

        AudioManager.Instance.PlayRandomMusic(MusicEnum.Menu);              
        MenuManager.GoToMenu(MenuEnum.MainMenu);
        Destroy(gameObject);
    }
    #endregion
}
