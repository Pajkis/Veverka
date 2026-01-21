using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Manages navigation across the menu universe system
/// </summary>
public class SceneFlowManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;

    void Awake()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "SceneFlowManager initialized - persisting across scene loads", this);
        DontDestroyOnLoad(gameObject); 
    }

    private void OnEnable()
    {
        sceneNavigationEvents?.AddListener(OnSceneNavigationEvent);
    }

    private void OnDisable()
    {
        sceneNavigationEvents?.RemoveListener(OnSceneNavigationEvent);
    }

    private void OnSceneNavigationEvent(SceneNavigationEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"SceneNavigation event received - Type: {payload.EventType}", this);

        switch (payload.EventType)
        {
            case SceneNavigationEventType.GoToScene:
                DebugLogger.Log(DebugLogCategory.SceneManager, $"Processing GoToScene request for: {payload.Scene}", this);
                GoToScene(payload.Scene);
                break;
            case SceneNavigationEventType.OpenOverlay:
                DebugLogger.Log(DebugLogCategory.SceneManager, $"Processing OpenOverlay request for: {payload.Overlay}", this);
                OpenOverlay(payload.Overlay);
                break;
        }
    }

    /// <summary>
    /// Loads the specified scene with smooth transition effects.
    /// Uses TransitionManager if available, otherwise falls back to direct loading.
    /// </summary>
    /// <param name="scene">Scene to load.</param>
    private void GoToScene(SceneType scene)
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"GoToScene initiated for: {scene}", this);

        if (SceneRefProvider.Instance == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "SceneRefProvider.Instance is not set - cannot load scene", this);
            return;
        }

        AssetReference sceneRef = SceneRefProvider.Instance.GetScene(scene);

        if (sceneRef == null || !sceneRef.RuntimeKeyIsValid())
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, $"Scene reference for {scene} is not assigned or invalid in SceneRefProvider", this);
            return;
        }

        // Use TransitionManager for smooth scene transitions if available
        if (TransitionManager.Instance != null)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Loading scene {scene} with transition", this);
            TransitionManager.Instance.TransitionToScene(scene, sceneRef);
        }
        else
        {
            // Fallback to direct loading if TransitionManager not available
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "TransitionManager not found - loading scene directly without transition", this);
            sceneRef.LoadSceneAsync();
        }
    }

    /// <summary>
    /// Opens the specified overlay with animation.
    /// </summary>
    /// <param name="overlay">Overlay to open.</param>
    private void OpenOverlay(OverlayType overlay)
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, $"OpenOverlay initiated for: {overlay}", this);

        if (OverlayPrefabProvider.Instance == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "OverlayPrefabProvider.Instance is not set - cannot open overlay", this);
            return;
        }

        GameObject prefab = OverlayPrefabProvider.Instance.GetPrefab(overlay);

        if (prefab == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, $"Overlay prefab for {overlay} is not assigned in OverlayPrefabProvider", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.SceneManager, $"Instantiating overlay {overlay} from prefab: {prefab.name}", this);
        GameObject instance = Object.Instantiate(prefab);

        // Get OverlayAnimationController (should already exist in prefab)
        OverlayAnimationController animController = instance.GetComponent<OverlayAnimationController>();
        if (animController != null)
        {
            // Set overlay type so controller knows which settings to use
            animController.SetOverlayType(overlay);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager,
                $"OverlayAnimationController not found on {overlay} prefab - animations won't work", this);
        }
    }
}
