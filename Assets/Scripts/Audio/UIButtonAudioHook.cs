using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Audio sounds to buttons
/// </summary>
public class UIButtonAudioHook : MonoBehaviour
{
    [SerializeField] private string buttonTag = "UIButton";

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
        }
    }      
}
