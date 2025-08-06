using System.Collections;
using UnityEngine;

/// <summary>
/// Load data from levelX csv file and validates then
/// </summary>
public class LevelLoader: MonoBehaviour
{

    #region fields

    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;
    #endregion


    #region Methods       

    /// <summary>
    /// Start is called before update
    /// </summary>
    private void Start()
    {
        // add listener
        levelSelectEvent.AddListener(LoadLevel);

        // invoke level reset 
        levelSelectEvent.Raise(new LevelSelectPayload
        {
            levelNumber = levelDatabase.CurrentLevelIndex,
            resetRequested = true
        });
    }

    /// <summary>
    /// remove events on disable
    /// </summary>
    private void OnDisable()
    {
        levelSelectEvent.RemoveListener(LoadLevel);
    }

    /// <summary>
    /// load level after reset
    /// </summary>
    /// <param name="payload"></param>
    private void LoadLevel(LevelSelectPayload payload)
    {
        if (!payload.resetRequested)
        {
            StartCoroutine(LoadLevelCoroutine());
        }
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
        string levelName = ((LevelEnum)levelDatabase.CurrentLevelIndex).ToString();
        TileType[,] grid = LevelUtils.LoadGridFromCsv(levelName);

        if (grid != null)
        {
            //level Grid Validation
            bool success = LevelUtils.ValidateGrid(grid);
            if (!success)
            {
                //level data not valid
                Debug.LogWarning("Invalid level configuration!");
                SceneManager.GoToScene(SceneType.LevelMenu);
                        
                yield break;
            }
            else
            {
                // Wait for loading to pass
                float timeElapsed = Time.time - startTime;
                if (timeElapsed < minLoadingTime)
                    yield return new WaitForSeconds(minLoadingTime - timeElapsed);

                //enter game scene              
                SceneManager.GoToScene(SceneType.GamePlay);
            }
        }
        // Grid is not loaded properly
        else
        {
            SceneManager.GoToScene(SceneType.LevelMenu);
            yield break;
        }
    }

    #endregion
}