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
    [SerializeField] private SettingDataRequestEvent settingDataRequestEvent;
    [SerializeField] private SettingDataBroadcastEvent settingDataBroadcastEvent;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Try to get components if not set in Inspector
        if (dropdown == null)
            dropdown = GetComponentInChildren<TMP_Dropdown>(true);

        // Try to find nameLabel if not set in Inspector
        if (nameLabel == null)
        {
            var labels = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var label in labels)
            {
                if (label.name.Contains("NameLabel"))
                {
                    nameLabel = label;
                    break;
                }
            }
        }

        // Validate components
        if (dropdown == null)
        {
            Debug.LogError($"[{nameof(DropDownSettingItem)}] TMP_Dropdown not found on {name}.");
            enabled = false;
            return;
        }

        if (nameLabel == null)
        {
            Debug.LogWarning($"[{nameof(DropDownSettingItem)}] Name label not found on {name}.");
        }

        PopulateDropdown();
    }

    private void OnEnable()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(OnDropDownValueChanged);
            settingDataBroadcastEvent.AddListener(OnSettingData);
            settingDataRequestEvent.Raise(settingType);
        }
    }

    private void OnDisable()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
            settingDataBroadcastEvent.RemoveListener(OnSettingData);
        }
    }
    #endregion

    #region event handlers

    /// <summary>
    /// Gets setting data from broadcast event and updates dropdown and label
    /// </summary>
    /// <param name="payload"></param>
    private void OnSettingData(SettingDataPayload payload)
    {
        if (payload.Setting != settingType)
            return;

        int settingValue = Mathf.RoundToInt(payload.Value);
        int dropdownIndex = FindEnumIndex(settingValue);

        if (dropdownIndex >= 0 && dropdownIndex < dropdown.options.Count)
        {
            dropdown.SetValueWithoutNotify(dropdownIndex);
        }
        else
        {
            dropdown.SetValueWithoutNotify(0);
        }

        if (nameLabel != null)
        {
            nameLabel.text = FormatEnumName(settingType.ToString());
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
            return;

        dropdown.options.Clear();

        var enumValues = enumType.GetValues();
        foreach (var enumValue in enumValues)
        {
            string optionText = FormatEnumName(enumValue.ToString());
            dropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
        }

        dropdown.RefreshShownValue();
    }

    /// <summary>
    /// Initializes the dropdown value and name label from the game settings
    /// </summary>
    private void InitializeFromSettings()
    {
        if (enumType == null)
            return;

        // Set name label
        if (nameLabel != null)
        {
            nameLabel.text = FormatEnumName(settingType.ToString());
        }
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
            return;

        var enumValues = enumType.GetValues();
        if (dropdownIndex < 0 || dropdownIndex >= enumValues.Length)
            return;

        // Get the enum value at the selected index
        var selectedEnumValue = enumValues[dropdownIndex];
        int enumIntValue = enumType.GetIntValue(selectedEnumValue);

        Debug.Log($"[{nameof(DropDownSettingItem)}] {settingType} changed to {selectedEnumValue} (value: {enumIntValue})");

        // Update game settings       
        settingDataBroadcastEvent.Raise(new SettingDataPayload
        {
            Setting = settingType,
            Value = enumIntValue
        });
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
            return -1;

        var enumValues = enumType.GetValues();
        for (int i = 0; i < enumValues.Length; i++)
        {
            if (enumType.GetIntValue(enumValues[i]) == enumIntValue)
            {
                return i;
            }
        }

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

    public Type GetSystemType()
    {
        if (cachedType == null && !string.IsNullOrEmpty(typeName))
        {
            cachedType = Type.GetType(typeName);
        }
        return cachedType;
    }

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

    public int GetIntValue(object enumValue)
    {
        return Convert.ToInt32(enumValue);
    }

    public bool IsValid()
    {
        var type = GetSystemType();
        return type != null && type.IsEnum;
    }
}