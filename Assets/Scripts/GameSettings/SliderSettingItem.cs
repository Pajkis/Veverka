using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// Game setting slider
/// </summary>
public class SliderSettingItem : MonoBehaviour
{
    #region fields

    [SerializeField] private GameSettingsEnum settingType;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;

    [SerializeField] private SettingDataRequestEvent settingDataRequestEvent;
    [SerializeField] private SettingDataBroadcastEvent settingDataBroadcastEvent;

    private Slider slider;
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI valueLabel;
    #endregion

    #region method
    /// <summary>
    /// Start method
    /// </summary>
    private void Start()
    {
        //get slider object
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError($"Slider not found in {gameObject.name}!");
            return;
        }

        //get textMesh components
        var textsComponents = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var component in textsComponents)
        {
            if (component != null)
            {
                //name label game object
                if (component.name.ToLower().Contains("name"))
                {
                    nameLabel = component;
                }
                else if (component.name.ToLower().Contains("value"))
                {
                    valueLabel = component;
                }
            }
            else
            {
                Debug.LogError($"TextMesh object {component} not found in {gameObject.name}!");
                return;
            }
        }

        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.onValueChanged.AddListener(OnSliderChanged);

        settingDataBroadcastEvent.AddListener(OnSettingData);
        nameLabel.text = SplitCamelCase(settingType.ToString());
        settingDataRequestEvent.Raise(settingType);
    }

    /// <summary>
    /// Reacts on slider value change and save it
    /// </summary>
    /// <param name="value"></param>
    private void OnSliderChanged(float value)
    {
        settingDataBroadcastEvent.Raise(new SettingDataPayload
        {
            Setting = settingType,
            Value = Mathf.RoundToInt(value)
        });

        if (valueLabel != null)
        {
            valueLabel.text = Mathf.RoundToInt(value).ToString();
        }
    }

    /// <summary>
    /// Changes EnumName (text without space) into text with spaces
    /// </summary>
    /// <param name="input">Text without spaces</param>
    /// <returns>Text with spaces</returns>
    private string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "(\\B[A-Z])", " $1");
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
        settingDataBroadcastEvent.RemoveListener(OnSettingData);
    }

    private void OnSettingData(SettingDataPayload payload)
    {
        if (payload.Setting != settingType)
            return;

        float value = Mathf.Clamp(payload.Value, minValue, maxValue);
        slider.SetValueWithoutNotify(value);
        if (valueLabel != null)
            valueLabel.text = Mathf.RoundToInt(value).ToString();
    }

    #endregion
}