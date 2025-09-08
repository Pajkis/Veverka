using UnityEngine;

/// <summary>
/// Manages the main menu interactions
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GoToSceneEvent goToSceneEvent;
    [SerializeField] private OpenOverlayEvent openOverlayEvent;

    /// <summary>
    /// Handles the click on play button event
    /// </summary>
    public void HandlePlayButtonOnClickEvent()
    {
      goToSceneEvent.Raise(SceneType.LevelMenu);
    }

    /// <summary>
    ///handles the click on help button event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {    
        openOverlayEvent.Raise(OverlayType.GameHelp);
    }

    /// <summary>
    /// hndles the click on credits button event
    /// </summary>
    public void HandleCreditsButtonOnClickEvent()
    { 
        openOverlayEvent.Raise(OverlayType.Credits);
    }

    /// <summary>
    /// Handles game settings menu on click event
    /// </summary>
    public void HandleGameSettingsButtonOnClickEvent()
    {
       openOverlayEvent.Raise(OverlayType.SettingsMenu);
    }

    /// <summary>
    /// Handles the on click event from the quit button
    /// </summary>
    public void HandleQuitGameButtonOnClickEvent()
    {
        Application.Quit();
    }
}
