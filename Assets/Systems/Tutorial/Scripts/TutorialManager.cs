using UnityEngine;

/// <summary>
/// Manages persistent tutorial grid. Lives in Bootstrap scene with DontDestroyOnLoad.
/// Listens to TutorialEvents and coordinates tutorial loading/clearing.
/// Similar to GameGrid but for tutorials.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialEvents tutorialEvents;
    [SerializeField] private TutorialSetManager tutorialSetManager;
    [SerializeField] private GridBuilder gridBuilder;

    private void Awake()
    {
        // TutorialManager is part of overlay - no longer needs to be persistent
        // It will be destroyed when overlay is destroyed
        DebugLogger.Log(DebugLogCategory.Tutorial, "TutorialManager initialized", this);
    }

    private void OnEnable()
    {
        tutorialEvents?.AddListener(OnTutorialEvent);
    }

    private void OnDisable()
    {
        tutorialEvents?.RemoveListener(OnTutorialEvent);
    }

    /// <summary>
    /// Handle tutorial events from overlay or other systems
    /// </summary>
    private void OnTutorialEvent(TutorialEventPayload payload)
    {
        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"TutorialManager: Event received - Type={payload.EventType}, Set={payload.SetType}, Index={payload.TutorialIndex}", this);

        switch (payload.EventType)
        {
            case TutorialEventType.LoadTutorial:
                LoadTutorial(payload.SetType, payload.TutorialIndex);
                break;

            case TutorialEventType.ClearTutorial:
                ClearTutorial();
                break;

            case TutorialEventType.ReplayTutorial:
                ReplayTutorial(payload.SetType, payload.TutorialIndex);
                break;
        }
    }

    /// <summary>
    /// Load and display tutorial
    /// </summary>
    private void LoadTutorial(TutorialSetType setType, int tutorialIndex)
    {
        if (tutorialSetManager == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialSetManager not assigned!", this);
            return;
        }

        if (gridBuilder == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "GridBuilder not assigned!", this);
            return;
        }

        // Get CSV data
        TextAsset csvData = tutorialSetManager.GetTutorialCsv(setType, tutorialIndex);

        if (csvData == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial,
                $"Tutorial CSV not found: {setType}[{tutorialIndex}]", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial, "CSV loaded - parsing grid", this);

        // Parse grid
        TileType[,] grid = TileParser.LoadGridFromTextAsset(csvData);

        if (grid == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial,
                $"Failed to parse CSV for tutorial {setType}[{tutorialIndex}]", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"Grid parsed - Size: {grid.GetLength(0)}x{grid.GetLength(1)}", this);

        // Build tutorial grid (spawn animation handled by GameObjectAnimator on each prefab)
        gridBuilder.BuildLevel(grid);

        DebugLogger.Log(DebugLogCategory.Tutorial, "Tutorial load complete", this);
    }

    /// <summary>
    /// Clear tutorial grid - Don't destroy tiles here, they will be destroyed with overlay
    /// This just exits tutorial mode (camera, flags)
    /// </summary>
    private void ClearTutorial()
    {
        // Tiles are children of overlay and will be destroyed when overlay is destroyed
        // We only need to signal exit tutorial mode (handled by CameraFollow listener)
        DebugLogger.Log(DebugLogCategory.Tutorial, "Exiting tutorial mode - overlay will handle tile destruction", this);
    }

    /// <summary>
    /// Replay tutorial (clear + reload)
    /// </summary>
    private void ReplayTutorial(TutorialSetType setType, int tutorialIndex)
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, "Replaying tutorial", this);
        gridBuilder.ResetLevel();
        LoadTutorial(setType, tutorialIndex);
    }
}
