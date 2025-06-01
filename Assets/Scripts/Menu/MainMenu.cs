using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Handles the click on play button event
    /// </summary>
    public void HandlePlayButtonOnClickEvent()
    {
        // play cool sound
        // go to level menu
        MenuManager.GoToMenu(MenuEnum.LevelMenu);
    }

    /// <summary>
    /// WIP - Handles the click on player button event
    /// </summary>
    public void HandlePlayerButtonOnClickEvent()
    {
    
       
    }

    /// <summary>
    /// WIP - Handles the click on high score button event
    /// </summary>
    public void HandleHighScoreButtonOnClickEvent()
    { 
        
    }

    /// <summary>
    /// Handles the on click event from the quit button
    /// </summary>
    public void HandleQuitButtonOnClickEvent() 
    { 
        Application.Quit(); 
    }


}
