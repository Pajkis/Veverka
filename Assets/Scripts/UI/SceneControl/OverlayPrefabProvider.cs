using UnityEngine;

/// <summary>
/// Provides access to UI overlay prefabs used by the SceneFlowManager.
/// The prefabs can be assigned in the inspector and do not need to reside in the Resources folder.
/// </summary>
public class OverlayPrefabProvider : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the provider.
    /// </summary>
    public static OverlayPrefabProvider Instance { get; private set; }

    [Header("Overlay Prefabs")]
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject levelFinishedMenuPrefab;
    [SerializeField] private GameObject settingsMenuPrefab;
    [SerializeField] private GameObject gameHelpPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Retrieves overlay prefab for given scene type.
    /// </summary>
    /// <param name="scene">Overlay scene type</param>
    /// <returns>Prefab assigned for the scene type, or null if not set.</returns>
    public GameObject GetPrefab(SceneType scene)
    {
        return scene switch
        {
            SceneType.PauseMenu => pauseMenuPrefab,
            SceneType.LevelFinishedMenu => levelFinishedMenuPrefab,
            SceneType.SettingsMenu => settingsMenuPrefab,
            SceneType.GameHelp => gameHelpPrefab,
            _ => null
        };
    }
}
