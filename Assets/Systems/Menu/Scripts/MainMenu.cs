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
      DebugLogger.Log(DebugLogCategory.SceneManager, "MainMenu: Play button clicked - navigating to LevelMenu", this);
      sceneNavigationEvents.Raise(new SceneNavigationEventPayload
      {
          EventType = SceneNavigationEventType.GoToScene,
          Scene = SceneType.LevelSelect
      });
    }

    /// <summary>
    ///handles the click on help button event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "MainMenu: Help button clicked - opening GameHelp overlay", this);
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
        DebugLogger.Log(DebugLogCategory.SceneManager, "MainMenu: Credits button clicked - opening Credits overlay", this);
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
       DebugLogger.Log(DebugLogCategory.SceneManager, "MainMenu: Settings button clicked - opening SettingsMenu overlay", this);
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
        DebugLogger.Log(DebugLogCategory.SceneManager, "MainMenu: Quit button clicked - exiting application", this);
        Application.Quit();
    }
}
