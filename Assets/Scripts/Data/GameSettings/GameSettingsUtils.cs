using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Game settings utilities - wrapping PlayerPrefab funtions
/// </summary>
public static class GameSettingsUtils
{
/// <summary>
/// load values from register or initialize new ones
/// </summary>
/// <returns>dictionary with game settings</returns>
    public static Dictionary<GameSettingsEnum, int> LoadOrInitializeDefaults()
    {
        Dictionary<GameSettingsEnum, int> settings = new();

        //Load or set default values
        void Init(GameSettingsEnum key, int defaultValue)
        {
            if (!PlayerPrefs.HasKey(key.ToString()))
            {
                PlayerPrefs.SetInt(key.ToString(), defaultValue);
                settings[key] = defaultValue;
            }
            else
            {
                settings[key] = PlayerPrefs.GetInt(key.ToString());
            }
        }

        // Set each parameter
        Init(GameSettingsEnum.ActivePlayer, 0);
        Init(GameSettingsEnum.AnimationSpeed, 1);
        Init(GameSettingsEnum.MusicVolume, 5);
        Init(GameSettingsEnum.EffectVolume, 5);
        Init(GameSettingsEnum.MenuVolume, 5);

        PlayerPrefs.Save();

        return settings;
    }

    /// <summary>
    /// Set integer game settings properties
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void Set(GameSettingsEnum setting, int value)
    {
       PlayerPrefs.SetInt(setting.ToString(), value);
    }

    /// <summary>
    /// Set float game settings properties
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void Set(GameSettingsEnum setting, float value) =>
       PlayerPrefs.SetFloat(setting.ToString(), value);

    /// <summary>
    /// Set string game settings properties
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void Set(GameSettingsEnum setting, string value) =>
       PlayerPrefs.SetString(setting.ToString(), value);


    /// <summary>
    /// Get integer game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static int GetInt(GameSettingsEnum setting, int defaultValue = 0) =>
      PlayerPrefs.GetInt(setting.ToString(), defaultValue);

    /// <summary>
    /// Get float game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static float GetFloat(GameSettingsEnum setting, float defaultValue = 0f) =>
     PlayerPrefs.GetFloat(setting.ToString(), defaultValue);

    /// <summary>
    /// Get string game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static string GetString(GameSettingsEnum setting, string defaultValue = "") =>
       PlayerPrefs.GetString(setting.ToString(), defaultValue);

    /// <summary>
    /// get bool game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static bool GetBool(GameSettingsEnum setting, bool defaultValue = false) =>
       PlayerPrefs.GetInt(setting.ToString(), defaultValue ? 1 : 0) == 1;

    /// <summary>
    /// set integer game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void SetBool(GameSettingsEnum setting, bool value) =>
        PlayerPrefs.SetInt(setting.ToString(), value ? 1 : 0);      
}


