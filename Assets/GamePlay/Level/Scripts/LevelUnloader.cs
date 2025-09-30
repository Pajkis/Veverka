using System.Collections;
using UnityEngine;

/// <summary>
/// Level unloading manager
/// </summary>
public class LevelUnloader : MonoBehaviour
{
    [SerializeField] private GridEvents gridEvents;

    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;

    [SerializeField] private AudioEvents audioEvents;

    void Start()
    {
        // raise reset event
        gridEvents.Raise(new GridEventPayload { EventType = GridEventType.ResetGrid });
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlayMusic, Music = MusicType.Menu });
        StartCoroutine(UnloadLevelCoroutine());
    }

    /// <summary>
    /// Load and validate level from file
    /// </summary>
    /// <returns></returns>
    IEnumerator UnloadLevelCoroutine()
    {
        yield return null; // wait one frame so the scene is fully loaded

        float minLoadingTime = 2f;
        float startTime = Time.time;

        // Wait for loading to pass
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minLoadingTime)
            yield return new WaitForSeconds(minLoadingTime - timeElapsed);

        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.MainMenu
        });
     }


}
