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
    private GoToSceneEvent goToSceneEvent;

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
        goToSceneEvent.Raise(SceneType.LoadLevel);
        // Destroy(gameObject);
    }

    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        Time.timeScale = 1;                        
      
        goToSceneEvent.Raise(SceneType.UnloadLevel);
      //  Destroy(gameObject);
    }

    #endregion
}
