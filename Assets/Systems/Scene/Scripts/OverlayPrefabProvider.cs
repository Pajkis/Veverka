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
    [SerializeField] private GameObject creditsPrefab;
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private GameObject levelValidationPrefab;

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
    /// <param name="overlay">Overlay type</param>
    /// <returns>Prefab assigned for the overlay type, or null if not set.</returns>
    public GameObject GetPrefab(OverlayType overlay)
    {
        return overlay switch
        {
            OverlayType.PauseMenu => pauseMenuPrefab,
            OverlayType.LevelFinishedMenu => levelFinishedMenuPrefab,
            OverlayType.SettingsMenu => settingsMenuPrefab,
            OverlayType.GameHelp => gameHelpPrefab,
            OverlayType.Credits => creditsPrefab,
            OverlayType.Notification => notificationPrefab,
            OverlayType.LevelValidationError => levelValidationPrefab,
            _ => null,
        };            
    }
}
