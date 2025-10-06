using UnityEngine;

/// <summary>
/// Level finished pop out menu handle class
/// </summary>
public class LevelFinishedMenu : MonoBehaviour
{
    #region fields
    GameObject buttonNextLevel;

    [SerializeField]
    private LevelSelection levelSelection;   

    [SerializeField]
    private SceneNavigationEvents sceneNavigationEvent;

    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"LevelFinishedMenu opened - Level {levelSelection.CurrentLevelIndex} completed", this);

        // hide button on max level finished
        buttonNextLevel = GameObject.Find("ButtonNextLevel");

        if (levelSelection.CurrentLevelIndex == 9)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, "Last level completed - hiding Next Level button", this);
            buttonNextLevel.SetActive(false);
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, "More levels available - showing Next Level button", this);
            buttonNextLevel.SetActive(true);
        }

        DebugLogger.Log(DebugLogCategory.SceneManager, "Pausing game time for level finished menu", this);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Handle next level button on click event
    /// </summary>
    public void HandleNextLevelButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"LevelFinishedMenu: Next Level button clicked - advancing to level {levelSelection.CurrentLevelIndex + 1}", this);
        Time.timeScale = 1;

        //Set next level
        levelSelection.CurrentLevelIndex++;
        levelSelection.CurrentAction = LevelAction.Load;
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LoadLevel
        });
        // Destroy(gameObject);
    }

    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "LevelFinishedMenu: Quit button clicked - returning to level select", this);
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
