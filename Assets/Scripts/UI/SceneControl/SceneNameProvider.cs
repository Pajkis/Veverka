using UnityEngine;

/// <summary>
/// Provides access to scene names used by the <see cref="SceneFlowManager"/>.
/// The names can be assigned in the inspector to avoid hard coded strings.
/// </summary>
public class SceneNameProvider : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the provider.
    /// </summary>
    public static SceneNameProvider Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "10_MainMenu";
    [SerializeField] private string levelMenuScene = "11_LevelMenu";
    [SerializeField] private string playerMenuScene;
    [SerializeField] private string highScoreMenuScene;
    [SerializeField] private string gamePlayScene = "30_GamePlay";
    [SerializeField] private string loadLevelScene = "20_LevelLoad";
    [SerializeField] private string unloadLevelScene = "21_LevelUnload";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Retrieves scene name for given scene type.
    /// </summary>
    /// <param name="scene">Scene type</param>
    /// <returns>Name of the scene, or null if not set.</returns>
    public string GetSceneName(SceneType scene)
    {
        return scene switch
        {
            SceneType.MainMenu => mainMenuScene,
            SceneType.LevelMenu => levelMenuScene,
            SceneType.PlayerMenu => playerMenuScene,
            SceneType.HighScoreMenu => highScoreMenuScene,
            SceneType.GamePlay => gamePlayScene,
            SceneType.LoadLevel => loadLevelScene,
            SceneType.UnloadLevel => unloadLevelScene,
            _ => null,
        };
    }
}

