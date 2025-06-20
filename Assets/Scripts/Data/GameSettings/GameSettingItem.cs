using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Reflection.Emit;

/// <summary>
/// Game setting slider
/// </summary>
public class GameSettingItem : MonoBehaviour
{
    #region fields

    [SerializeField] private GameSettingsEnum settingType;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;

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

        // get textMesh components
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

        // Set initial value from GameSettings
        if (GameSettings.Instance != null)
        {            
            float value = GameSettings.Instance.Get(settingType);
            value = Mathf.Clamp(value, minValue, maxValue);
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = value;
            valueLabel.text = Mathf.RoundToInt(value).ToString();
            nameLabel.text = SplitCamelCase(settingType.ToString());
        }

        // Add listener for changes
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    /// <summary>
    /// Reacts on slider value change and save it
    /// </summary>
    /// <param name="value"></param>
    private void OnSliderChanged(float value)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.Set(settingType, Mathf.RoundToInt(value));
            
            //  OnSettingChanged?.Invoke(newValue); 
            // change volume in audio manager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UpdateVolume(settingType, Mathf.RoundToInt(value));
            }
        }

        // change text in settings menu
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

    #endregion
}
