using UnityEngine;

/// <summary>
/// Manages the main menu interactions
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;

    /// <summary>
    /// Handles the click on play button event
    /// </summary>
    public void HandlePlayButtonOnClickEvent()
    {
      sceneNavigationEvents.Raise(new SceneNavigationEventPayload
      {
          EventType = SceneNavigationEventType.GoToScene,
          Scene = SceneType.LevelMenu
      });
    }

    /// <summary>
    ///handles the click on help button event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {    
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.OpenOverlay,
            Overlay = OverlayType.GameHelp
        });
    }

    /// <summary>
    /// hndles the click on credits button event
    /// </summary>
    public void HandleCreditsButtonOnClickEvent()
    { 
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.OpenOverlay,
            Overlay = OverlayType.Credits
        });
    }

    /// <summary>
    /// Handles game settings menu on click event
    /// </summary>
    public void HandleGameSettingsButtonOnClickEvent()
    {
       sceneNavigationEvents.Raise(new SceneNavigationEventPayload
       {
           EventType = SceneNavigationEventType.OpenOverlay,
           Overlay = OverlayType.SettingsMenu
       });
    }

    /// <summary>
    /// Handles the on click event from the quit button
    /// </summary>
    public void HandleQuitGameButtonOnClickEvent()
    {
        Application.Quit();
    }
}
