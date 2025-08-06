using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages navigation across the menu universe system
/// </summary>
public static class SceneManager 
{
    /// <summary>
    /// Go to menu navigation
    /// </summary>
    /// <param name="sceneName">name of the menu enum</param>
    public static void GoToScene(SceneType sceneName)
    {
        switch (sceneName)
        {
            case SceneType.MainMenu:
                // Go to main menu scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("10_MainMenu");
                break;

            case SceneType.PlayerMenu:

                break;

            case SceneType.LevelMenu:

                // Go to level menu scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("11_LevelMenu");
                break;

            case SceneType.PauseMenu:

                Object.Instantiate(Resources.Load("PauseMenu"));
                break;

             case SceneType.LevelFinishedMenu:

                Object.Instantiate(Resources.Load("LevelFinishedMenu"));
                break;

            case SceneType.HighScoreMenu:

                break;

            case SceneType.SettingsMenu:

                Object.Instantiate(Resources.Load("SettingsMenu"));
                break;

            case SceneType.LoadLevel:

                UnityEngine.SceneManagement.SceneManager.LoadScene("20_LevelLoad");
                break;

            case SceneType.UnloadLevel:
                UnityEngine.SceneManagement.SceneManager.LoadScene("21_LevelUnload");
                break;
                    
            case SceneType.GamePlay:
                UnityEngine.SceneManagement.SceneManager.LoadScene("30_GamePlay");
                break;

            case SceneType.GameHelp:
                Object.Instantiate(Resources.Load("GameHelp"));
                break;
        }
    }


}
