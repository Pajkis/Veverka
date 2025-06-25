using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Select level menu handling class
/// </summary>
public class LevelMenu : MonoBehaviour
{

    #region fields
    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

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

        //Raise level start event
        levelDatabase.CurrentLevelIndex = levelNumber;
        levelSelectEvent.Raise(new LevelSelectPayload
        {            
            levelNumber = levelDatabase.CurrentLevelIndex,
        });

    }

    /// <summary>
    /// handles on click back button event
    /// </summary>
    public void HandleBackButtonOnCLickEvent()
    {
        MenuManager.GoToMenu(MenuEnum.MainMenu);
    }


   
}
