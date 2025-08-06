using UnityEngine;

public class InGameMenu : MonoBehaviour
{        
    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        SceneManager.GoToScene(SceneType.LoadLevel);
    }

    /// <summary>
    /// handles opening of the pause menu
    /// </summary>
    public void HandlePauseButtonOnClickEvent()
    {
        SceneManager.GoToScene(SceneType.PauseMenu);
    }

    /// <summary>
    /// Handle help button on click event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {
        SceneManager.GoToScene(SceneType.GameHelp);
    }
}
