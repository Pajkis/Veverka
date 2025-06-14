using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Audio sounds to buttons
/// </summary>
public class UIButtonAudioHook : MonoBehaviour
{
    [SerializeField] private string buttonTag = "UIButton";

    // hook all butons with tag
    void Start()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag(buttonTag);

        foreach (GameObject btnObj in buttons)
        {
            Button button = btnObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlaySound(SoundChannel.SoundUI, UiEnum.ButtonClick);
                });
            }

            // Add hover sound via EventTrigger
            AddHoverSound(btnObj);
        }       
    }


    void AddHoverSound(GameObject buttonObj)
    {
        EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObj.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) =>
        {
            AudioManager.Instance?.PlaySound(SoundChannel.SoundUI, UiEnum.ButtonHover);
        });

        trigger.triggers.Add(entry);
    }
}
