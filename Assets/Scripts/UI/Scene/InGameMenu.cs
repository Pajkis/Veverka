using UnityEngine;

public class InGameMenu : MonoBehaviour
{        
    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        SceneManager.GoToMenu(SceneType.LoadLevel);
    }

    /// <summary>
    /// handles opening of the pause menu
    /// </summary>
    public void HandlePauseButtonOnClickEvent()
    {
        SceneManager.GoToMenu(SceneType.PauseMenu);
    }

    /// <summary>
    /// Handle help button on click event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    { 
        
    }
}
