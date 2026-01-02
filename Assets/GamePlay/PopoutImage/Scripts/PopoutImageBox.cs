using UnityEngine;
using System.Collections;

/// <summary>
/// Component that handles popout image animation - movement and fade
/// Uses SpriteRenderer for world space rendering with manual sprite assignment in prefabs.
/// Supports multiple prefab variants with different sprites or sprite arrays (via RandomSprite component).
/// </summary>
public class PopoutImageBox : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform cachedTransform;

    // Animation parameters (set by Init)
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float animationDuration;
    private float fadeStartTime;
    private Color baseTintColor;

    private void Awake()
    {
        // Find required components (must be manually added to prefab)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay,
                "PopoutImageBox requires a SpriteRenderer component!", this);
            return;
        }

        cachedTransform = transform;
    }

    /// <summary>
    /// Initialize the popout image with animation parameters.
    /// Sprite is expected to be pre-assigned in the prefab (not set from payload).
    /// </summary>
    public void Init(PopoutImageEventPayload payload)
    {
        if (!payload.IsValid())
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "PopoutImageBox: Invalid payload", this);
            Destroy(gameObject);
            return;
        }

        // Apply tint color to existing sprite (sprite is set in prefab)
        Color spriteColor = payload.TintColor;
        spriteColor.a = 1f; // Start fully visible
        spriteRenderer.color = spriteColor;

        // Set scale (accounting for tile size)
        float tileSize = 1f; // Default fallback
        if (DisplaySettings.Instance != null && DisplaySettings.Instance.ActiveProfile != null)
        {
            tileSize = DisplaySettings.Instance.ActiveProfile.tileSize;
        }

        // Scale calculation for world space
        // Applies both tileSize and payload.Scale
        cachedTransform.localScale = Vector3.one * payload.Scale * tileSize;

        // Calculate positions (already in world space)
        startPosition = cachedTransform.localPosition;
        Vector2 directionVector = payload.AnimationDirection.ToVector2();
        Vector2 moveOffset = directionVector * payload.AnimationDistance;
        targetPosition = startPosition + (Vector3)moveOffset;

        // Store animation parameters
        animationDuration = payload.AnimationDuration;
        fadeStartTime = payload.FadeStartTime;
        baseTintColor = payload.TintColor; // Store for fading

        // Start animation with delay
        if (payload.StartDelay > 0)
        {
            StartCoroutine(DelayedStart(payload.StartDelay));
        }
        else
        {
            StartCoroutine(AnimateSequence());
        }

        // Auto-destroy after animation completes (including delay)
        Destroy(gameObject, payload.StartDelay + animationDuration + 0.1f);
    }

    /// <summary>
    /// Waits for StartDelay before starting the animation
    /// </summary>
    private IEnumerator DelayedStart(float delay)
    {
        // Hide the sprite during delay
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
        }

        yield return new WaitForSeconds(delay);

        // Show sprite and start animation
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        StartCoroutine(AnimateSequence());
    }

    /// <summary>
    /// Main animation coroutine - handles both movement and fading
    /// Uses SpriteRenderer.color for fading and transform.localPosition for world space movement
    /// </summary>
    private IEnumerator AnimateSequence()
    {
        float elapsedTime = 0f;

        // Start fully visible
        Color currentColor = baseTintColor;
        currentColor.a = 1f;
        spriteRenderer.color = currentColor;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;

            // Move position (smooth interpolation) - world space
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            cachedTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

            // Handle fading via sprite color alpha
            if (elapsedTime >= fadeStartTime)
            {
                float fadeProgress = (elapsedTime - fadeStartTime) / (animationDuration - fadeStartTime);
                currentColor = baseTintColor;
                currentColor.a = Mathf.Lerp(1f, 0f, fadeProgress);
                spriteRenderer.color = currentColor;
            }

            yield return null;
        }

        // Ensure final state
        cachedTransform.localPosition = targetPosition;
        currentColor = baseTintColor;
        currentColor.a = 0f;
        spriteRenderer.color = currentColor;
    }
}
