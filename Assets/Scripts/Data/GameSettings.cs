using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

public class GameSettings: MonoBehaviour
{
    /// <summary>
    /// Game settings as singleton
    /// </summary>
    public static GameSettings Instance { get; private set; }

    #region fields
    Dictionary<GameSettingsEnum, int> gameSettings = new Dictionary<GameSettingsEnum, int>();
    #endregion


    #region propeties

    /// <summary>
    /// Get a value
    /// </summary>
    /// <param name="key">GameSettings enum name</param>
    /// <returns>value of settings property</returns>
    public int Get(GameSettingsEnum key) => gameSettings[key];

    #endregion

    #region Init methods

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
        //get init values for PlayerPrefab
        gameSettings =  GameSettingsUtils.LoadOrInitializeDefaults();

        // Test that values are set properly 
        foreach (GameSettingsEnum key in Enum.GetValues(typeof(GameSettingsEnum)))
        {
            Debug.Log($"Settings property {key}: value {gameSettings[key]}");
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

        /// use switch to clamp or add logic for setting each parameters!
    }
    #endregion
}
