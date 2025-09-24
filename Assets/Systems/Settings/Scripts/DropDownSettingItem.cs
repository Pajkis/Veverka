using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Universal dropdown setting item that works with any enum type.
/// Select the enum type directly in the Inspector.
/// </summary>
public class DropDownSettingItem : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameSettingsEnum settingType;

    [Header("Enum Configuration")]
    [SerializeField] private EnumType enumType;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TextMeshProUGUI nameLabel;

    [Header("Events")]
    [SerializeField] private SettingEvents settingEvents;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"DropDownSettingItem Awake - Setting: {settingType}, EnumType: {enumType?.TypeName}", this);

        // Try to get components if not set in Inspector
        if (dropdown == null)
        {
            dropdown = GetComponentInChildren<TMP_Dropdown>(true);
            DebugLogger.Log(DebugLogCategory.Settings, dropdown != null ? "TMP_Dropdown component found" : "TMP_Dropdown component not found", this);
        }

        // Try to find nameLabel if not set in Inspector
        if (nameLabel == null)
        {
            var labels = GetComponentsInChildren<TextMeshProUGUI>(true);
            DebugLogger.Log(DebugLogCategory.Settings, $"Searching for name label in {labels.Length} TextMeshPro components", this);
            foreach (var label in labels)
            {
                if (label.name.Contains("NameLabel"))
                {
                    nameLabel = label;
                    DebugLogger.Log(DebugLogCategory.Settings, $"Found name label component: {label.name}", this);
                    break;
                }
            }
        }

        // Validate components
        if (dropdown == null)
        {
            DebugLogger.LogError(DebugLogCategory.Settings, $"TMP_Dropdown not found on {name} - disabling component", this);
            enabled = false;
            return;
        }

        if (nameLabel == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Name label not found on {name}", this);
        }

        DebugLogger.Log(DebugLogCategory.Settings, $"Component validation complete for {settingType}, populating dropdown", this);
        PopulateDropdown();
    }

    private void OnEnable()
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"DropDownSettingItem OnEnable - Setting: {settingType}", this);

        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(OnDropDownValueChanged);
            settingEvents.AddListener(OnSettingEvent);
            DebugLogger.Log(DebugLogCategory.Settings, $"Event listeners added for {settingType}", this);

            settingEvents.Raise(new SettingEventPayload
            {
                EventType = SettingsEventType.DataRequest,
                Setting = settingType
            });
            DebugLogger.Log(DebugLogCategory.Settings, $"DataRequest event raised for {settingType}", this);
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.Settings, $"Dropdown is null in OnEnable for {settingType}", this);
        }
    }

    private void OnDisable()
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"DropDownSettingItem OnDisable - Setting: {settingType}", this);

        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
            settingEvents.RemoveListener(OnSettingEvent);
            DebugLogger.Log(DebugLogCategory.Settings, $"Event listeners removed for {settingType}", this);
        }
    }
    #endregion

    #region event handlers

    /// <summary>
    /// Gets setting data from broadcast event and updates dropdown and label
    /// </summary>
    /// <param name="payload"></param>
    private void OnSettingEvent(SettingEventPayload payload)
    {
        if (payload.EventType != SettingsEventType.DataBroadcast ||
            payload.Setting != settingType)
            return;

        DebugLogger.Log(DebugLogCategory.Settings, $"OnSettingEvent for {settingType} - received value: {payload.Value}", this);

        int settingValue = Mathf.RoundToInt(payload.Value);
        int dropdownIndex = FindEnumIndex(settingValue);
        DebugLogger.Log(DebugLogCategory.Settings, $"Setting value {settingValue} maps to dropdown index: {dropdownIndex}", this);

        if (dropdownIndex >= 0 && dropdownIndex < dropdown.options.Count)
        {
            dropdown.SetValueWithoutNotify(dropdownIndex);
            DebugLogger.Log(DebugLogCategory.Settings, $"Dropdown {settingType} set to index {dropdownIndex} (option: {dropdown.options[dropdownIndex].text})", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Invalid dropdown index {dropdownIndex} for {settingType}, defaulting to index 0", this);
            dropdown.SetValueWithoutNotify(0);
        }

        if (nameLabel != null)
        {
            string formattedName = FormatEnumName(settingType.ToString());
            nameLabel.text = formattedName;
            DebugLogger.Log(DebugLogCategory.Settings, $"Name label for {settingType} set to: '{formattedName}'", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Name label is null for {settingType}", this);
        }
    }
    #endregion

    #region Initialization Methods
    /// <summary>
    /// Populates the dropdown with options based on the selected enum
    /// </summary>
    private void PopulateDropdown()
    {
        if (enumType == null || dropdown == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"Cannot populate dropdown - enumType: {enumType != null}, dropdown: {dropdown != null}", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Settings, $"Populating dropdown for {settingType} with enum type: {enumType.TypeName}", this);

        dropdown.options.Clear();

        var enumValues = enumType.GetValues();
        DebugLogger.Log(DebugLogCategory.Settings, $"Found {enumValues.Length} enum values for {enumType.TypeName}", this);

        foreach (var enumValue in enumValues)
        {
            string optionText = FormatEnumName(enumValue.ToString());
            dropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
            DebugLogger.Log(DebugLogCategory.Settings, $"Added dropdown option: '{optionText}' (enum: {enumValue})", this);
        }

        dropdown.RefreshShownValue();
        DebugLogger.Log(DebugLogCategory.Settings, $"Dropdown population complete for {settingType} - {dropdown.options.Count} options added", this);
    }

    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles dropdown value changes
    /// </summary>
    /// <param name="dropdownIndex">Selected dropdown index</param>
    public void OnDropDownValueChanged(int dropdownIndex)
    {
        if (enumType == null)
        {
            DebugLogger.LogError(DebugLogCategory.Settings, $"EnumType is null for {settingType} dropdown change", this);
            return;
        }

        var enumValues = enumType.GetValues();
        if (dropdownIndex < 0 || dropdownIndex >= enumValues.Length)
        {
            DebugLogger.LogError(DebugLogCategory.Settings, $"Invalid dropdown index {dropdownIndex} for {settingType} (valid range: 0-{enumValues.Length - 1})", this);
            return;
        }

        // Get the enum value at the selected index
        var selectedEnumValue = enumValues[dropdownIndex];
        int enumIntValue = enumType.GetIntValue(selectedEnumValue);

        DebugLogger.Log(DebugLogCategory.Settings, $"Dropdown {settingType} changed to index {dropdownIndex}: {selectedEnumValue} (int value: {enumIntValue})", this);

        // Update game settings
        settingEvents.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataBroadcast,
            Setting = settingType,
            Value = enumIntValue
        });
        DebugLogger.Log(DebugLogCategory.Settings, $"DataBroadcast event raised for {settingType} with value {enumIntValue}", this);
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Formats enum name by adding spaces before uppercase letters and making all letters lowercase
    /// </summary>
    /// <param name="enumName">Raw enum name</param>
    /// <returns>Formatted name with spaces and lowercase letters</returns>
    private string FormatEnumName(string enumName)
    {
        if (string.IsNullOrEmpty(enumName))
            return enumName;

        // Add spaces before uppercase letters (except the first one)
        string spacedName = System.Text.RegularExpressions.Regex.Replace(enumName, "(\\B[A-Z])", " $1");

        // Make all letters lowercase
        return spacedName.ToLower();
    }

    /// <summary>
    /// Finds the dropdown index for a given enum integer value
    /// </summary>
    /// <param name="enumIntValue">Integer value of the enum</param>
    /// <returns>Index in the dropdown, or -1 if not found</returns>
    private int FindEnumIndex(int enumIntValue)
    {
        if (enumType == null)
        {
            DebugLogger.LogError(DebugLogCategory.Settings, $"EnumType is null when finding index for value {enumIntValue}", this);
            return -1;
        }

        var enumValues = enumType.GetValues();
        for (int i = 0; i < enumValues.Length; i++)
        {
            if (enumType.GetIntValue(enumValues[i]) == enumIntValue)
            {
                DebugLogger.Log(DebugLogCategory.Settings, $"Found enum value {enumIntValue} at index {i} ({enumValues[i]})", this);
                return i;
            }
        }

        DebugLogger.LogWarning(DebugLogCategory.Settings, $"Enum value {enumIntValue} not found in {enumType.TypeName} enum", this);
        return -1;
    }
    #endregion
}

