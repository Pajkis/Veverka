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
    GameObject gridRoot;
    GameObject buttonNextLevel;

    // Start is called before the first frame update
    void Start()
    {
        // add invokers
        if (!unityEvents.ContainsKey(EventEnum.LevelStartEvent))
        {
            unityEvents.Add(EventEnum.LevelStartEvent, new LevelStartEvent());            
        }
        EventManager.AddInvoker(EventEnum.LevelStartEvent, this);

        // Add invokers for events
        if (!unityEvents.ContainsKey(EventEnum.ResetGridEvent))
        {
            unityEvents.Add(EventEnum.ResetGridEvent, new UnityEvent<int>());
        }
        EventManager.AddInvoker(EventEnum.ResetGridEvent, this);

        // hide button on max level finished
        buttonNextLevel = GameObject.Find("ButtonNextLevel");
        
        if (LevelUtils.SelectedLevel == 10)
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
    /// <param name="nextLevel"></param>
    public void HandleNextLevelButtonOnClickEvent(int nextLevel)
    {
        Time.timeScale = 1;        
        unityEvents[EventEnum.ResetGridEvent].Invoke(0);
        nextLevel = LevelUtils.SelectedLevel + 1;
        unityEvents[EventEnum.LevelStartEvent].Invoke(nextLevel);
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
}
