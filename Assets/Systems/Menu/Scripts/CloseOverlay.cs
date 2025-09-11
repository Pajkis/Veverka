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
            Debug.LogWarning("Overlay menu does not include canvas");
            return;
        }

        Destroy(overlay);
    }

}
