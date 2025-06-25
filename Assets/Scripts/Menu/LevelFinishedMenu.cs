using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Level finished pop out menu handle class
/// </summary>
public class LevelFinishedMenu : MonoBehaviour
{
    #region fields
    GameObject gridRoot;
    GameObject buttonNextLevel;

    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

    [SerializeField]
    private ResetGridEvent resetGridEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;

    #endregion

    #region methods

    // Start is called before the first frame update
    void Start()
    {  
        // hide button on max level finished
        buttonNextLevel = GameObject.Find("ButtonNextLevel");
        
        if (levelDatabase.CurrentLevelIndex == 10)
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
        
        //level select event raise event
        levelDatabase.CurrentLevelIndex++;
        levelSelectEvent.Raise(new LevelSelectPayload 
        {
          levelNumber = levelDatabase.CurrentLevelIndex,
          resetRequested = true,
        });
        Destroy(gameObject);
    }

    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        Time.timeScale = 1;
        
        // raise event to reset grid
        resetGridEvent.Raise();
                
        AudioManager.Instance.PlayRandomMusic(MusicEnum.Menu);
        MenuManager.GoToMenu(MenuEnum.MainMenu);
        Destroy(gameObject);
    }

    #endregion
}
