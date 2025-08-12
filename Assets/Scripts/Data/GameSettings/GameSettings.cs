using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings: MonoBehaviour
{
    /// <summary>
    /// Game settings as singleton
    /// </summary>
    public static GameSettings Instance { get; private set; }

    #region fields
    Dictionary<GameSettingsEnum, int> gameSettings = new Dictionary<GameSettingsEnum, int>();
    [SerializeField] private GameSettingsConfig config;
    #endregion

    #region properties

    /// <summary>
    /// Get a value
    /// </summary>
    /// <param name="key">GameSettings enum name</param>
    /// <returns>value of settings property</returns>
    public int Get(GameSettingsEnum key)
    {
        if (!gameSettings.ContainsKey(key))
        {
            Debug.LogError($"[GameSettings] Missing key {key}, returning fallback from config.");
            return config != null ? config.GetDefault(key) : 0;
        }
        return gameSettings[key];
    }

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
    /// InitGame Settings
    /// </summary>
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

    #region methods
    /// <summary>
    /// Set a value of settings property
    /// </summary>
    /// <param name="key">key of the property</param>
    /// <param name="value">value to be set</param>
    public void Set(GameSettingsEnum key, int value)
    {
        gameSettings[key] = value;
        GameSettingsUtils.Set(key, value); // persist
        PlayerPrefs.Save();      
    }

    #endregion
}
