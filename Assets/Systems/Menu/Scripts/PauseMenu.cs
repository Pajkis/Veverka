using UnityEngine;

/// <summary>
/// Pause Menu handling class
/// </summary>
public class PauseMenu : MonoBehaviour
{
    #region fields

    [SerializeField]
    private SceneNavigationEvents sceneNavigationEvent;
    [SerializeField]
    private LevelSelection levelSelection;
    #endregion

    #region methods

    void Start()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu opened", this);
    }

    /// <summary>
    /// handles on click resume button event
    /// </summary>
    public void HandleResumeButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu: Resume button clicked", this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu: Restart button clicked - reloading current level", this);
        levelSelection.CurrentAction = LevelAction.Load;
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LevelTransition
        });
    }

    /// <summary>
    /// Handles game settings menu on click event
    /// </summary>
    public void HandleGameSettingsButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu: Settings button clicked - opening SettingsMenu overlay", this);
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.OpenOverlay,
            Overlay = OverlayType.SettingsMenu
        });
    }

    /// <summary>
    /// Handles on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu: Quit button clicked - unloading level and returning to level select", this);
        levelSelection.CurrentAction = LevelAction.Unload;
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LevelTransition
        });
    }
    #endregion
}
