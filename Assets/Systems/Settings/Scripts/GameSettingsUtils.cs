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
    public static Dictionary<GameSettingsEnum, int> LoadOrInitializeDefaults(GameSettingsConfig config)
    {
        DebugLogger.Log(DebugLogCategory.Settings, "LoadOrInitializeDefaults called");
        Dictionary<GameSettingsEnum, int> settings = new();

        // Load or set default values
        void Init(GameSettingsEnum key)
        {
            int defaultValue = config.GetDefault(key);
            if (!PlayerPrefs.HasKey(key.ToString()))
            {
                DebugLogger.Log(DebugLogCategory.Settings, $"Key {key} not found in PlayerPrefs, initializing with default value: {defaultValue}");
                PlayerPrefs.SetInt(key.ToString(), defaultValue);
                settings[key] = defaultValue;
            }
            else
            {
                int savedValue = PlayerPrefs.GetInt(key.ToString());
                DebugLogger.Log(DebugLogCategory.Settings, $"Key {key} loaded from PlayerPrefs with value: {savedValue}");
                settings[key] = savedValue;
            }
        }

        DebugLogger.Log(DebugLogCategory.Settings, "Initializing all settings from enum values");
        foreach (GameSettingsEnum key in System.Enum.GetValues(typeof(GameSettingsEnum)))
        {
            Init(key);
        }

        DebugLogger.Log(DebugLogCategory.Settings, "Saving PlayerPrefs after initialization");
        PlayerPrefs.Save();
        DebugLogger.Log(DebugLogCategory.Settings, $"LoadOrInitializeDefaults completed, {settings.Count} settings loaded");

        return settings;
    }

    /// <summary>
    /// Set integer game settings properties
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void Set(GameSettingsEnum setting, int value)
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"Setting {setting} to int value: {value}");
        PlayerPrefs.SetInt(setting.ToString(), value);
    }

    /// <summary>
    /// Set float game settings properties
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void Set(GameSettingsEnum setting, float value)
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"Setting {setting} to float value: {value}");
        PlayerPrefs.SetFloat(setting.ToString(), value);
    }

    /// <summary>
    /// Set string game settings properties
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void Set(GameSettingsEnum setting, string value)
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"Setting {setting} to string value: {value}");
        PlayerPrefs.SetString(setting.ToString(), value);
    }


    /// <summary>
    /// Get integer game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static int GetInt(GameSettingsEnum setting, int defaultValue = 0)
    {
        int value = PlayerPrefs.GetInt(setting.ToString(), defaultValue);
        DebugLogger.Log(DebugLogCategory.Settings, $"Getting int value for {setting}: {value} (default: {defaultValue})");
        return value;
    }

    /// <summary>
    /// Get float game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static float GetFloat(GameSettingsEnum setting, float defaultValue = 0f)
    {
        float value = PlayerPrefs.GetFloat(setting.ToString(), defaultValue);
        DebugLogger.Log(DebugLogCategory.Settings, $"Getting float value for {setting}: {value} (default: {defaultValue})");
        return value;
    }

    /// <summary>
    /// Get string game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static string GetString(GameSettingsEnum setting, string defaultValue = "")
    {
        string value = PlayerPrefs.GetString(setting.ToString(), defaultValue);
        DebugLogger.Log(DebugLogCategory.Settings, $"Getting string value for {setting}: '{value}' (default: '{defaultValue}')");
        return value;
    }

    /// <summary>
    /// get bool game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="defaultValue">settings value</param>
    public static bool GetBool(GameSettingsEnum setting, bool defaultValue = false)
    {
        int intValue = PlayerPrefs.GetInt(setting.ToString(), defaultValue ? 1 : 0);
        bool value = intValue == 1;
        DebugLogger.Log(DebugLogCategory.Settings, $"Getting bool value for {setting}: {value} (PlayerPrefs int: {intValue}, default: {defaultValue})");
        return value;
    }

    /// <summary>
    /// set integer game settings property value
    /// </summary>
    /// <param name="setting">settings property name</param>
    /// <param name="value">settings value</param>
    public static void SetBool(GameSettingsEnum setting, bool value)
    {
        int intValue = value ? 1 : 0;
        DebugLogger.Log(DebugLogCategory.Settings, $"Setting bool {setting} to: {value} (PlayerPrefs int: {intValue})");
        PlayerPrefs.SetInt(setting.ToString(), intValue);
    }      
}


