using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a UI component for configuring a game setting using a dropdown menu.
/// </summary>
/// <remarks>This class is responsible for initializing the dropdown menu with the current value of the associated
/// game setting and updating the game setting when the dropdown value changes. It requires a <see cref="Dropdown"/>
/// component and a <see cref="TextMeshProUGUI"/> component for the name label to be present as children of the
/// GameObject. Ensure that the <see cref="GameSettings.Instance"/> is properly initialized before using this
/// component.</remarks>
public class DropDownSettingItem : MonoBehaviour
{
    #region fields
    [SerializeField] private GameSettingsEnum settingType;
    [SerializeField] private TMP_Dropdown dropdown;       // doporučeno nastavit v Inspectoru
    [SerializeField] private TextMeshProUGUI nameLabel;
    #endregion

    private void Awake()
    {
        // Try to get components if not set in Inspector
        if (dropdown == null)
            dropdown = GetComponentInChildren<TMP_Dropdown>(true);

        // Try to find nameLabel if not set in Inspector
        if (nameLabel == null)
        {
            var labels = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in labels)
                if (t.name.Contains("NameLabel")) { nameLabel = t; break; }
        }

        // Validate found components
        if (dropdown == null)
        {
            Debug.LogError($"[{nameof(DropDownSettingItem)}] TMP_Dropdown not found on {name}.");
            enabled = false; return;
        }
        if (nameLabel == null)
        {
            Debug.LogWarning($"[{nameof(DropDownSettingItem)}] Name label not found on {name}.");
        }
    }

    /// <summary>
    /// Subscribes to the dropdown value change event and initializes the component's state from settings.
    /// </summary>
    /// <remarks>This method is called automatically when the component is enabled. It ensures that the
    /// dropdown's value change listener is registered and the component's state is initialized based on the current
    /// settings.</remarks>
    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnDropDownValueChanged);
        InitializeFromSettings();
    }

    /// <summary>
    /// Unsubscribes to the dropdown value change event
    /// </summary>
    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnDropDownValueChanged);
    }

    /// <summary>
    /// Initializes the dropdown value and name label from the game settings.
    /// </summary>
    private void InitializeFromSettings()
    {
        if (GameSettings.Instance == null) return;

        // Načti hodnotu
        int index = GameSettings.Instance.Get(settingType);       
        dropdown.SetValueWithoutNotify(index);

        // Nastav popisek
        if (nameLabel != null)
            nameLabel.text = SplitCamelCase(settingType.ToString());
    }

    /// <summary>
    /// Handles the event triggered when the dropdown value changes.
    /// </summary>
    /// <remarks>Updates the corresponding game setting with the specified value.  Ensure that <see
    /// cref="GameSettings.Instance"/> is not null before invoking this method.</remarks>
    /// <param name="value">The new value selected in the dropdown.</param>
    public void OnDropDownValueChanged(int value)
    {
        // Update GameSettings when the dropdown value changes
        Debug.Log($"[{nameof(DropDownSettingItem)}] {settingType} changed to {value}");

        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.Set(settingType, value);
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

}
