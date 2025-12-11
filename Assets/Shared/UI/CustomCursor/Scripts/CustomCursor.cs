using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private Vector2 hotspot;

    private Canvas canvas;
    private RectTransform canvasRect;
    private bool isActive;

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

    void Start()
    {
        if (!isActive)
            return;

        // Load hotspot from DisplaySettings config
        if (DisplaySettings.Instance != null && DisplaySettings.Instance.ActiveProfile != null)
        {
            hotspot = DisplaySettings.Instance.ActiveProfile.cursorHotspot;
        }
        else
        {
            // Fallback default
            hotspot = new Vector2(50f, -50f);
        }

        Cursor.visible = false;
    }

    void Update()
    {
        if (!isActive || cursorImage == null)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out pos
        );

        cursorImage.localPosition = pos + hotspot;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
