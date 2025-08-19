using UnityEngine;

/// <summary>
/// Select level menu handling class
/// </summary>
public class LevelMenu : MonoBehaviour
{

    #region fields

    [SerializeField]
    private LevelDatabase levelDatabase;

    [SerializeField] private PlayMusicEvent playMusicEvent;

    [SerializeField]
    private GoToSceneEvent goToSceneEvent;
    #endregion

    /// <summary>
    /// Handle on click level button on click event
    /// </summary>
    /// <param name="levelNumber"></param>
    public void HandleLevelButtonOnClickEvent(int levelNumber)
    {
        playMusicEvent.Raise(MusicType.Game);

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
