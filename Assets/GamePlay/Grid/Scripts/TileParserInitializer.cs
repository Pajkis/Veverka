using UnityEngine;

/// <summary>
/// Initializer for TileParser system - should be placed in the first scene (Bootstrap/Init)
/// Ensures TileParser is initialized with config before any level loading
/// Add this component to a GameObject in your Bootstrap/Init scene and assign the config
/// </summary>
public class TileParserInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private TileParsingConfig tileParsingConfig;

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// Uses Awake to ensure early initialization
    /// </summary>
    private void Awake()
    {
        // Make this object persist across scenes
        DontDestroyOnLoad(gameObject);

        // Initialize TileParser with the config
        if (tileParsingConfig != null)
        {
            TileParser.SetConfig(tileParsingConfig);
            Debug.Log($"[TileParser] Initialized with config: {tileParsingConfig.name}");
        }
        else
        {
            Debug.LogWarning("[TileParser] No TileParsingConfig assigned - using fallback tile parsing. Please assign config in Inspector.", this);
        }
    }
}
