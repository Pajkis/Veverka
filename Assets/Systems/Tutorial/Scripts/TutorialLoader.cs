using UnityEngine;

/// <summary>
/// Loads tutorials based on current selection from TutorialSelectData.
/// Similar to LevelLoader but simpler - just loads tutorial grid directly without events.
/// </summary>
public class TutorialLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialSelectData tutorialSelection;
    [SerializeField] private TutorialSetManager tutorialSetManager;
    [SerializeField] private TutorialGridBuilder gridBuilder;

    /// <summary>
    /// Load tutorial when overlay starts
    /// </summary>
    private void Start()
    {
        LoadCurrentTutorial();
    }

    /// <summary>
    /// Load the currently selected tutorial from TutorialSelectData
    /// </summary>
    private void LoadCurrentTutorial()
    {
        if (tutorialSelection == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialSelection is not assigned!", this);
            return;
        }

        if (tutorialSetManager == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialSetManager is not assigned!", this);
            return;
        }

        if (gridBuilder == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialGridBuilder is not assigned!", this);
            return;
        }

        // 1. Read current selection
        TutorialSetType setType = tutorialSelection.TutorialSetType;
        int tutorialIndex = tutorialSelection.TutorialIndex;

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"TutorialLoader: Reading selection - Set={setType}, Index={tutorialIndex}", this);

        // 2. Get CSV data from tutorial set manager
        TextAsset csvData = tutorialSetManager.GetTutorialCsv(setType, tutorialIndex);

        if (csvData == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial,
                $"Tutorial CSV not found: {setType}[{tutorialIndex}]", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial, "CSV data loaded successfully - parsing grid", this);

        // 3. Parse grid from CSV
        TileType[,] grid = TileParser.LoadGridFromTextAsset(csvData);

        if (grid == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial,
                $"Failed to parse CSV for tutorial {setType}[{tutorialIndex}]", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"Grid parsed successfully - Size: {grid.GetLength(0)}x{grid.GetLength(1)}", this);

        // 4. Build tutorial grid (simple direct call, no events needed)
        gridBuilder.BuildTutorialGrid(grid);

        DebugLogger.Log(DebugLogCategory.Tutorial, "Tutorial load complete", this);
    }

    /// <summary>
    /// Replay button handler - clears grid and rebuilds tutorial
    /// </summary>
    public void OnReplayButtonClick()
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, "Replay button clicked - reloading tutorial", this);

        gridBuilder.ClearGrid();
        LoadCurrentTutorial();
    }
}
