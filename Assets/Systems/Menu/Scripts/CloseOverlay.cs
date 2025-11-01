using UnityEngine;

public class CloseOverlay : MonoBehaviour
{
    /// <summary>
    /// Handle on click quit button event with smooth animation.
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        // Find the parent canvas of this menu
        Canvas parentCanvas = gameObject.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Overlay menu does not include canvas - cannot close overlay", this);
            return;
        }

        GameObject overlay = parentCanvas.gameObject;
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

}
