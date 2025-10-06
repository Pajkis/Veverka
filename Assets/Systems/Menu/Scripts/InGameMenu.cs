using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;
    [SerializeField] private LevelSelection levelSelection;

    /// <summary>
    /// Handles on click level restart button event
    /// </summary>
    public void HandleRestartButtonOnClickEvent()
    {
        levelSelection.CurrentAction = LevelAction.Load;
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LoadLevel
        });
    }

    /// <summary>
    /// handles opening of the pause menu
    /// </summary>
    public void HandlePauseButtonOnClickEvent()
    {
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.OpenOverlay,
            Overlay = OverlayType.PauseMenu
        });
    }

    /// <summary>
    /// Handle help button on click event
    /// </summary>
    public void HandleHelpButtonOnClickEvent()
    {
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.OpenOverlay,
            Overlay = OverlayType.GameHelp
        });
    }
}
