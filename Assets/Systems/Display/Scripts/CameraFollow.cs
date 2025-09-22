using UnityEngine;

/// <summary>
/// Camera follows target and clamps within GameGrid bounds.
/// Systems/Display/Scripts/CameraFollow
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Config (SO)")]
    [SerializeField] private DisplayConfig displayConfig;
    [SerializeField] private LevelDatabase levelDatabase;

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

        if (levelDatabase == null || levelDatabase.gridOrigin == null ||
            levelDatabase.gridCenterStartTarget == null || levelDatabase.gridSize == null)
        {
            Debug.LogError("LevelDatabase or its paramateres is not set in CameraFollow. Please assign it or set values.");
            return;
        }
        Init(levelDatabase.gridCenterStartTarget, levelDatabase.gridOrigin, levelDatabase.gridSize);
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

        Vector2 levelSize = new Vector2(gridSize.x * tileSize, gridSize.y * tileSize);
        Vector2 visibleSize = new Vector2(maxStaticTileSize.x * tileSize, maxStaticTileSize.y * tileSize);
        Vector2 halfVisibleSize = visibleSize / 2f;

        minBounds = gridOrigin + (Vector3)halfVisibleSize + ScreenToPlayScreenOffset;
        maxBounds = gridOrigin + (Vector3)levelSize - (Vector3)halfVisibleSize + ScreenToPlayScreenOffset;

        // Camera mode setup
        if (gridSize.x <= maxStaticTileSize.x && gridSize.y <= maxStaticTileSize.y)
        {
            mode = CameraMode.StaticCenter;
            Vector3 center = gridOrigin + (Vector3)(levelSize / 2f);
              
            // adjust to even tile numbered grid
            if ((gridSize.x % 2) == 0)
            {
                center += new Vector3(evenTilesOffset, 0, 0);
            }

            if ((gridSize.y % 2) == 0)
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
