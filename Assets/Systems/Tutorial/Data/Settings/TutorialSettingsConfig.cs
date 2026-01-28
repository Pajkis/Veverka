using System.Net.Http.Headers;
using UnityEngine;

/// <summary>
/// Scriptable object storing default values for game settings.
/// </summary>
[CreateAssetMenu(fileName = "TutorialSettingsConfig", menuName = "Configs/TutorialSettingsConfig")]
public class TutorialSettingsConfig : ScriptableObject
{
    #region fields
    [Header("Tutorial Settings")]

    [SerializeField] private bool autoplayEnable = true;
    [SerializeField] private TutorialAnimSpeedType animSpeedIndex = TutorialAnimSpeedType.Normal;
    [SerializeField] private TutorialAnimSpeedType textSpeedIndex = TutorialAnimSpeedType.Normal;

    #endregion

    #region Properties
    public bool AutoplayEnable => autoplayEnable;
    public TutorialAnimSpeedType AnimSpeedIndex => animSpeedIndex;
    public TutorialAnimSpeedType TextSpeedIndex => textSpeedIndex;
    #endregion

    #region Methods

    /// <summary>
    /// Loads tutorial settings from persistent storage using Unity's PlayerPrefs system.
    /// </summary>
    /// <remarks>This method retrieves all values associated with the <c>TutorialSettingsType</c> enumeration
    /// from PlayerPrefs and updates the corresponding in-memory settings. If a setting is not present in PlayerPrefs,
    /// the current in-memory value is retained.</remarks>
    public void TutorialLoadFromPlayerPrefs()
    {

        foreach (TutorialSettingsType settingType in System.Enum.GetValues(typeof(TutorialSettingsType)))
        {
            string PlayerPrefKey = settingType.ToString();
            switch (settingType)
            {
                case TutorialSettingsType.Tutorial_Autoplay:
                    autoplayEnable = PlayerPrefs.GetInt(PlayerPrefKey, autoplayEnable ? 1 : 0) == 1;
                    break;
                case TutorialSettingsType.Tutorial_AnimSpeed:
                    animSpeedIndex = (TutorialAnimSpeedType)PlayerPrefs.GetInt(PlayerPrefKey, (int)animSpeedIndex);
                    break;
                case TutorialSettingsType.Tutorial_TextSpeed:
                    textSpeedIndex = (TutorialAnimSpeedType)PlayerPrefs.GetInt(PlayerPrefKey, (int)textSpeedIndex);
                    break;
            }
        }
    }

    /// <summary>
    /// Saves a specific tutorial setting to persistent storage using Unity's PlayerPrefs system.
    /// </summary>
    /// <param name="settingtype"></param>
    /// <param name="value"></param>
    private void TutorialSaveToPlayerPrefs(TutorialSettingsType settingtype, int value)
    {
        PlayerPrefs.SetInt(settingtype.ToString(), value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Enables or disables the tutorial autoplay feature.
    /// </summary>
    /// <param name="value"><see langword="true"/> to enable autoplay for the tutorial; <see langword="false"/> to disable it.</param>
    public void TutorialSetAutoplay(bool value)
    {
        autoplayEnable = value;
        TutorialSaveToPlayerPrefs(TutorialSettingsType.Tutorial_Autoplay, value ? 1 : 0);
    }

    /// <summary>
    /// Sets the animation speed for the tutorial.
    /// </summary>
    /// <param name="value">The animation speed to apply, specified as a <see cref="TutorialAnimSpeedType"/> value.</param>
    public void TutorialSetAnimSpeed(TutorialAnimSpeedType value)
    {
        animSpeedIndex = value;
        TutorialSaveToPlayerPrefs(TutorialSettingsType.Tutorial_AnimSpeed, (int)value);
    }

    /// <summary>
    /// Sets the text display speed for tutorial messages.
    /// </summary>
    /// <param name="value">The speed setting to apply to tutorial text. Specifies how quickly tutorial messages are displayed.</param>
    public void TutorialSetTextSpeed(TutorialAnimSpeedType value)
    {
        textSpeedIndex = value;
        TutorialSaveToPlayerPrefs(TutorialSettingsType.Tutorial_TextSpeed, (int)value);
    }

    #endregion

    #region Static instance
    private static TutorialSettingsConfig instance;

    public static TutorialSettingsConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<TutorialSettingsConfig>("Tutorial/TutorialSettingsConfig");
                if (instance == null)
                {
                    Debug.LogError("TutorialSettingsConfig asset not found in Resources!");
                }
            }
            return instance;
        }
    }
    #endregion

}

