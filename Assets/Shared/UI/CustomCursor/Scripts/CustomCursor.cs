using UnityEngine;


public class CustomCursor : MonoBehaviour
{
    // Fallback if DisplaySettings is not available
    private Vector2 HotspotFallback = new Vector2(40f, -40f);
    private Vector3 CursorImageSizeFallback = new Vector3(80f, 80f, 0f);


    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private Vector2 hotspot;
    [SerializeField] private Vector3 cursorSize = new Vector3(80f, 80f, 0f);

    private Canvas canvas;
    private RectTransform canvasRect;
    private bool isActive;
    private Vector3 lastCursorSize;
     
    /// <summary>
    /// Initializes the cursor controller and configures the cursor image based on the current platform.
    /// </summary>
    /// <remarks>This method is called automatically by Unity when the script instance is being loaded. It
    /// enables or disables the cursor image depending on whether the application is running on a supported platform
    /// (such as Windows). If the cursor image is present and the platform is supported, the associated canvas is set to
    /// persist across scene loads.</remarks>
    void Awake()
    {
        DevicePlatformType currentPlatform = DisplayConfig.FromApplicationPlatform(Application.platform);
        isActive = currentPlatform == DevicePlatformType.Windows;
        

        if (!isActive)
        {
            if (cursorImage != null)
                cursorImage.gameObject.SetActive(false);
            return;
        }

        if (cursorImage != null)   
        {         
            canvas = cursorImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasRect = canvas.GetComponent<RectTransform>();
                DontDestroyOnLoad(canvas.gameObject);
            }
        }
    }

    /// <summary>
    /// Initializes the cursor by applying the configured hotspot and size settings, and hides the system cursor.
    /// </summary>
    /// <remarks>This method should be called to set up the cursor appearance based on the current display
    /// profile. If no active display profile is available, default fallback values are used. The system cursor is
    /// hidden after initialization.</remarks>
    void Start()
    {
        if (!isActive)
            return;

        // Load hotspot and size from DisplaySettings config
        if (DisplaySettings.Instance != null && DisplaySettings.Instance.ActiveProfile != null)
        {
            hotspot = DisplaySettings.Instance.ActiveProfile.cursorHotspot;
            cursorSize = DisplaySettings.Instance.ActiveProfile.cursorImageSize;
        }
        else
        {
            // Fallback defaults
            hotspot = HotspotFallback;
            cursorSize = CursorImageSizeFallback;
        }

        // Apply cursor size to RectTransform
        ApplyCursorSize();
        lastCursorSize = cursorSize;

        Cursor.visible = false;
    }

    void Update()
    {
        if (!isActive || cursorImage == null)
            return;

        // Check if cursor size changed during runtime
        if (cursorSize != lastCursorSize)
        {
            ApplyCursorSize();
            lastCursorSize = cursorSize;
        }

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out pos
        );

        cursorImage.localPosition = pos + hotspot;
    }

    private void ApplyCursorSize()
    {
        if (cursorImage != null)
        {
            cursorImage.sizeDelta = new Vector2(cursorSize.x, cursorSize.y);
        }
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
