using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Audio sounds to buttons 
/// </summary>
public class UIButtonAudioHook : MonoBehaviour
{
    [SerializeField] private AudioEvents audioEvents;

    [Header("Debug")]
    [SerializeField] private bool logButtonProcessing = false;

    // Hook all buttons with audio
    void Start()
    {
        // Delay the setup to ensure all buttons are initialized
        StartCoroutine(DelayedAudioHookSetup());
    }

    private System.Collections.IEnumerator DelayedAudioHookSetup()
    {
        // Wait for end of frame to ensure all UI elements are initialized
        yield return null;

        Button[] buttons = GetComponentsInChildren<Button>(includeInactive: true);

        if (logButtonProcessing)
            Debug.Log($"UIButtonAudioHook processing {buttons.Length} buttons");

        foreach (Button button in buttons)
        {
            //add audio listener to button
            AddAudioToButton(button);
            AddHoverSound(button.gameObject);
        }
    }

    /// <summary>
    /// Add audio listener to button
    /// </summary>
    /// <param name="button">Target button</param>
    private void AddAudioToButton(Button button)
    {
        // Kontrola, jestli už není audio přidáno
        bool hasAudioListener = false;

        // Check existing EventTrigger for PointerEnter event
        EventTrigger existingTrigger = button.GetComponent<EventTrigger>();
        if (existingTrigger != null)
        {
            foreach (var entry in existingTrigger.triggers)
            {
                if (entry.eventID == EventTriggerType.PointerEnter)
                {
                    hasAudioListener = true;
                    break;
                }
            }
        }

        // add listener only if not already present - prevents duplicate sounds
        if (!hasAudioListener)
        {            
            button.onClick.AddListener(() =>
            {
                audioEvents?.Raise(new AudioEventPayload { EventType = AudioEventType.PlayUi, Ui = UiType.ButtonClick });
            });

            if (logButtonProcessing)
                Debug.Log($"Added audio to button: {button.name}");
        }
        else if (logButtonProcessing)
        {
            Debug.Log($"Button {button.name} already has audio listeners");
        }
    }

    /// <summary>
    /// Add hover sound eventtrigger 
    /// </summary>
    /// <param name="buttonObj">target button on current screen</param>
    void AddHoverSound(GameObject buttonObj)
    {
        EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObj.AddComponent<EventTrigger>();
        }

        // Check if a hover sound entry already exists
        foreach (var entry in trigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerEnter)
            {
                // Already added
                return;
            }
        }

        // add hover entry
        EventTrigger.Entry entryHover = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryHover.callback.AddListener((data) =>
        {
            audioEvents?.Raise(new AudioEventPayload { EventType = AudioEventType.PlayUi, Ui = UiType.ButtonHover });
        });

        trigger.triggers.Add(entryHover);

        if (logButtonProcessing)
            Debug.Log($"Added hover sound to button: {buttonObj.name}");
    }
}