using UnityEngine;

/// <summary>
/// Pause Menu handling class
/// </summary>
public class PauseMenu : MonoBehaviour
{
    #region fields

     [SerializeField]
    private GoToSceneEvent goToSceneEvent;

    [SerializeField]
    private OpenOverlayEvent openOverlayEvent;
    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0; //  *not necessary in logic games, where time does not matter*         
    }

    /// <summary>
    /// handles on click resume button event
    /// </summary>
    public void HandleResumeButtonOnClickEvent()
    {
        Time.timeScale = 1;
        Destroy(gameObject);    
    }

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        Time.timeScale = 1;
       goToSceneEvent.Raise(SceneType.LoadLevel);
       // Destroy(gameObject);
    }

    /// <summary>
    /// Handles game settings menu on click event
    /// </summary>
    public void HandleGameSettingsButtonOnClickEvent()
    {
        openOverlayEvent.Raise(OverlayType.SettingsMenu);
    }

    /// <summary>
    /// Handles on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()   
    
    {       
        Time.timeScale = 1;       
        goToSceneEvent.Raise(SceneType.UnloadLevel);
       
      //  Destroy(gameObject);
    }
    #endregion
}
