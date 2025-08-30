using UnityEngine;

/// <summary>
/// Select level menu handling class
/// </summary>
public class LevelMenu : MonoBehaviour
{

    #region fields
    [Header("Level database")]    
    [SerializeField] private LevelDatabase levelDatabase;

    [Header("Events")]
    [SerializeField]  private GoToSceneEvent goToSceneEvent;

    [Header("Current selection set")]
    [SerializeField] private LevelSetType currentSet = LevelSetType.Basic;
    #endregion


    /// <summary>
    /// Handle on click set button 
    /// </summary>
    public void HandleSetButtonOnClickEvent(int setIndex)
    {
        currentSet = (LevelSetType)setIndex;
        levelDatabase.LevelSetType = currentSet;
        Debug.Log("Current set: " + currentSet.ToString());
    }

    /// <summary>
    /// Handle on click level button on click event
    /// </summary>
    /// <param name="levelNumber"></param>
    public void HandleLevelButtonOnClickEvent(int levelNumber)
    {
        // Set level and change screen
        levelDatabase.CurrentLevelIndex = levelNumber;
        goToSceneEvent.Raise(SceneType.LoadLevel);
    }

    /// <summary>
    /// handles on click back button event
    /// </summary>
    public void HandleBackButtonOnCLickEvent()
    {
        goToSceneEvent.Raise(SceneType.MainMenu);
    }


   
}
