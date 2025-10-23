using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Audio sounds to buttons 
/// </summary>
public class UIButtonAudioHook : MonoBehaviour
{
    [SerializeField] private AudioEvents audioEvents;

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

        DebugLogger.Log(DebugLogCategory.Audio, $"UIButtonAudioHook processing {buttons.Length} buttons", this);

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

            DebugLogger.Log(DebugLogCategory.Audio, $"Added audio to button: {button.name}", this);
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.Audio, $"Button {button.name} already has audio listeners", this);
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

        DebugLogger.Log(DebugLogCategory.Audio, $"Added hover sound to button: {buttonObj.name}", this);
    }
}