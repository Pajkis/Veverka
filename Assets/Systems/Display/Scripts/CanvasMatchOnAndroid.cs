using UnityEngine;

public class CanvasMatchOnAndroid : MonoBehaviour
{
    [SerializeField] DisplayConfig displayConfig; // Reference to the active DisplayConfigSO
    [SerializeField] Vector2 fallbackAspect = new(16, 9); // Fallback aspect ratio if no profile is found

    void Start()
    {
#if UNITY_ANDROID
        var scaler = GetComponent<CanvasScaler>();
        if (!scaler) return;

        var profile = displayConfig ? displayConfig.ResolveProfile() : null;
        Vector2 aspect = profile != null ? profile.targetAspect : fallbackAspect;

        float window = (float)Screen.width / Screen.height;
        float target = aspect.x / aspect.y;

        scaler.matchWidthOrHeight = (window < target) ? 0f : 1f;
#endif
    }
}