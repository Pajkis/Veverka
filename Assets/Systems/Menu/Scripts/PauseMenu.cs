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

    // Start is called before the first frame update
    void Start()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu opened - pausing game time", this);
        Time.timeScale = 0; //  *not necessary in logic games, where time does not matter*
    }

    /// <summary>
    /// handles on click resume button event
    /// </summary>
    public void HandleResumeButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu: Resume button clicked - resuming game and destroying pause menu", this);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "PauseMenu: Restart button clicked - reloading current level", this);
        Time.timeScale = 1;
        levelSelection.CurrentAction = LevelAction.Load;
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LoadLevel
        });
       // Destroy(gameObject);
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
        Time.timeScale = 1;
        levelSelection.CurrentAction = LevelAction.Unload;
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LoadLevel
        });

      //  Destroy(gameObject);
    }
    #endregion
}
