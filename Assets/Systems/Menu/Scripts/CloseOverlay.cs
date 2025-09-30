using UnityEngine;

public class CloseOverlay : MonoBehaviour
{
    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        // Find the parent canvas of this menu
        GameObject overlay = gameObject.GetComponentInParent<Canvas>().gameObject;
        if (overlay == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "Overlay menu does not include canvas - cannot close overlay", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.SceneManager, $"Closing overlay: {overlay.name}", this);
        Destroy(overlay);
    }

}
