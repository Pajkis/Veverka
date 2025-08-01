using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{

    #region fields
    

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        SceneManager.LoadScene("20_LevelLoad");      
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
