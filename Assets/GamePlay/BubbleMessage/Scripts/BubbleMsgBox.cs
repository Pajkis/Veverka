using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BubbleMsgBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;

    // These will be found automatically
    private Image bubbleImage;
    private TextMeshProUGUI bubbleText;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Find components in children
        bubbleImage = GetComponentInChildren<Image>();
        bubbleText = GetComponentInChildren<TextMeshProUGUI>();

        // Try to get existing CanvasGroup or add one
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // If messageText is not assigned, use the found text component
        if (messageText == null)
        {
            messageText = bubbleText;
        }
    }

    public void Init(string message, float timeToFade)
    {
        if (messageText == null)
        {
            Debug.LogError("Message Text is not found in the prefab.");
            return;
        }

        messageText.text = message;
        StartCoroutine(FadeSequence(timeToFade));
        Destroy(gameObject, timeToFade + 0.1f);
    }

    /// <summary>
    /// Fade the entire bubble message box using CanvasGroup
    /// This will fade all UI elements (image, text, etc.) together
    /// </summary>
    IEnumerator FadeSequence(float duration)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = 1f;
        float targetAlpha = 0f;
        float elapsedTime = 0f;

        // Ensure we start at full opacity
        canvasGroup.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Use a smooth fade curve (optional - you can use linear with just progress)
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothProgress);

            yield return null;
        }

        // Ensure exact final alpha value
        canvasGroup.alpha = targetAlpha;
    }
}