using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veverka.GridSystem.GameGrid;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Handles the click on play button event
    /// </summary>
    public void HandlePlayButtonOnClickEvent()
    {
        // play cool sound
        // go to level menu
        SceneFlowManager.GoToScene(SceneType.LevelMenu);
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
    /// Handles game settings menu on click event
    /// </summary>
    public void HandleGameSettingsButtonOnClickEvent()
    {
       SceneFlowManager.OpenOverlay(OverlayType.SettingsMenu);
    }

    /// <summary>
    /// Handles the on click event from the quit button
    /// </summary>
    public void HandleQuitGameButtonOnClickEvent() 
    { 
        Application.Quit(); 
    }


}
