using UnityEngine;

/// <summary>
/// Repositions the camera when a tutorial opens and restores it when the tutorial closes.
/// Attach to any camera that needs to display the tutorial grid (e.g. MainMenu camera).
/// Reads the target position from DisplayConfig so it stays in sync with the rest of the display settings.
/// </summary>
public class TutorialCameraPositioner : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DisplayConfig displayConfig;

    [Header("Events")]
    [SerializeField] private TutorialEvents tutorialEvents;

    private Vector3 savedPosition;

    private void OnEnable()
    {
        tutorialEvents?.AddListener(OnTutorialEvent);
    }

    private void OnDisable()
    {
        tutorialEvents?.RemoveListener(OnTutorialEvent);
    }

    private void OnTutorialEvent(TutorialEventPayload payload)
    {
        switch (payload.EventType)
        {
            case TutorialEventType.LoadTutorial:
                EnterTutorialMode();
                break;
            case TutorialEventType.ClearTutorial:
                ExitTutorialMode();
                break;
        }
    }

    /// <summary>
    /// Saves the current camera position and moves to the tutorial camera position from DisplayConfig.
    /// </summary>
    private void EnterTutorialMode()
    {
        if (displayConfig == null) return;

        var profile = displayConfig.ResolveProfile();
        if (profile == null) return;

        savedPosition = transform.position;
        transform.position = profile.tutorialCameraPosition;
    }

    /// <summary>
    /// Restores the camera to the position it was at before the tutorial opened.
    /// </summary>
    private void ExitTutorialMode()
    {
        transform.position = savedPosition;
    }
}
