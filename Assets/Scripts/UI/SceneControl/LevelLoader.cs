using System.Collections;
using UnityEngine;

/// <summary>
/// Load data from levelX csv file and validates then
/// </summary>
public class LevelLoader: MonoBehaviour
{

    #region fields

    // Serialized fields for events and level database
    [SerializeField]  private LevelSelectEvent levelSelectEvent;   
    [SerializeField]  private GoToSceneEvent goToSceneEvent;
    [SerializeField]  private BuildGridEvent buildGridEvent;
    [SerializeField]  private BuildGridDoneEvent buildGridDoneEvent;
    [SerializeField]  private PlayMusicEvent playMusicEvent;
    [SerializeField]  private LevelDatabase levelDatabase;

    private bool levelBuilt = false;
        
    #endregion


    #region Methods       

    /// <summary>
    /// Start is called before update
    /// </summary>
    private void Start()
    {
        //play music
        playMusicEvent.Raise(MusicType.Game);

        // invoke level reset
        levelSelectEvent.Raise(new LevelSelectPayload
        {
            levelNumber = levelDatabase.CurrentLevelIndex,
            resetRequested = true
        });
    }

    private void OnEnable()
    {
        // add listener
        levelSelectEvent.AddListener(LoadLevel);
        buildGridDoneEvent.AddListener(onLevelBuilt);
        
    }

    /// <summary>
    /// remove events on disable
    /// </summary>
    private void OnDisable()
    {
        levelSelectEvent.RemoveListener(LoadLevel);
        buildGridDoneEvent.RemoveListener(onLevelBuilt);
    }

    /// <summary>
    /// Get info that level built has finished
    /// </summary>
    /// <remarks>This method sets the internal state to indicate that the level build process has
    /// started.</remarks>
    /// <param name="payload">The data required to initialize the level.</param>
    private void onLevelBuilt()
    {
        levelBuilt = true;    
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

        // raise event to build grid
        if (grid == null)
        {
            Debug.LogError("Grid is null, level loading failed!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            yield break;
        }

        // validate grid
        if (!LevelUtils.ValidateGrid(grid))
        {
            Debug.LogWarning("Invalid level configuration!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            yield break;
        }

        // wait for the level to be built
        levelBuilt = false;
        buildGridEvent.Raise(grid);
        yield return new WaitUntil(() => levelBuilt);

        //keep track of the time elapsed and ensure a minimum loading time
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - timeElapsed);
        }

        goToSceneEvent.Raise(SceneType.GamePlay);
    }

    #endregion
}