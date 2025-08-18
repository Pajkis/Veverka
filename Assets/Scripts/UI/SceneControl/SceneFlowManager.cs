using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages navigation across the menu universe system
/// </summary>
public static class SceneFlowManager 
{
    /// <summary>
    /// Go to menu navigation
    /// </summary>
    /// <param name="sceneName">name of the menu enum</param>
    public static void GoToScene(SceneType sceneName)
    {
        switch (sceneName)
        {
            case SceneType.MainMenu:
                // Go to main menu scene
                SceneManager.LoadScene("10_MainMenu");
                break;

            case SceneType.PlayerMenu:

                break;

            case SceneType.LevelMenu:

                // Go to level menu scene
                SceneManager.LoadScene("11_LevelMenu");
                break;

            // Handle overlay scenes
            case SceneType.PauseMenu:
            case SceneType.LevelFinishedMenu:
            case SceneType.SettingsMenu:
            case SceneType.GameHelp:
                InstantiateOverlay(sceneName);
                break;

            case SceneType.HighScoreMenu:

                break;                         

            case SceneType.LoadLevel:

                SceneManager.LoadScene("20_LevelLoad");
                break;

            case SceneType.UnloadLevel:
                SceneManager.LoadScene("21_LevelUnload");
                break;
                    
            case SceneType.GamePlay:
                SceneManager.LoadScene("30_GamePlay");
                break;            
        }
    }

    // <summary>
    /// Instantiates overlay prefabs based on scene type using <see cref="OverlayPrefabProvider"/>.
    /// </summary>
    /// <param name="overlayType">Type of overlay to instantiate.</param>
    private static void InstantiateOverlay(SceneType overlayType)
    {
        if (OverlayPrefabProvider.Instance == null)
        {
            Debug.LogWarning("OverlayPrefabProvider.Instance is not set.");
            return;
        }

        GameObject prefab = OverlayPrefabProvider.Instance.GetPrefab(overlayType);

        if (prefab == null)
        {
            Debug.LogWarning($"Overlay prefab for {overlayType} is not assigned in OverlayPrefabProvider.");
            return;
        }

        Object.Instantiate(prefab);
    }
}
