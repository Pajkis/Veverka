using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple random tips display system for Unity
/// Automatically cycles through messages every 5-10 seconds
/// </summary>
public class RandomTipsDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI messageText; // TextMeshPro component for displaying messages

    [Header("Timing Settings")]
    [SerializeField] private float minDisplayTime = 5f; // Minimum time between messages (seconds)
    [SerializeField] private float maxDisplayTime = 10f; // Maximum time between messages (seconds)
    [SerializeField] private float fadeDuration = 0.5f; // Duration of fade animations (seconds)

    [Header("Messages")]
    [SerializeField] private List<string> tipMessages = new List<string>(); // List of messages to display

    // Internal variables
    private Coroutine displayCoroutine;
    private int lastMessageIndex = -1; // Prevents showing the same message twice in a row

    // Default messages if none are provided
    private string[] defaultMessages = {
        "Tip: Sometimes you have try moving without thinking to find the correct way",
        "Hint: Try different strategies to solve problems. Sorry, this is obvious advice from AI.",
        "Hint: The loading of a level takes almost no time, I just like you wait 2s until the level starts",
        "Hint: Trying to put normal nut into a hole is like throwing your money out of window",
        "Advice: Learn from your mistakes. Or keep repeating them, if you enjoy it!",
        "Tip: Patience leads to better results. Lack of patience leads to breaking your keyboard and getting new better one",
        "Tip: Communication and proper teamwork solves many problems. Sadly, this is single player game.",
        "Hint: You can use 'Undo' to go back until last scored goal.",
        "Message: If you think some visuals and grafics does not match, you are right.",
    };

    /// <summary>
    /// Initialize the system when the object becomes active
    /// </summary>
    void Start()
    {
        InitializeComponent();
        StartDisplayingTips();
    }

    /// <summary>
    /// Set up all necessary components and default values
    /// </summary>
    void InitializeComponent()
    {
        // Use default messages if none provided
        if (tipMessages.Count == 0)
        {
            tipMessages.AddRange(defaultMessages);
        }

        // Auto-find TextMeshPro component if not assigned
        if (messageText == null)
        {
            messageText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Start with transparent text
        if (messageText != null)
        {
            Color textColor = messageText.color;
            textColor.a = 0f;
            messageText.color = textColor;
        }
    }

    /// <summary>
    /// Start the main tip display loop
    /// </summary>
    void StartDisplayingTips()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(DisplayTipsCoroutine());
    }

    /// <summary>
    /// Main loop that continuously shows random messages
    /// </summary>
    IEnumerator DisplayTipsCoroutine()
    {
        while (true)
        {
            // Get random message
            string randomMessage = GetRandomMessage();

            // Set the message text
            if (messageText != null)
            {
                messageText.text = randomMessage;
            }

            // Fade in text
            yield return StartCoroutine(FadeTextToAlpha(1f, fadeDuration));

            // Wait random time
            float waitTime = Random.Range(minDisplayTime, maxDisplayTime);
            yield return new WaitForSeconds(waitTime);

            // Fade out text
            yield return StartCoroutine(FadeTextToAlpha(0f, fadeDuration));

            // Brief pause between messages
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Get a random message, avoiding immediate repetition
    /// </summary>
    string GetRandomMessage()
    {
        if (tipMessages.Count <= 1)
        {
            return tipMessages.Count > 0 ? tipMessages[0] : "No messages available";
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, tipMessages.Count);
        }
        while (randomIndex == lastMessageIndex);

        lastMessageIndex = randomIndex;
        return tipMessages[randomIndex];
    }

    /// <summary>
    /// Smooth fade animation for TextMeshPro text only
    /// </summary>
    IEnumerator FadeTextToAlpha(float targetAlpha, float duration)
    {
        if (messageText == null) yield break;

        Color startColor = messageText.color;
        float startAlpha = startColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Interpolate only the alpha value, keep RGB unchanged
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, progress);
            messageText.color = newColor;

            yield return null;
        }

        // Ensure exact final alpha value
        Color finalColor = messageText.color;
        finalColor.a = targetAlpha;
        messageText.color = finalColor;
    }

    /// <summary>
    /// Clean up when object is destroyed
    /// </summary>
    void OnDestroy()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
    }
}