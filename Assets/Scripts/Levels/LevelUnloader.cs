using System.Collections;
using UnityEngine;

public class LevelUnloader : MonoBehaviour
{
    [SerializeField]
    private ResetGridEvent resetGridEvent;


    void Start()
    {
        // raise reset event
        resetGridEvent.Raise();
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

        SceneManager.GoToMenu(SceneType.MainMenu);      
     }


}
