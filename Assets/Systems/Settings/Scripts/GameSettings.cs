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
        // Init singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adds listeners for setting data events.
    /// </summary>
    private void OnEnable()
    {
        settingEvents.AddListener(OnSettingEvent);
    }

    /// <summary>
    /// Removes listeners for setting data events.
    /// </summary>
    private void OnDisable()
    {
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
        if (config == null)
        {
            config = Resources.Load<GameSettingsConfig>("GameSettingsConfig");
            if (config == null)
            {
                Debug.LogError("GameSettingsConfig asset not found. Using default values.");
                config = ScriptableObject.CreateInstance<GameSettingsConfig>();
            }
        }

        //get init values for PlayerPrefab
        gameSettings =  GameSettingsUtils.LoadOrInitializeDefaults(config);

        // Test that values are set properly 
        foreach (GameSettingsEnum key in Enum.GetValues(typeof(GameSettingsEnum)))
        {
            Debug.Log($"Settings property {key}: value {gameSettings[key]}");
        }

        // Validation of animation speed
        if (gameSettings[GameSettingsEnum.AnimationSpeed] <= 0)
        {
            Debug.LogWarning("AnimationSpeed <= 0! Resetting to default.");
            gameSettings[GameSettingsEnum.AnimationSpeed] = config.GetDefault(GameSettingsEnum.AnimationSpeed);
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
        switch (payload.EventType)
        {
            case SettingsEventType.DataRequest:
                var key = payload.Setting;
                if (!gameSettings.TryGetValue(key, out int value))
                {
                    Debug.LogError($"[GameSettings] Missing key {key}, returning fallback from config.");
                    value = config != null ? config.GetDefault(key) : 0;
                }
                settingEvents.Raise(new SettingEventPayload
                {
                    EventType = SettingsEventType.DataBroadcast,
                    Setting = key,
                    Value = value
                });
                break;
            case SettingsEventType.DataBroadcast:
                int intValue = Mathf.RoundToInt(payload.Value);
                if (gameSettings.TryGetValue(payload.Setting, out int current) && current == intValue)
                    return;

                gameSettings[payload.Setting] = intValue;
                GameSettingsUtils.Set(payload.Setting, intValue);
                PlayerPrefs.Save();
                break;
        }
    }
    #endregion
}
