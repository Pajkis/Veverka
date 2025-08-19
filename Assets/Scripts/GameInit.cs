using System.Collections;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private AudioInitEvent audioInitEvent;

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
        // call game settings init
        GameSettings.Instance.InitGameSettings();
        audioInitEvent.Raise();

        // Game start delay
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

        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minLoadingTime)
            yield return new WaitForSeconds(minLoadingTime - timeElapsed);
        SceneFlowManager.GoToScene(SceneType.MainMenu);
    }


}
