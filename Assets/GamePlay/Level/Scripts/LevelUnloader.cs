using System.Collections;
using UnityEngine;

/// <summary>
/// Event-driven level unloading manager
/// Listens for scene navigation events and handles level cleanup
/// </summary>
public class LevelUnloader : MonoBehaviour
{
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private LevelDatabase levelDatabase;

    private bool isUnloading = false;

    /// <summary>
    /// Start is called before update - checks LevelAction and proceeds if Unload
    /// </summary>
    private void Start()
    {
        // Only proceed if CurrentAction is Unload
        if (levelDatabase == null)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, "LevelDatabase is not assigned!", this);
            return;
        }

        if (levelDatabase.CurrentAction != LevelAction.Unload)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"LevelUnloader: CurrentAction is {levelDatabase.CurrentAction}, skipping unload", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"LevelUnloader: CurrentAction is Unload, proceeding with level unloading", this);
        StartCoroutine(UnloadLevelCoroutine());
    }

    /// <summary>
    /// Unload level and clean up resources
    /// </summary>
    private IEnumerator UnloadLevelCoroutine()
    {
        isUnloading = true;

        DebugLogger.Log(DebugLogCategory.LevelSystem, "Starting level unload process", this);

        // Reset grid
        DebugLogger.Log(DebugLogCategory.GridSystem, "Resetting grid", this);
        gridEvents.Raise(new GridEventPayload { EventType = GridEventType.ResetGrid });

        // Switch to menu music
        DebugLogger.Log(DebugLogCategory.Audio, "Switching to menu music", this);
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlayMusic, Music = MusicType.Menu });

        yield return null; // Wait one frame for cleanup

        float minUnloadTime = 0.5f;
        float startTime = Time.time;

        // Wait for minimum unload time
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minUnloadTime)
        {
            yield return new WaitForSeconds(minUnloadTime - timeElapsed);
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, "Level unload complete - transitioning to LevelSelect", this);

        // Navigate to LevelSelect scene
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.LevelSelect
        });

        isUnloading = false;
    }
}
