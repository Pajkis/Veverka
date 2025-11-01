using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Provides access to scene references used by the <see cref="SceneFlowManager"/>.
/// The references can be assigned in the inspector to avoid hard coded strings.
/// </summary>
public class SceneRefProvider : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the provider.
    /// </summary>
    public static SceneRefProvider Instance { get; private set; }
    
    [Header("Scene References")]
    [SerializeField] private AssetReference mainMenuScene;
    [SerializeField] private AssetReference levelSelectScene;
    [SerializeField] private AssetReference playerMenuScene;
    [SerializeField] private AssetReference highScoreMenuScene;
    [SerializeField] private AssetReference gamePlayScene;
    [SerializeField] private AssetReference levelTransitionScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Retrieves scene reference for given scene type.
    /// </summary>
    /// <param name="scene">Scene type</param>
    /// <returns>Asset reference of the scene, or null if not set.</returns>
    public AssetReference GetScene(SceneType scene)
    {
        return scene switch
        {
            SceneType.MainMenu => mainMenuScene,
            SceneType.LevelSelect => levelSelectScene,
            SceneType.PlayerMenu => playerMenuScene,
            SceneType.HighScoreMenu => highScoreMenuScene,
            SceneType.GamePlay => gamePlayScene,
            SceneType.LevelTransition => levelTransitionScene,
            _ => null,
        };
    }
}

