using UnityEngine;

/// <summary>
/// pulsates the sprite by changing its scale and contrast over time
/// </summary>
public class PulsatingSprite : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float scaleMultiplier = 0.2f;
    [SerializeField] private float contrastMultiplier = 0.3f;

    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Material spriteMaterial;
    private Color originalColor;

    /// <summary>
    /// get original scale and color, create a copy of the material to avoid affecting other sprites
    /// </summary>
    void Start()
    {
        // Ulož původní hodnoty
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("PulsatingSprite: SpriteRenderer komponenta nenalezena!");
            return;
        }

        // Vytvoř kopii materiálu pro tento sprite
        spriteMaterial = new Material(spriteRenderer.material);
        spriteRenderer.material = spriteMaterial;
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// change scale and contrast over time using a sine wave
    /// </summary>
    void Update()
    {
        if (spriteRenderer == null) return;

        // Vypočítej pulse hodnotu pomocí sinu (-1 až 1, pak převeď na 0 až 1)
        float pulseValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        // Aplikuj změnu velikosti
        float scaleChange = 1f + (pulseValue * scaleMultiplier);
        transform.localScale = originalScale * scaleChange;

        // Aplikuj změnu kontrastu/jasu
        float brightness = 1f + (pulseValue * contrastMultiplier);
        Color newColor = originalColor * brightness;
        newColor.a = originalColor.a; // Zachovej původní alpha
        spriteRenderer.color = newColor;
    }

    /// <summary>
    /// destroy the created material to avoid memory leaks
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