using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Level finished pop out menu handle class
/// </summary>
public class LevelFinishedMenu : IntEventInvoker
{
    #region fields
    GameObject gridRoot;
    GameObject buttonNextLevel;

    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {  
        // Add invokers for events
        if (!unityEvents.ContainsKey(EventEnum.ResetGridEvent))
        {
            unityEvents.Add(EventEnum.ResetGridEvent, new UnityEvent<int>());
        }
        EventManager.AddInvoker(EventEnum.ResetGridEvent, this);

        // hide button on max level finished
        buttonNextLevel = GameObject.Find("ButtonNextLevel");
        
        if (levelDatabase.CurrentLevelIndex == 10)
        {
            buttonNextLevel.SetActive(false);
        }
        else 
        {
            buttonNextLevel.SetActive(true);
        }

        Time.timeScale = 0;
    }

    /// <summary>
    /// Handle next level button on click event
    /// </summary>  
    public void HandleNextLevelButtonOnClickEvent()
    {
        Time.timeScale = 1;        
        unityEvents[EventEnum.ResetGridEvent].Invoke(0);

        //Raise event
        levelDatabase.CurrentLevelIndex++;
        levelSelectEvent.Raise(new LevelSelectPayload 
        {
          levelNumber = levelDatabase.CurrentLevelIndex,
        });
        Destroy(gameObject);
    }

    /// <summary>
    /// Handle on click quit button event
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
