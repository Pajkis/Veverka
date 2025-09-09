using UnityEngine;

/// <summary>
/// Level finished pop out menu handle class
/// </summary>
public class LevelFinishedMenu : MonoBehaviour
{
    #region fields
    GameObject buttonNextLevel;

    [SerializeField]
    private LevelDatabase levelDatabase;   

    [SerializeField]
    private SceneNavigationEvents sceneNavigationEvent;

    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {  
        // hide button on max level finished
        buttonNextLevel = GameObject.Find("ButtonNextLevel");
        
        if (levelDatabase.CurrentLevelIndex == 9)
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
    public void HandleNextLevelButtonOnClickEvent()
    {
        Time.timeScale = 1; 
        
        //Set next level
        levelDatabase.CurrentLevelIndex++;
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
        Time.timeScale = 1;                        
      
        sceneNavigationEvent.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.UnloadLevel
        });
      //  Destroy(gameObject);
    }

    #endregion
}
