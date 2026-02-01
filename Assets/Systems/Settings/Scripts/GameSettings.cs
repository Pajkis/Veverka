using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the configuration and management of game settings, providing a singleton instance to access and modify
/// settings values. This class ensures settings are initialized, persisted, and validated as needed.
/// </summary>
/// <remarks>The <see cref="GameSettings"/> class is implemented as a singleton, ensuring a single instance is
/// available throughout the application's lifecycle. It provides methods to retrieve and update game settings, with
/// support for default values and persistence. The settings are initialized from a configuration asset (<see
/// cref="GameSettingsConfig"/>) or default values if the asset is unavailable.  This class is designed to persist
/// across scene transitions and should be attached to a GameObject in the initial scene. It validates certain settings
/// (e.g., ensuring animation speed is positive) and logs warnings or errors for invalid or missing values.</remarks>
public class GameSettings: MonoBehaviour
{
    /// <summary>
    /// Game settings as singleton
    /// </summary>
    public static GameSettings Instance { get; private set; }

    #region fields
    Dictionary<GameSettingsEnum, int> gameSettings = new Dictionary<GameSettingsEnum, int>();

    #endregion

    #region events
    [Header("Configs and events")]
    [SerializeField] private GameSettingsConfig config;
    [SerializeField] private SettingEvents settingEvents;
    #endregion
      
