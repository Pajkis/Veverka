using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Select level menu handling class
/// </summary>
public class LevelMenu : MonoBehaviour
{

    #region fields  

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion

    /// <summary>
    /// Handle on click level button on click event
    /// </summary>
    /// <param name="levelNumber"></param>
    public void HandleLevelButtonOnClickEvent(int levelNumber)
    {
        AudioManager.Instance.PlayRandomMusic(MusicEnum.Game);

        // Set level and change screen
        levelDatabase.CurrentLevelIndex = levelNumber;
        MenuManager.GoToMenu(MenuEnum.LoadLevel);

    }

    /// <summary>
    /// handles on click back button event
    /// </summary>
    public void HandleBackButtonOnCLickEvent()
    {
        MenuManager.GoToMenu(MenuEnum.MainMenu);
    }


   
}
