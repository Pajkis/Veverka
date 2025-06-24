using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Pause Menu handling class
/// </summary>
public class PauseMenu : IntEventInvoker
{
    #region fields
    [SerializeField]
    private LevelStartEvent levelStartEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0; //  *not necessary in logic games, where time does not matter*
                
        if (!unityEvents.ContainsKey(EventEnum.ResetGridEvent))
        {
            unityEvents.Add(EventEnum.ResetGridEvent, new UnityEvent<int>());
        }
        EventManager.AddInvoker(EventEnum.ResetGridEvent, this);
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
        unityEvents[EventEnum.ResetGridEvent].Invoke(0);
       
        //Raise level start event
        levelStartEvent.Raise(new LevelStartPayload
        {
            levelNumber = levelDatabase.CurrentLevelIndex,
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
        unityEvents[EventEnum.ResetGridEvent].Invoke(0);
        AudioManager.Instance.PlayRandomMusic(MusicEnum.Menu);              
        MenuManager.GoToMenu(MenuEnum.MainMenu);
        Destroy(gameObject);
    }
    #endregion
}
