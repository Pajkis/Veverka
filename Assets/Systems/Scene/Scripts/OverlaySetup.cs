using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic overlay setup script - assigns camera and sorting layers based on overlay level.
/// Works for all overlay types (PauseMenu=1, Settings=2, Tutorial=3, Notification=4).
///
/// Expected Canvas structure:
/// - Root GameObject with Canvas → Overlay{X}Background (tag: Background)
/// - CanvasFrame (or tag "Frame") → Overlay{X}Frame
/// - CanvasContent (or tag "Content") → Overlay{X}Content
///
/// Detection priority: Tag > Name contains keyword
/// </summary>
public class OverlaySetup : MonoBehaviour
{
    [Header("Display Configuration")]
    [SerializeField] private DisplaySettings displaySettings;

    [Header("Overlay Configuration")]
    [Tooltip("1=PauseMenu/LevelFinished/Credits, 2=Settings/HowToPlay, 3=Tutorial, 4=Notification/LevelValidation")]
    [SerializeField] private int overlayLevel = 1; // 1-4
    

    // Tag constants
    private const string TAG_BACKGROUND = "Background";
    private const string TAG_FRAME = "Frame";
    private const string TAG_CONTENT = "Content";

    private void Awake()
    {
        SetupCanvases();
    }

    private void SetupCanvases()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            DebugLogger.LogError(DebugLogCategory.UI,
                "Main camera not found - Canvases cannot render properly!", this);
            return;
        }

        int backgroundCount = 0;
        int frameCount = 0;
        int contentCount = 0;

        // Setup root canvas first (this GameObject)
        Canvas rootCanvas = GetComponent<Canvas>();
        if (rootCanvas != null)
        {
            rootCanvas.worldCamera = mainCamera;
            rootCanvas.sortingLayerName = $"Overlay{overlayLevel}Background";
            rootCanvas.sortingOrder = 0;
            backgroundCount++;

            DebugLogger.Log(DebugLogCategory.UI,
                $"Root Canvas '{rootCanvas.name}' → {rootCanvas.sortingLayerName}", this);
        }

        // Setup CanvasScaler on root
        CanvasScaler rootScaler = GetComponent<CanvasScaler>();
        if (rootScaler != null)
        {
            rootScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            rootScaler.referenceResolution = new Vector2(1920, 1080);
            rootScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            rootScaler.matchWidthOrHeight = 1f; // Balanced
        }

        // Find all Canvas components in children (excluding root)
        Canvas[] allCanvases = GetComponentsInChildren<Canvas>(true);

        foreach (var canvas in allCanvases)
        {
            // Skip root canvas (already handled)
            if (canvas.gameObject == gameObject)
                continue;

            // Assign camera to all canvases
            canvas.worldCamera = mainCamera;

            // Determine sorting layer by tag first, then by name
            string sortingLayer = DetermineSortingLayer(canvas.gameObject);

            if (!string.IsNullOrEmpty(sortingLayer))
            {
                canvas.sortingLayerName = sortingLayer;
                canvas.sortingOrder = 0;

                // Count by type
                if (sortingLayer.Contains("Background"))
                    backgroundCount++;
                else if (sortingLayer.Contains("Frame"))
                    frameCount++;
                else if (sortingLayer.Contains("Content"))
                    contentCount++;

                DebugLogger.Log(DebugLogCategory.UI,
                    $"Canvas '{canvas.name}' → {sortingLayer}", this);
            }
        }

        CanvasScaler[] allScalers = GetComponentsInChildren<CanvasScaler>(true);
        foreach (var scaler in allScalers)
        {
            // Unified settings for all CanvasScalers in the overlay
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f; 
        }

        DebugLogger.Log(DebugLogCategory.UI,
            $"Overlay{overlayLevel}: Setup complete - {backgroundCount} background, {frameCount} frame, {contentCount} content canvases", this);
   
       
    }

    /// <summary>
    /// Determines sorting layer based on tag (priority) or name.
    /// </summary>
    private string DetermineSortingLayer(GameObject obj)
    {
        // Check tag first (priority)
        if (obj.CompareTag(TAG_CONTENT))
            return $"Overlay{overlayLevel}Content";

        if (obj.CompareTag(TAG_FRAME))
            return $"Overlay{overlayLevel}Frame";

        if (obj.CompareTag(TAG_BACKGROUND))
            return $"Overlay{overlayLevel}Background";

        // Fallback to name checking
        // Order matters: check Content BEFORE Frame (in case name contains both)
        string objName = obj.name;

        if (objName.Contains("Content") || objName.Contains("FrameComponent"))
            return $"Overlay{overlayLevel}Content";

        if (objName.Contains("Frame"))
            return $"Overlay{overlayLevel}Frame";

        if (objName.Contains("Background"))
            return $"Overlay{overlayLevel}Background";

        return null;
    }
}
