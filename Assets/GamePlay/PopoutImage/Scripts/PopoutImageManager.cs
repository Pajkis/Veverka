using UnityEngine;

/// <summary>
/// Manages popout image display system - listens for events and instantiates animated images
/// Follows the pattern of BubbleMessageManager
/// </summary>
public class PopoutImageManager : MonoBehaviour
{
    #region Fields
    [Header("Events")]
    [SerializeField] private GoalEvents goalEvents;
    [SerializeField] private ObstacleEvents obstacleEvents;

    [Header("Prefab Variants")]
    [Tooltip("Prefab for basic goal popouts")]
    [SerializeField] private GameObject popoutGoalPrefab;
    [Tooltip("Prefab for golden goal popouts")]
    [SerializeField] private GameObject popoutGoldenGoalPrefab;

    private Transform gridRoot;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        // Find GridRoot - popout images will be children of GridRoot like bubble messages
        gridRoot = GameObject.Find("GridRoot")?.transform;
        if (gridRoot == null)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "GridRoot not found in the scene. Please make sure there is a GameObject named 'GridRoot'.", this);
        }

        // Validate prefabs
        if (popoutGoalPrefab == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay, "PopoutGoal prefab is not assigned!", this);
        }
        if (popoutGoldenGoalPrefab == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay, "PopoutGoldenGoal prefab is not assigned!", this);
        }
    }

    private void OnEnable()
    {
        if (goalEvents != null)
            goalEvents.AddListener(OnGoalEvent);
        if (obstacleEvents != null)
            obstacleEvents.AddListener(OnObstacleEvent);
    }

    private void OnDisable()
    {
        if (goalEvents != null)
            goalEvents.RemoveListener(OnGoalEvent);
        if (obstacleEvents != null)
            obstacleEvents.RemoveListener(OnObstacleEvent);
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles goal events - creates popout image if specified in sub-payload.
    /// Selects appropriate prefab variant based on PopoutImageType (NOT GoalType).
    /// </summary>
    private void OnGoalEvent(GoalEventPayload payload)
    {
        if (payload.EventType != GoalEventsType.GoalResolve) return;
        if (payload.PopoutImage == null || !payload.PopoutImage.ShouldShow) return;

        // Set position from parent payload (sub-payload doesn't have position set)
        payload.PopoutImage.Position = payload.Position;

        // Select correct prefab based on IMAGE TYPE (not goal type)
        GameObject prefabToUse = null;
        switch (payload.PopoutImage.ImageType)
        {
            case PopoutImageType.GoldenGoal:
                prefabToUse = popoutGoldenGoalPrefab;
                break;
            case PopoutImageType.BasicGoal:
                prefabToUse = popoutGoalPrefab;
                break;
            case PopoutImageType.NoImage:
                // No popout to display
                return;
            default:
                DebugLogger.LogWarning(DebugLogCategory.Gameplay,
                    $"No popout prefab configured for PopoutImageType: {payload.PopoutImage.ImageType}", this);
                return;
        }

        if (prefabToUse == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay,
                $"Popout prefab is null for PopoutImageType: {payload.PopoutImage.ImageType}", this);
            return;
        }

        CreatePopoutImage(prefabToUse, payload.PopoutImage);
    }

    /// <summary>
    /// Handles obstacle events - creates popout image if specified in sub-payload.
    /// Uses ImageType to select appropriate prefab variant.
    /// </summary>
    private void OnObstacleEvent(ObstacleEventPayload payload)
    {
        if (payload.EventType != ObstacleEventsType.ObstacleResolve) return;
        if (payload.PopoutImage == null || !payload.PopoutImage.ShouldShow) return;

        // Set position from parent payload (sub-payload doesn't have position set)
        payload.PopoutImage.Position = payload.Position;

        // Select prefab based on IMAGE TYPE
        GameObject prefabToUse = null;
        switch (payload.PopoutImage.ImageType)
        {
            case PopoutImageType.BasicGoal:
                prefabToUse = popoutGoalPrefab;
                break;
            case PopoutImageType.GoldenGoal:
                prefabToUse = popoutGoldenGoalPrefab;
                break;
            case PopoutImageType.NoImage:
                return;
            default:
                DebugLogger.LogWarning(DebugLogCategory.Gameplay,
                    $"No popout prefab configured for PopoutImageType: {payload.PopoutImage.ImageType}", this);
                return;
        }

        if (prefabToUse == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay,
                $"Popout prefab is null for PopoutImageType: {payload.PopoutImage.ImageType}", this);
            return;
        }

        CreatePopoutImage(prefabToUse, payload.PopoutImage);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Creates and initializes a popout image from the specified prefab and popout payload.
    /// Sprite is expected to be pre-assigned in the prefab (not set from payload).
    /// </summary>
    private void CreatePopoutImage(GameObject prefab, PopoutImageEventPayload popout)
    {
        if (!popout.IsValid())
        {
            DebugLogger.LogWarning(DebugLogCategory.Gameplay, "Invalid PopoutImageEventPayload, skipping popout creation", this);
            return;
        }

        if (prefab == null || gridRoot == null)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "Cannot create popout image - missing prefab or GridRoot", this);
            return;
        }

        // Instantiate prefab as child of GridRoot (same as BubbleMessage)
        GameObject popoutObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, gridRoot);

        // Convert grid position to world position and apply offset
        Vector3 worldPos = GridUtils.GridToWorld(popout.Position);
        worldPos += (Vector3)popout.PositionOffset;
        popoutObj.transform.localPosition = worldPos;

        // Get component and initialize (use GetComponentInChildren to support nested prefab structures)
        PopoutImageBox popoutBox = popoutObj.GetComponentInChildren<PopoutImageBox>();
        if (popoutBox == null)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "PopoutImage prefab missing PopoutImageBox component!", this);
            Destroy(popoutObj);
            return;
        }

        popoutBox.Init(popout);

        DebugLogger.Log(DebugLogCategory.Gameplay, $"Created popout image at {popout.Position} with offset {popout.PositionOffset}: {popout}", this);
    }
    #endregion
}
