using UnityEngine;

public class InGameMenu : MonoBehaviour
{        
    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        MenuManager.GoToMenu(MenuEnum.LoadLevel);
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
