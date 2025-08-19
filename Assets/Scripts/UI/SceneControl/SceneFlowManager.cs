using UnityEngine;
using UnityEngine.AddressableAssets;


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
        if (SceneRefProvider.Instance == null)
        {
            Debug.LogWarning("SceneNameProvider.Instance is not set.");
            return;
        }

        AssetReference sceneRef = SceneRefProvider.Instance.GetScene(scene);

        if (sceneRef == null || !sceneRef.RuntimeKeyIsValid())
        {
            Debug.LogWarning($"Scene reference for {scene} is not assigned in SceneNameProvider.");
            return;
        }

        sceneRef.LoadSceneAsync();
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
