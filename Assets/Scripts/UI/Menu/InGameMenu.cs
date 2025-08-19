using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GoToSceneEvent goToSceneEvent;
    [SerializeField] private OpenOverlayEvent openOverlayEvent;

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        goToSceneEvent.Raise(SceneType.LoadLevel);
    }

    /// <summary>
    /// handles opening of the pause menu
    /// </summary>
    public void HandlePauseButtonOnClickEvent()
    {
        openOverlayEvent.Raise(OverlayType.PauseMenu);
    }

    /// <summary>
    /// Handle help button on click event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {
        openOverlayEvent.Raise(OverlayType.GameHelp);
    }
}
