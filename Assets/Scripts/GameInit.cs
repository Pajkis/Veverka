using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    GameSettings gameSettings;
    // Start is called before the first frame update
    void Awake()
    {
        EventManager.InitEvents();
        LevelUtils.Initialize();
        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
    // call game settings init
     gameSettings = GetComponent<GameSettings>();
     if (gameSettings != null)
      {
        gameSettings.InitGameSettings();
      }
        
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
        SceneManager.LoadScene("MainMenu");
    }


}
