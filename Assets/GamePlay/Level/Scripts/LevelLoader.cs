using System.Collections;
using UnityEngine;

/// <summary>
/// Load data from level sets based on LevelDatabase configuration
/// </summary>
public class LevelLoader : MonoBehaviour
{
    #region fields

    // Serialized fields for events and level database
    [SerializeField] private LevelSelectEvent levelSelectEvent;
    [SerializeField] private GoToSceneEvent goToSceneEvent;
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private LevelDatabase levelDatabase;

    [Header("Level Management")]
    [SerializeField] private LevelSetManager levelSetManager;

    private bool levelBuilt = false;

    #endregion

    #region Methods       

    /// <summary>
    /// Start is called before update
    /// </summary>
    private void Start()
    {
        // Validate required components
        if (levelSetManager == null)
        {
            Debug.LogError("LevelSetManager is not assigned! Cannot load levels.");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            return;
        }

        if (levelDatabase == null)
        {
            Debug.LogError("LevelDatabase is not assigned!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            return;
        }

        // play music
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlayMusic, Music = MusicType.Game });

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
        gridEvents.AddListener(OnGridEvent);
    }

    /// <summary>
    /// remove events on disable
    /// </summary>
    private void OnDisable()
    {
        levelSelectEvent.RemoveListener(LoadLevel);
        gridEvents.RemoveListener(OnGridEvent);
    }

    /// <summary>
    /// Handle grid events.
    /// </summary>
    private void OnGridEvent(GridEventPayload payload)
    {
        if (payload.EventType == GridEventType.BuildGrid && payload.BuildDone)
        {
            levelBuilt = true;
        }
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
    /// Load and validate level from the configured level set
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadLevelCoroutine()
    {
        yield return null; // wait one frame so the scene is fully loaded

        float minLoadingTime = 2f;
        float startTime = Time.time;

        // Get level data from the level set system
        LevelSetType currentSet = levelDatabase.LevelSetType;
        int currentLevelIndex = levelDatabase.CurrentLevelIndex;

        Debug.Log($"Loading level {currentLevelIndex} from set {currentSet}");

        // Check if the level exists in the set
        if (!levelSetManager.HasLevel(currentSet, currentLevelIndex))
        {
            Debug.LogError($"Level {currentLevelIndex} not found in set {currentSet}!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            yield break;
        }

        // Get the CSV data for the level
        TextAsset levelCsv = levelSetManager.GetLevelCsv(currentSet, currentLevelIndex);
        if (levelCsv == null)
        {
            Debug.LogError($"Failed to load CSV data for level {currentLevelIndex} in set {currentSet}!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            yield break;
        }

        // Parse the grid from the CSV data
        TileType[,] grid = GridUtils.LoadGridFromTextAsset(levelCsv);

        // raise event to build grid
        if (grid == null)
        {
            Debug.LogError($"Grid parsing failed for level {currentLevelIndex} in set {currentSet}!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            yield break;
        }

        // validate grid
        if (!GridUtils.ValidateGrid(grid))
        {
            Debug.LogWarning($"Invalid level configuration for level {currentLevelIndex} in set {currentSet}!");
            goToSceneEvent.Raise(SceneType.LevelMenu);
            yield break;
        }

        // wait for the level to be built
        levelBuilt = false;
        gridEvents.Raise(new GridEventPayload
        {
            EventType = GridEventType.BuildGrid,
            Grid = grid,
            BuildDone = false
        });
        yield return new WaitUntil(() => levelBuilt);

        //keep track of the time elapsed and ensure a minimum loading time
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minLoadingTime)
        {
            yield return new WaitForSeconds(minLoadingTime - timeElapsed);
        }

        Debug.Log($"Successfully loaded level {currentLevelIndex} from set {currentSet}");
        goToSceneEvent.Raise(SceneType.GamePlay);
    }

    #endregion

    #region Debug Methods (Editor only)

#if UNITY_EDITOR
    /// <summary>
    /// Debug method to list all available levels in all sets
    /// </summary>
    [ContextMenu("Debug - List All Available Levels")]
    private void DebugListAllLevels()
    {
        if (levelSetManager == null)
        {
            Debug.Log("LevelSetManager not assigned!");
            return;
        }

        var allSets = levelSetManager.GetAllLevelSets();
        foreach (var levelSet in allSets)
        {
            if (levelSet != null)
            {
                Debug.Log($"Set: {levelSet.setType} ({levelSet.setDisplayName}) - {levelSet.LevelCount} levels");
            }
        }
    }

    /// <summary>
    /// Debug method to validate current level selection
    /// </summary>
    [ContextMenu("Debug - Validate Current Selection")]
    private void DebugValidateCurrentSelection()
    {
        if (levelDatabase == null || levelSetManager == null)
        {
            Debug.Log("Required components not assigned!");
            return;
        }

        bool hasLevel = levelSetManager.HasLevel(levelDatabase.LevelSetType, levelDatabase.CurrentLevelIndex);
        Debug.Log($"Current selection - Set: {levelDatabase.LevelSetType}, Level: {levelDatabase.CurrentLevelIndex} - Valid: {hasLevel}");
    }
#endif

    #endregion
}