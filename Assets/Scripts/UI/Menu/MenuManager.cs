
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages navigation across the menu universe system
/// </summary>
public static class MenuManager 
{
    /// <summary>
    /// Go to menu navigation
    /// </summary>
    /// <param name="menuName">name of the menu enum</param>
    public static void GoToMenu(MenuEnum menuName)
    {
        switch (menuName)
        {
            case MenuEnum.MainMenu:

                // Go to main menu scene
                SceneManager.LoadScene("10_MainMenu");
                break;

            case MenuEnum.PlayerMenu:

                break;

            case MenuEnum.LevelMenu:

                // Go to level menu scene
                SceneManager.LoadScene("11_LevelMenu");
                break;

            case MenuEnum.PauseMenu:

                Object.Instantiate(Resources.Load("PauseMenu"));
                break;

             case MenuEnum.LevelFinishedMenu:

                Object.Instantiate(Resources.Load("LevelFinishedMenu"));
                break;

            case MenuEnum.HighScoreMenu:

                break;

            case MenuEnum.SettingsMenu:

                Object.Instantiate(Resources.Load("SettingsMenu"));
                break;
        }
    }


}
