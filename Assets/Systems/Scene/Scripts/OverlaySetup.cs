using UnityEngine;

/// <summary>
/// Generic overlay setup script - assigns camera and sorting layers based on overlay level.
/// Works for all overlay types (PauseMenu=1, Settings=2, Tutorial=3, Notification=4).
///
/// Expected Canvas structure (children of this GameObject):
/// - CanvasBackground (or name contains "Background") → Overlay{X}Background
/// - CanvasFrame (or name contains "Frame" but not "FrameComponent") → Overlay{X}Frame
/// - CanvasFrameComponent (or name contains "FrameComponent") → Overlay{X}FrameComponent
/// </summary>
public class OverlaySetup : MonoBehaviour
{
    [Header("Overlay Configuration")]
    [SerializeField] private int overlayLevel = 1; // 1-4
    [Tooltip("1=PauseMenu/LevelFinished/Credits, 2=Settings/HowToPlay, 3=Tutorial, 4=Notification/LevelValidation")]

    private void Start()
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
        int componentCount = 0;

        // Find all Canvas components in children
        Canvas[] allCanvases = GetComponentsInChildren<Canvas>(true);

        foreach (var canvas in allCanvases)
        {
            // Assign camera to all canvases
            canvas.worldCamera = mainCamera;

            string canvasName = canvas.name;

            // Check canvas name to determine sorting layer
            // Order matters: check FrameComponent BEFORE Frame (since "FrameComponent" contains "Frame")
            if (canvasName.Contains("FrameComponent"))
            {
                canvas.sortingLayerName = $"Overlay{overlayLevel}FrameComponent";
                canvas.sortingOrder = 0;
                componentCount++;

                DebugLogger.Log(DebugLogCategory.UI,
                    $"Canvas '{canvasName}' → {canvas.sortingLayerName}", this);
            }
            else if (canvasName.Contains("Frame"))
            {
                canvas.sortingLayerName = $"Overlay{overlayLevel}Frame";
                canvas.sortingOrder = 0;
                frameCount++;

                DebugLogger.Log(DebugLogCategory.UI,
                    $"Canvas '{canvasName}' → {canvas.sortingLayerName}", this);
            }
            else if (canvasName.Contains("Background"))
            {
                canvas.sortingLayerName = $"Overlay{overlayLevel}Background";
                canvas.sortingOrder = 0;
                backgroundCount++;

                DebugLogger.Log(DebugLogCategory.UI,
                    $"Canvas '{canvasName}' → {canvas.sortingLayerName}", this);
            }
        }

        DebugLogger.Log(DebugLogCategory.UI,
            $"Overlay{overlayLevel}: Setup complete - {backgroundCount} background, {frameCount} frame, {componentCount} component canvases", this);
    }
}
