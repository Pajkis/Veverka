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

    [SerializeField] private SettingEvents settingEvents;

    private Slider slider;
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI valueLabel;
    #endregion

    #region method
    /// <summary>
    /// Start method
    /// </summary>
    private void Awake()
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"SliderSettingItem Awake - Setting: {settingType}", this);

        //get slider object
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            DebugLogger.LogError(DebugLogCategory.Settings, $"Slider component not found in {gameObject.name}!", this);
            return;
        }
        DebugLogger.Log(DebugLogCategory.Settings, $"Slider component found for {settingType}", this);

        //get textMesh components
        var textsComponents = GetComponentsInChildren<TextMeshProUGUI>();
        DebugLogger.Log(DebugLogCategory.Settings, $"Found {textsComponents.Length} TextMeshPro components", this);

        foreach (var component in textsComponents)
        {
            if (component != null)
            {
                //name label game object
                if (component.name.ToLower().Contains("name"))
                {
                    nameLabel = component;
                    DebugLogger.Log(DebugLogCategory.Settings, $"Found name label component: {component.name}", this);
                }
                else if (component.name.ToLower().Contains("value"))
                {
                    valueLabel = component;
                    DebugLogger.Log(DebugLogCategory.Settings, $"Found value label component: {component.name}", this);
                }
            }
            else
            {
                DebugLogger.LogError(DebugLogCategory.Settings, $"TextMesh component is null in {gameObject.name}!", this);
                return;
            }
        }

        slider.minValue = minValue;
        slider.maxValue = maxValue;
        DebugLogger.Log(DebugLogCategory.Settings, $"Slider {settingType} configured with range [{minValue}, {maxValue}]", this);
    }

    /// <summary>
    /// add listneres and init on enable
    /// </summary>
    private void OnEnable()
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"SliderSettingItem OnEnable - Setting: {settingType}", this);

        slider.onValueChanged.AddListener(OnSliderChanged);
        settingEvents.AddListener(OnSettingEvent);
        DebugLogger.Log(DebugLogCategory.Settings, $"Event listeners added for {settingType}", this);

        nameLabel.text = SplitCamelCase(settingType.ToString());
        DebugLogger.Log(DebugLogCategory.Settings, $"Name label set to: '{nameLabel.text}' for {settingType}", this);

        settingEvents.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataRequest,
            Setting = settingType
        });
        DebugLogger.Log(DebugLogCategory.Settings, $"DataRequest event raised for {settingType}", this);
    }


    /// <summary>
    /// Remove listeners
    /// </summary>
    private void OnDisable()
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"SliderSettingItem OnDisable - Setting: {settingType}", this);
        slider.onValueChanged.RemoveListener(OnSliderChanged);
        settingEvents.RemoveListener(OnSettingEvent);
        DebugLogger.Log(DebugLogCategory.Settings, $"Event listeners removed for {settingType}", this);
    }


    /// <summary>
    /// Reacts on slider value change and save it
    /// </summary>
    /// <param name="value"></param>
    private void OnSliderChanged(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        DebugLogger.Log(DebugLogCategory.Settings, $"Slider {settingType} value changed to: {value} (rounded: {intValue})", this);

        settingEvents.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataBroadcast,
            Setting = settingType,
            Value = intValue
        });

        if (valueLabel != null)
        {
            valueLabel.text = intValue.ToString();
            DebugLogger.Log(DebugLogCategory.Settings, $"Value label updated to: {intValue} for {settingType}", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Value label is null for {settingType}, cannot update display", this);
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

    
    private void OnSettingEvent(SettingEventPayload payload)
    {
        if (payload.EventType != SettingsEventType.DataBroadcast ||
            payload.Setting != settingType)
        {
            return;
        }

        DebugLogger.Log(DebugLogCategory.Settings, $"OnSettingEvent for {settingType} - received value: {payload.Value}", this);

        float value = Mathf.Clamp(payload.Value, minValue, maxValue);
        if (value != payload.Value)
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Value {payload.Value} for {settingType} was clamped to {value} (range: [{minValue}, {maxValue}])", this);
        }

        slider.SetValueWithoutNotify(value);
        DebugLogger.Log(DebugLogCategory.Settings, $"Slider {settingType} value set to: {value} (without triggering events)", this);

        if (valueLabel != null)
        {
            int displayValue = Mathf.RoundToInt(value);
            valueLabel.text = displayValue.ToString();
            DebugLogger.Log(DebugLogCategory.Settings, $"Value label for {settingType} updated to: {displayValue}", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Value label is null for {settingType}", this);
        }
    }

    #endregion
}