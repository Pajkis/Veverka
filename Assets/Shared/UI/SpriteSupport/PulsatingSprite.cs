using UnityEngine;

/// <summary>
/// pulsates the sprite by changing its scale and contrast over time
/// </summary>
public class PulsatingSprite : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float _pulseSpeed = 2f;
    [SerializeField] private float _scaleMultiplier = 0.2f;
    [SerializeField] private float _contrastMultiplier = 0.3f;

    // Public properties for runtime configuration
    public float pulseSpeed
    {
        get => _pulseSpeed;
        set => _pulseSpeed = value;
    }

    public float scaleMultiplier
    {
        get => _scaleMultiplier;
        set => _scaleMultiplier = value;
    }

    public float contrastMultiplier
    {
        get => _contrastMultiplier;
        set => _contrastMultiplier = value;
    }

    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Material spriteMaterial;
    private Color originalColor;

    /// <summary>
    /// Get original scale and color, create a copy of the material to avoid affecting other sprites
    /// </summary>
    void Start()
    {
        // Store original values
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            DebugLogger.LogError(DebugLogCategory.Gameplay, "SpriteRenderer component not found!", this);
            return;
        }

        // Create material copy for this sprite
        spriteMaterial = new Material(spriteRenderer.material);
        spriteRenderer.material = spriteMaterial;
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Change scale and contrast over time using a sine wave
    /// </summary>
    void Update()
    {
        if (!enabled) return;  // Respect component enabled state
        if (spriteRenderer == null) return;

        // Calculate pulse value using sine (-1 to 1, then convert to 0 to 1)
        float pulseValue = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) * 0.5f;

        // Apply scale change
        float scaleChange = 1f + (pulseValue * _scaleMultiplier);
        transform.localScale = originalScale * scaleChange;

        // Apply contrast/brightness change
        float brightness = 1f + (pulseValue * _contrastMultiplier);
        Color newColor = originalColor * brightness;
        newColor.a = originalColor.a; // Preserve original alpha
        spriteRenderer.color = newColor;
    }

    /// <summary>
    /// Destroy the created material to avoid memory leaks
    /// </summary>
    void OnDestroy()
    {
        // Clean up material when object is destroyed
        if (spriteMaterial != null)
        {
            DestroyImmediate(spriteMaterial);
        }
    }
}