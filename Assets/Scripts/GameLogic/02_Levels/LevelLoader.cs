using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load data from levelX csv file and validates then
/// </summary>
public class LevelLoader: MonoBehaviour
{
    #region Methods       
    void Start()
    {       
        StartCoroutine(LoadLevelCoroutine());
    }

    /// <summary>
    /// Load and validate level from file
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadLevelCoroutine()
    {
        yield return null; // wait one frame so the scene is fully loaded

        float minLoadingTime = 2f;
        float startTime = Time.time;


        // selected level to grid   
        string levelName = ((LevelEnum)LevelUtils.SelectedLevel).ToString();
        TileType[,] grid = LevelUtils.LoadGridFromCsv(levelName);

        if (grid != null)
        {
            //level Grid Validation
            bool success = LevelUtils.ValidateGrid(grid);
            if (!success)
            {
                //level data not valid
                Debug.Log("Invalid level configuration!");
                SceneManager.LoadScene("LevelMenu");                
                yield break;
            }
            else
            {
                // Wait for loading to pass
                float timeElapsed = Time.time - startTime;
                if (timeElapsed < minLoadingTime)
                    yield return new WaitForSeconds(minLoadingTime - timeElapsed);

                //enter game scene
                SceneManager.LoadScene("GameScene");
            }
        }
        // Grid is not loaded properly
        else
        {
            SceneManager.LoadScene("LevelMenu");
            yield break;
        }
    }

    #endregion
}