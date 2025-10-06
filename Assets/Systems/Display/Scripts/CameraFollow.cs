using UnityEngine;

/// <summary>
/// Camera follows target and clamps within GameGrid bounds.
/// Systems/Display/Scripts/CameraFollow
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Config (SO)")]
    [SerializeField] private DisplayConfig displayConfig;
    [SerializeField] private LevelInitData levelInitData;

    [Header("Events")]
    [SerializeField] private CharacterEvents characterEvents;

    #region fields
    [Header("Follow Settings")]
    [SerializeField] private Transform target;

    [Header("Camera bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    // Config values loaded from DisplayConfig
    private float smoothMoveTime;
    private Vector3 FollowOffset;
    private Vector3 ScreenToPlayScreenOffset;
    private float tileSize;
    private Vector2Int maxStaticTileSize;
    private float evenTilesOffset;

    private bool initialized = false;
    private bool hasReceivedCharacterData = false;

    private CameraMode mode;

    #endregion

    #region methods
    /// <summary>
    /// awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        ApplyConfig();

        if (levelInitData == null || levelInitData.gridOrigin == null ||
            levelInitData.gridCenterStartTarget == null || levelInitData.gridSize == null)
        {
            Debug.LogError("LevelInitData or its parameters is not set in CameraFollow. Please assign it or set values.");
            return;
        }
        Init(levelInitData.gridCenterStartTarget, levelInitData.gridOrigin, levelInitData.gridSize);
    }

    /// <summary>
    /// event subscriptions
    /// </summary>
    private void OnEnable()
    {
        if (characterEvents != null)
        {
            characterEvents.AddListener(OnCharacterEvent);
        }
    }

    /// <summary>
    /// event unsubscriptions
    /// </summary>
    private void OnDisable()
    {
        if (characterEvents != null)
        {
            characterEvents.RemoveListener(OnCharacterEvent);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// called when the script is loaded or a value is changed in the inspector
    /// </summary>
    private void OnValidate()
    {
        // changes in editor are applied immediately
        if (!Application.isPlaying)
            ApplyConfig();
    }
#endif
    /// <summary>
    /// apply configuration from DisplayConfig
    /// </summary>
    private void ApplyConfig()
    {
        if (displayConfig == null)
        {
            Debug.LogError("DisplayConfig is not assigned in CameraFollow!");
            return;
        }

        var profile = displayConfig.ResolveProfile();
        if (profile == null)
        {
            Debug.LogError("No suitable display profile found!");
            return;
        }

        // Load camera settings from unified DisplayConfig
        smoothMoveTime = profile.cameraFollowTime;
        FollowOffset = profile.cameraFollowOffset;
        ScreenToPlayScreenOffset = profile.screenToPlayOffset;
        tileSize = profile.tileSize;
        maxStaticTileSize = profile.maxStaticScreenSize;
        evenTilesOffset = profile.evenTilesOffset;
    }

    /// <summary>
    /// Initialize Camera and centers it on set target
    /// </summary>
    /// <param name="target">set target of camera to follow</param>
    /// <param name="gridSize">size of the grid</param>
    private void Init(Transform target, Transform origin, Vector2Int gridSize)
    {
        this.target = target;

        Vector3 gridOrigin = origin.position;

        // Calculate effective grid size considering surrounding walls
        Vector2Int effectiveGridSize = CalculateEffectiveGridSize(gridSize);
        Vector2 effectiveLevelSize = new Vector2(effectiveGridSize.x * tileSize, effectiveGridSize.y * tileSize);

        // Calculate grid offset due to surrounding walls centering
        Vector3 gridOffset = CalculateGridOffset(gridSize, effectiveGridSize);
        Vector3 effectiveGridOrigin = gridOrigin + gridOffset;

        Vector2 visibleSize = new Vector2(maxStaticTileSize.x * tileSize, maxStaticTileSize.y * tileSize);
        Vector2 halfVisibleSize = visibleSize / 2f;

        // Use effective grid origin and size for bounds calculation
        minBounds = effectiveGridOrigin + (Vector3)halfVisibleSize + ScreenToPlayScreenOffset;
        maxBounds = effectiveGridOrigin + (Vector3)effectiveLevelSize - (Vector3)halfVisibleSize + ScreenToPlayScreenOffset;

        // Camera mode setup
        if (gridSize.x <= maxStaticTileSize.x && gridSize.y <= maxStaticTileSize.y)
        {
            mode = CameraMode.StaticCenter;
            Vector3 center = effectiveGridOrigin + (Vector3)(effectiveLevelSize / 2f);

            // adjust to even tile numbered grid
            if ((effectiveGridSize.x % 2) == 0)
            {
                center += new Vector3(evenTilesOffset, 0, 0);
            }

            if ((effectiveGridSize.y % 2) == 0)
            {
                center += new Vector3(0, evenTilesOffset, 0);
            }

            transform.position = center + ScreenToPlayScreenOffset;
        }
        else
        {
            mode = CameraMode.FollowWithClamp;
            // Don't set initialized = true yet, wait for character event
        }
    }

    /// <summary>
    /// Calculate effective grid size including surrounding walls
    /// </summary>
    /// <param name="originalGridSize">Original grid size without walls</param>
    /// <returns>Effective grid size including walls</returns>
    private Vector2Int CalculateEffectiveGridSize(Vector2Int originalGridSize)
    {
        Vector2Int effectiveSize = originalGridSize;

        // If width is smaller than max screen, it will be filled to max screen width
        if (originalGridSize.x < maxStaticTileSize.x)
        {
            effectiveSize.x = maxStaticTileSize.x;
        }

        // If height is smaller than max screen, it will be filled to max screen height
        if (originalGridSize.y < maxStaticTileSize.y)
        {
            effectiveSize.y = maxStaticTileSize.y;
        }

        return effectiveSize;
    }

    /// <summary>
    /// Calculate grid offset due to surrounding walls centering
    /// </summary>
    /// <param name="originalGridSize">Original grid size</param>
    /// <param name="effectiveGridSize">Effective grid size with walls</param>
    /// <returns>Offset to apply to grid origin</returns>
    private Vector3 CalculateGridOffset(Vector2Int originalGridSize, Vector2Int effectiveGridSize)
    {
        Vector3 offset = Vector3.zero;

        // Calculate offset for X axis (horizontal centering)
        int diffX = effectiveGridSize.x - originalGridSize.x;
        if (diffX > 0)
        {
            int offsetLeft = diffX / 2;
            offset.x = -offsetLeft * tileSize;
        }

        // Calculate offset for Y axis (vertical centering)
        int diffY = effectiveGridSize.y - originalGridSize.y;
        if (diffY > 0)
        {
            int offsetDown = diffY / 2;
            offset.y = -offsetDown * tileSize;
        }

        return offset;
    }

    /// <summary>
    /// Handles character events to position camera on character spawn
    /// </summary>
    /// <param name="payload"></param>
    private void OnCharacterEvent(CharacterEventPayload payload)
    {
        if (payload.EventType == CharacterEventType.DataResponse && !hasReceivedCharacterData)
        {
            hasReceivedCharacterData = true;
            if (mode == CameraMode.FollowWithClamp && payload.Character != null)
            {
                Vector3 desired = payload.Character.transform.position + FollowOffset + ScreenToPlayScreenOffset;
                Vector3 clamped = ClampToBounds(desired);
                transform.position = clamped;
                initialized = true; // Now start following normally
            }
        }
    }

    /// <summary>
    /// Late update is called every frame if Behaviour is enabled
    /// </summary>
    private void LateUpdate()
    {
        if (mode == CameraMode.StaticCenter) return;
        if (!initialized || target == null) return;
            
        Vector3 desired = target.position + FollowOffset;
        Vector3 clamped = ClampToBounds(desired);
        transform.position = Vector3.Lerp(transform.position, clamped, smoothMoveTime);
    }

    /// <summary>
    /// Clamp bounds to the size of grid
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private Vector3 ClampToBounds(Vector3 targetPos)
    {
          
        float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
           
        return new Vector3(clampedX, clampedY, targetPos.z);         
    }

    #endregion
}