    #region Init methods
    /// <summary>
    /// Establishes the singleton instance and keeps it alive across scenes.
    /// </summary>
    void Awake()
    {
        DebugLogger.Log(DebugLogCategory.Settings, "GameSettings Awake called", this);

        // Init singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DebugLogger.Log(DebugLogCategory.Settings, "GameSettings singleton instance created and marked as DontDestroyOnLoad", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, "GameSettings singleton already exists, destroying duplicate instance", this);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adds listeners for setting data events.
    /// </summary>
    private void OnEnable()
    {
        DebugLogger.Log(DebugLogCategory.Settings, "GameSettings OnEnable - adding setting event listener", this);
        settingEvents.AddListener(OnSettingEvent);
    }

    /// <summary>
    /// Removes listeners for setting data events.
    /// </summary>
    private void OnDisable()
    {
        DebugLogger.Log(DebugLogCategory.Settings, "GameSettings OnDisable - removing setting event listener", this);
        settingEvents.RemoveListener(OnSettingEvent);
    }


    /// <summary>
    /// Initializes the game settings by loading configuration values from a predefined asset or using default values if
    /// the asset is not found.
    /// </summary>
    /// <remarks>This method ensures that the game settings are properly initialized and validated. If the
    /// configuration asset  <c>GameSettingsConfig</c> is not found, a default configuration is created. The method also
    /// validates specific  settings, such as <see cref="GameSettingsEnum.AnimationSpeed"/>, and resets invalid values to
    /// their defaults.</remarks>
    public void InitGameSettings()
    {
        DebugLogger.Log(DebugLogCategory.Settings, "InitGameSettings called - starting settings initialization", this);

        if (config == null)
        {
            DebugLogger.Log(DebugLogCategory.Settings, "Config is null, attempting to load GameSettingsConfig from Resources", this);
            config = Resources.Load<GameSettingsConfig>("GameSettingsConfig");
            if (config == null)
            {
                DebugLogger.LogError(DebugLogCategory.Settings, "GameSettingsConfig asset not found in Resources. Creating default config instance", this);
                config = ScriptableObject.CreateInstance<GameSettingsConfig>();
            }
            else
            {
                DebugLogger.Log(DebugLogCategory.Settings, "GameSettingsConfig successfully loaded from Resources", this);
            }
        }

        //get init values for PlayerPrefs
        DebugLogger.Log(DebugLogCategory.Settings, "Loading or initializing default settings from config", this);
        gameSettings =  GameSettingsUtils.LoadOrInitializeDefaults(config);

        // Test that values are set properly
        DebugLogger.Log(DebugLogCategory.Settings, "Validating loaded settings values:", this);
        foreach (GameSettingsEnum key in Enum.GetValues(typeof(GameSettingsEnum)))
        {
            DebugLogger.Log(DebugLogCategory.Settings, $"Settings property {key}: value {gameSettings[key]}", this);
        }

        // Validation of animation speed
        if (gameSettings[GameSettingsEnum.AnimationSpeed] <= 0)
        {
            DebugLogger.LogWarning(DebugLogCategory.Settings, $"AnimationSpeed value {gameSettings[GameSettingsEnum.AnimationSpeed]} is invalid (<= 0), resetting to default", this);
            gameSettings[GameSettingsEnum.AnimationSpeed] = config.GetDefault(GameSettingsEnum.AnimationSpeed);
            DebugLogger.Log(DebugLogCategory.Settings, $"AnimationSpeed reset to default value: {gameSettings[GameSettingsEnum.AnimationSpeed]}", this);
        }

        DebugLogger.Log(DebugLogCategory.Settings, "InitGameSettings completed successfully", this);
    }
    #endregion

    #region public properties

    /// <summary>
    /// Gets the animation speed as a multiplier for gameplay.
    /// </summary>
    /// <returns>Animation speed multiplier (1.0x, 2.0x, 3.0x)</returns>
    public float AnimationSpeedMultiplier
    {
        get
        {
            if (!gameSettings.TryGetValue(GameSettingsEnum.AnimationSpeed, out int value))
            {
                DebugLogger.LogWarning(DebugLogCategory.Settings, "AnimationSpeed not found in gameSettings, returning default 1.0x", this);
                return 1.0f;
            }
            return (float)value;
        }
    }

    #endregion

    #region event handlers

    /// <summary>
    /// On setting data request event handler
    /// </summary>
    /// <param name="payload"></param>
    private void OnSettingEvent(SettingEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.Settings, $"OnSettingEvent received - EventType: {payload.EventType}, Setting: {payload.Setting}", this);

        switch (payload.EventType)
        {
            case SettingsEventType.DataRequest:
                var key = payload.Setting;
                DebugLogger.Log(DebugLogCategory.Settings, $"Processing DataRequest for setting: {key}", this);

                if (!gameSettings.TryGetValue(key, out int value))
                {
                    DebugLogger.LogError(DebugLogCategory.Settings, $"Missing key {key} in gameSettings dictionary, using fallback from config", this);
                    value = config != null ? config.GetDefault(key) : 0;
                    DebugLogger.Log(DebugLogCategory.Settings, $"Fallback value for {key}: {value}", this);
                }
                else
                {
                    DebugLogger.Log(DebugLogCategory.Settings, $"Found value for {key}: {value}", this);
                }

                settingEvents.Raise(new SettingEventPayload
                {
                    EventType = SettingsEventType.DataBroadcast,
                    Setting = key,
                    Value = value
                });
                DebugLogger.Log(DebugLogCategory.Settings, $"Raised DataBroadcast event for {key} with value {value}", this);
                break;

            case SettingsEventType.DataBroadcast:
                int intValue = Mathf.RoundToInt(payload.Value);
                DebugLogger.Log(DebugLogCategory.Settings, $"Processing DataBroadcast for {payload.Setting} with value {intValue}", this);

                if (gameSettings.TryGetValue(payload.Setting, out int current) && current == intValue)
                {
                    DebugLogger.Log(DebugLogCategory.Settings, $"Value for {payload.Setting} unchanged ({intValue}), skipping update", this);
                    return;
                }

                DebugLogger.Log(DebugLogCategory.Settings, $"Updating {payload.Setting} from {(gameSettings.ContainsKey(payload.Setting) ? gameSettings[payload.Setting] : "null")} to {intValue}", this);
                gameSettings[payload.Setting] = intValue;
                GameSettingsUtils.Set(payload.Setting, intValue);
                PlayerPrefs.Save();
                DebugLogger.Log(DebugLogCategory.Settings, $"Successfully saved {payload.Setting} = {intValue} to PlayerPrefs", this);
                break;
        }
    }
    #endregion
}
