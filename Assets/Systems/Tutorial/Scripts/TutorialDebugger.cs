using UnityEngine;

/// <summary>
/// Debug helper to diagnose tutorial tile size issues
/// </summary>
public class TutorialDebugger : MonoBehaviour
{
    private void Start()
    {
        LogHierarchy();
    }

    [ContextMenu("Log Tutorial Hierarchy")]
    private void LogHierarchy()
    {
        Debug.Log("=== TUTORIAL OVERLAY DEBUG ===");

        // Log this object
        LogTransform("TutorialOverlay (this)", transform);

        // Log children
        foreach (Transform child in transform)
        {
            LogTransform($"  Child: {child.name}", child);

            // Log grandchildren
            foreach (Transform grandchild in child)
            {
                LogTransform($"    Grandchild: {grandchild.name}", grandchild);
            }
        }

        // Log camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log($"CAMERA: orthographic={mainCam.orthographic}, size={mainCam.orthographicSize}, pos={mainCam.transform.position}");
        }

        Debug.Log("=== END DEBUG ===");
    }

    private void LogTransform(string label, Transform t)
    {
        Debug.Log($"{label}: pos={t.position}, localPos={t.localPosition}, scale={t.localScale}, lossyScale={t.lossyScale}");
    }
}
