using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Awake()
    {                   
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // call game settings init
        GameSettings.Instance.InitGameSettings();
        AudioManager.Instance.Initialize();

        //Game start Delay
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("10_MainMenu");
    }


}
