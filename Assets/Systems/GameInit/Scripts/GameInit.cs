using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;
    [SerializeField] private UserLevelInitializer userLevelInitializer;

    /// <summary>
    /// Ensures the initializer persists across scene loads.
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Initializes game-wide systems and schedules loading of the main menu.
    /// </summary>
    void Start()
    {
        DebugLogger.Log(DebugLogCategory.SceneManager, "GameInit starting - initializing game systems", this);

        // call game settings init
        GameSettings.Instance.InitGameSettings();
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.Init });

        // Game start delay
        DebugLogger.Log(DebugLogCategory.SceneManager, "Starting game initialization delay", this);
        StartCoroutine(GameStartDelayCoroutine());
    }
    /// <summary>
    /// Game start delay coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator GameStartDelayCoroutine()
    {
        float minLoadingTime = 2f;
        float startTime = Time.time;

        // Initialize user levels if initializer is assigned
        if (userLevelInitializer != null)
        {
            DebugLogger.Log(DebugLogCategory.SceneManager, "Initializing user levels", this);
            yield return StartCoroutine(userLevelInitializer.InitializeUserLevels());
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.SceneManager, "UserLevelInitializer not assigned - skipping user level initialization", this);
        }

        // Ensure minimum loading time
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minLoadingTime)
        {
            float remainingTime = minLoadingTime - timeElapsed;
            DebugLogger.Log(DebugLogCategory.SceneManager, $"Enforcing minimum init time - waiting {remainingTime:F2}s more", this);
            yield return new WaitForSeconds(remainingTime);
        }

        DebugLogger.Log(DebugLogCategory.SceneManager, "Game initialization complete - navigating to MainMenu", this);
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.MainMenu
        });
    }


}
