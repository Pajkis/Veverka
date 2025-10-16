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
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;
    [SerializeField] private GridEvents gridEvents;
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private LevelSelection levelSelection;

    [Header("Level Management")]
    [SerializeField] private LevelSetManager levelSetManager;

    private bool levelBuilt = false;

    #endregion

    #region Methods

    /// <summary>
    /// Start is called before update - checks LevelAction and proceeds if Load
    /// </summary>
    private void Start()
    {
        // Only proceed if CurrentAction is Load
        if (levelSelection == null)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, "LevelSelection is not assigned!", this);
            return;
        }

        if (levelSelection.CurrentAction != LevelAction.Load)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"LevelLoader: CurrentAction is {levelSelection.CurrentAction}, skipping load", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"LevelLoader: CurrentAction is Load, proceeding with level loading", this);

        // Validate required components
        if (levelSetManager == null)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, "LevelSetManager is not assigned! Cannot load levels.", this);
            DebugLogger.LogError(DebugLogCategory.Gameplay, "Critical error - cannot start gameplay without LevelSetManager", this);
            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.GoToScene,
                Scene = SceneType.LevelSelect
            });
            return;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"LevelLoader initialized - Starting level {levelSelection.CurrentLevelIndex} from set {levelSelection.LevelSetType}", this);

        // play music
        DebugLogger.Log(DebugLogCategory.Audio, "Starting game music", this);
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlayMusic, Music = MusicType.Game });

        // invoke level reset
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Requesting level reset before loading", this);
        levelSelectEvent.Raise(new LevelSelectPayload
        {
            levelNumber = levelSelection.CurrentLevelIndex,
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
            DebugLogger.Log(DebugLogCategory.LevelSystem, "Grid build completed - level is ready", this);
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
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Starting level load coroutine for level {payload.levelNumber}", this);
            StartCoroutine(LoadLevelCoroutine());
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "Level reset requested - preparing for new level load", this);
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
        LevelSetType currentSet = levelSelection.LevelSetType;
        int currentLevelIndex = levelSelection.CurrentLevelIndex;

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Loading level {currentLevelIndex} from set {currentSet}", this);

        // Check if the level exists in the set
        if (!levelSetManager.HasLevel(currentSet, currentLevelIndex))
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Level {currentLevelIndex} not found in set {currentSet}!", this);
            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.GoToScene,
                Scene = SceneType.LevelSelect
            });
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, "Level exists in set - proceeding with CSV load", this);

        // Get the CSV data for the level
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Loading CSV data from level set", this);
        TextAsset levelCsv = levelSetManager.GetLevelCsv(currentSet, currentLevelIndex);
        if (levelCsv == null)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Failed to load CSV data for level {currentLevelIndex} in set {currentSet}!", this);
            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.GoToScene,
                Scene = SceneType.LevelSelect
            });
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, "CSV data loaded successfully - parsing grid", this);

        // Parse the grid from the CSV data
        TileType[,] grid = TileParser.LoadGridFromTextAsset(levelCsv);

        // raise event to build grid
        if (grid == null)
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, $"Grid parsing failed for level {currentLevelIndex} in set {currentSet}!", this);
            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.GoToScene,
                Scene = SceneType.LevelSelect
            });
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Grid parsed successfully - Size: {grid.GetLength(0)}x{grid.GetLength(1)}", this);

        // validate grid
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Validating grid configuration", this);
        if (!GridUtils.ValidateGrid(grid))
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, $"Invalid level configuration for level {currentLevelIndex} in set {currentSet}!", this);

            // Mark validation errors for display in LevelSelect scene
            LevelValidationErrorManager.MarkForDisplay();

            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.GoToScene,
                Scene = SceneType.LevelSelect
            });
            yield break;
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, "Grid validation passed - sending build request", this);

        // wait for the level to be built
        levelBuilt = false;
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Requesting grid build from GameGrid system", this);
        gridEvents.Raise(new GridEventPayload
        {
            EventType = GridEventType.BuildGrid,
            Grid = grid,
            BuildDone = false
        });
        DebugLogger.Log(DebugLogCategory.LevelSystem, "Waiting for grid build completion", this);
        yield return new WaitUntil(() => levelBuilt);

        //keep track of the time elapsed and ensure a minimum loading time
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < minLoadingTime)
        {
            float remainingTime = minLoadingTime - timeElapsed;
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Enforcing minimum loading time - waiting {remainingTime:F2}s more", this);
            yield return new WaitForSeconds(remainingTime);
        }

        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Level load complete - total time: {Time.time - startTime:F2}s", this);
        DebugLogger.Log(DebugLogCategory.SceneManager, "Transitioning to GamePlay scene", this);
        sceneNavigationEvents.Raise(new SceneNavigationEventPayload
        {
            EventType = SceneNavigationEventType.GoToScene,
            Scene = SceneType.GamePlay
        });
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
            DebugLogger.Log(DebugLogCategory.LevelSystem, "LevelSetManager not assigned!", this);
            return;
        }

        var allSets = levelSetManager.GetAllLevelSets();
        foreach (var levelSet in allSets)
        {
            if (levelSet != null)
            {
                DebugLogger.Log(DebugLogCategory.LevelSystem, $"Set: {levelSet.setType} ({levelSet.setDisplayName}) - {levelSet.LevelCount} levels", this);
            }
        }
    }

    /// <summary>
    /// Debug method to validate current level selection
    /// </summary>
    [ContextMenu("Debug - Validate Current Selection")]
    private void DebugValidateCurrentSelection()
    {
        if (levelSelection == null || levelSetManager == null)
        {
            DebugLogger.Log(DebugLogCategory.LevelSystem, "Required components not assigned!", this);
            return;
        }

        bool hasLevel = levelSetManager.HasLevel(levelSelection.LevelSetType, levelSelection.CurrentLevelIndex);
        DebugLogger.Log(DebugLogCategory.LevelSystem, $"Current selection - Set: {levelSelection.LevelSetType}, Level: {levelSelection.CurrentLevelIndex} - Valid: {hasLevel}", this);
    }
#endif

    #endregion
}