/// <summary>
/// Wrapper class for enum types to enable Inspector selection
/// </summary>
[System.Serializable]
public class EnumType
{
    [SerializeField] private string typeName;
    private Type cachedType;
    private object[] cachedValues;

    public string TypeName
    {
        get => typeName;
        set
        {
            if (typeName != value)
            {
                typeName = value;
                cachedType = null;
                cachedValues = null;
            }
        }
    }

    /// <summary>
    /// Get System type
    /// </summary>
    /// <returns></returns>
    public Type GetSystemType()
    {
        if (cachedType == null && !string.IsNullOrEmpty(typeName))
        {
            cachedType = Type.GetType(typeName);
        }
        return cachedType;
    }

    /// <summary>
    /// Get values of enum
    /// </summary>
    /// <returns></returns>
    public object[] GetValues()
    {
        if (cachedValues == null)
        {
            var type = GetSystemType();
            if (type != null && type.IsEnum)
            {
                var values = Enum.GetValues(type);
                cachedValues = new object[values.Length];
                values.CopyTo(cachedValues, 0);
            }
        }
        return cachedValues ?? new object[0];
    }

    /// <summary>
    /// Get int value of enum
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public int GetIntValue(object enumValue)
    {
        return Convert.ToInt32(enumValue);
    }

    /// <summary>
    /// Check validity
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        var type = GetSystemType();
        return type != null && type.IsEnum;
    }
}