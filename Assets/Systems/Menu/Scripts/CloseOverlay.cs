using UnityEngine;

public class CloseOverlay : MonoBehaviour
{
    /// <summary>
    /// Handle on click quit button event with smooth animation.
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        // Find the root canvas (the one with OverlaySetup or the topmost canvas)
        GameObject overlay = FindRootOverlay();
        if (overlay == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Overlay menu does not include canvas - cannot close overlay", this);
            return;
        }
        DebugLogger.Log(DebugLogCategory.SceneManager, $"Closing overlay: {overlay.name}", this);

        // Try to close with animation
        OverlayAnimationController animController = overlay.GetComponent<OverlayAnimationController>();
        if (animController != null)
        {
            // Animated close (will destroy after animation completes)
            animController.CloseWithAnimation();
        }
        else
        {
            // Fallback to immediate destroy if no animation controller
            DebugLogger.Log(DebugLogCategory.SceneManager, "No animation controller found - destroying immediately", this);
            Destroy(overlay);
        }
    }

    /// <summary>
    /// Finds the root overlay GameObject by looking for OverlaySetup component
    /// or the topmost Canvas in the hierarchy.
    /// </summary>
    private GameObject FindRootOverlay()
    {
        // First try to find OverlaySetup (definitive root marker)
        OverlaySetup overlaySetup = gameObject.GetComponentInParent<OverlaySetup>();
        if (overlaySetup != null)
        {
            return overlaySetup.gameObject;
        }

        // Fallback: find the topmost canvas in hierarchy
        Canvas[] parentCanvases = gameObject.GetComponentsInParent<Canvas>();
        if (parentCanvases.Length > 0)
        {
            // Last in array is the topmost (root) canvas
            return parentCanvases[parentCanvases.Length - 1].gameObject;
        }

        return null;
    }
}
