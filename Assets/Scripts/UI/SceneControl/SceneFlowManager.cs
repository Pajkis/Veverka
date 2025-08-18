using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages navigation across the menu universe system
/// </summary>
public static class SceneFlowManager
{
    /// <summary>
    /// Loads the specified scene.
    /// </summary>
    /// <param name="scene">Scene to load.</param>
    public static void GoToScene(SceneType scene)
    {
        if (SceneNameProvider.Instance == null)
        {
            Debug.LogWarning("SceneNameProvider.Instance is not set.");
            return;
        }

        string sceneName = SceneNameProvider.Instance.GetSceneName(scene);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"Scene name for {scene} is not assigned in SceneNameProvider.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Opens the specified overlay.
    /// </summary>
    /// <param name="overlay">Overlay to open.</param>
    public static void OpenOverlay(OverlayType overlay)
    {
        if (OverlayPrefabProvider.Instance == null)
        {
            Debug.LogWarning("OverlayPrefabProvider.Instance is not set.");
            return;
        }

        GameObject prefab = OverlayPrefabProvider.Instance.GetPrefab(overlay);

        if (prefab == null)
        {
            Debug.LogWarning($"Overlay prefab for {overlay} is not assigned in OverlayPrefabProvider.");
            return;
        }

        Object.Instantiate(prefab);
    }
}
