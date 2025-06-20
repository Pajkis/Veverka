using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Audio sounds to buttons
/// </summary>
public class UIButtonAudioHook : MonoBehaviour
{
    // hook all butons with tag
    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(includeInactive: true);

        foreach (Button button in buttons)
        {
            // Prevent double-click listeners
            button.onClick.RemoveAllListeners();

            // Add click sound
            button.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlaySound(SoundChannel.SoundUI, UiEnum.ButtonClick);
            });
            
            // Add hover sound via EventTrigger
            AddHoverSound(button.gameObject);
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
            AudioManager.Instance?.PlaySound(SoundChannel.SoundUI, UiEnum.ButtonHover);
        });

        trigger.triggers.Add(entryHover);
    }
}
