using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Game setting slider
/// </summary>
public class GameSettingSlider : MonoBehaviour
{
    #region fields
    [SerializeField] private GameSettingsEnum settingType;
    private Slider slider;
    #endregion

    #region method
    /// <summary>
    /// Start method
    /// </summary>
    private void Start()
    {
        slider = GetComponent<Slider>();

        // Set initial value from GameSettings
        if (GameSettings.Instance != null && slider != null)
        {
            slider.value = GameSettings.Instance.Get(settingType);
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
            GameSettings.Instance.Set(settingType, (int)value);
            //  OnSettingChanged?.Invoke(newValue); 
        }
    }
    #endregion
}
