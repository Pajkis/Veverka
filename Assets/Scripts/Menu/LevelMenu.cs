using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Select level menu handling class
/// </summary>
public class LevelMenu : IntEventInvoker
{
    // Start is called before the first frame update
    void Start()
    {
        if(!unityEvents.ContainsKey(EventEnum.LevelStartEvent))
        {
           unityEvents.Add(EventEnum.LevelStartEvent, new LevelStartEvent());
           EventManager.AddInvoker(EventEnum.LevelStartEvent, this);
        }        
        
    }

    /// <summary>
    /// Handle on click level button on click event
    /// </summary>
    /// <param name="levelNumber"></param>
    public void HandleLevelButtonOnClickEvent(int levelNumber)
    {
        unityEvents[EventEnum.LevelStartEvent].Invoke(levelNumber);
    }

    /// <summary>
    /// handles on click back button event
    /// </summary>
    public void HandleBackButtonOnCLickEvent()
    {
        MenuManager.GoToMenu(MenuEnum.MainMenu);
    }


   
}
