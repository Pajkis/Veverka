using UnityEngine;

public class InGameButtonHandler : MonoBehaviour
{

    #region fields
    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {        

        //Raise level start event
        levelSelectEvent.Raise(new LevelSelectPayload
        {
            levelNumber = levelDatabase.CurrentLevelIndex,
            resetRequested = true,
        });

        Debug.Log($"raising restart event: level {levelDatabase.CurrentLevelIndex}");
    }

    /// <summary>
    /// handles opening of the pause menu
    /// </summary>
    public void HandlePauseButtonOnClickEvent()
    {
        MenuManager.GoToMenu(MenuEnum.PauseMenu);
    }

    /// <summary>
    /// Handle help button on click event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    { 
        
    }
}
