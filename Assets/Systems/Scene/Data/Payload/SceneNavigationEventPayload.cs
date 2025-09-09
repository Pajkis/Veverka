using UnityEngine;

/// <summary>
/// Payload for <see cref="SceneNavigationEvent"/> combining scene and overlay navigation.
/// </summary>
[System.Serializable]
public struct SceneNavigationEventPayload
{
    /// <summary>
    /// Type of navigation event to process.
    /// </summary>
    public SceneNavigationEventType EventType;

    /// <summary>
    /// Scene to load when <see cref="SceneNavigationEventType.GoToScene"/> is raised.
    /// </summary>
    public SceneType Scene;

    /// <summary>
    /// Overlay to open when <see cref="SceneNavigationEventType.OpenOverlay"/> is raised.
    /// </summary>
    public OverlayType Overlay;
}
