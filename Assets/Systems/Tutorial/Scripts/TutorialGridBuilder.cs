using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds tutorial grids using the shared GridSpawner helper.
/// Uses tutorial prefabs (with tutorial event channels) and creates isolated GridData instance.
/// No dependency on LevelInitData - tutorial grids don't affect main game state.
/// </summary>
public class TutorialGridBuilder : MonoBehaviour
{
    #region Prefabs
    [Header("Tutorial Tile Prefabs")]

    // Road prefabs
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject stoneRoadPrefab;
    [SerializeField] private GameObject stoneFilledHolePrefab;
    [SerializeField] private GameObject roadGoldenGoalScoredPrefab;

    // Character prefab (tutorial variant with TutorialDirectionEvent)
    [SerializeField] private GameObject veverkaPrefab;

    // Nut prefabs (tutorial variants with TutorialNutEvents)
    [SerializeField] private GameObject nutPrefab;
    [SerializeField] private GameObject nutStonePrefab;
    [SerializeField] private GameObject nutWaterPrefab;
    [SerializeField] private GameObject nutGoldenPrefab;

    // Obstacle prefabs
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallStonePrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private GameObject waterHolePrefab;
    [SerializeField] private GameObject goldenStatuePrefab;

    // Goal prefabs (tutorial variants with TutorialGoalEvents)
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject goldenGoalPrefab;

    [Header("Other Prefabs")]
    [SerializeField] private GameObject backgroundPrefab;
    #endregion

    #region References
    [Header("Grid Root")]
    [SerializeField] private Transform gridRoot;

    /// <summary>
    /// Public getter for grid root transform.
    /// </summary>
    public Transform GridRoot => gridRoot;
    #endregion

    #region Local State
    // Tutorial uses its own GridData instance, NOT the singleton
    private GridData tutorialGridData;
    private GameObject tutorialGridDataObject;
    private readonly List<GameObject> backgroundPool = new();
    #endregion

    /// <summary>
    /// Builds a tutorial grid from the provided grid data.
    /// Creates isolated GridData instance and uses GridSpawner helper with tutorial prefabs.
    /// Spawns after configurable delay to sync with overlay fade-in.
    /// </summary>
    public void BuildTutorialGrid(TileType[,] grid)
    {
        StartCoroutine(BuildTutorialGridDelayed(grid));
    }

    /// <summary>
    /// Delayed grid building coroutine - waits for overlay fade-in before spawning.
    /// </summary>
    private IEnumerator BuildTutorialGridDelayed(TileType[,] grid)
    {
        // Wait for overlay fade-in before spawning grid
        float delay = DisplaySettings.Instance != null
            ? DisplaySettings.Instance.ActiveProfile.tutorialGridShowDelay
            : 0.5f;

        DebugLogger.Log(DebugLogCategory.Tutorial, $"Waiting {delay}s for overlay fade-in before spawning grid", this);
        yield return new WaitForSeconds(delay);

        // Get grid dimensions
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        DebugLogger.Log(DebugLogCategory.Tutorial, $"Building tutorial grid - Size: {width}x{height}", this);

        // Create isolated tutorial GridData instance (MonoBehaviour requires GameObject)
        if (tutorialGridData == null)
        {
            tutorialGridDataObject = new GameObject("TutorialGridData");
            tutorialGridDataObject.transform.SetParent(transform);
            tutorialGridData = tutorialGridDataObject.AddComponent<GridData>();
        }

        tutorialGridData.InitializeGrids(width, height);

        // Copy grid data from GridUtils cache to tutorial GridData
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            { 
                Vector2Int pos = new(x, y);
                tutorialGridData.SetTileType(pos, grid[x, y]);
                tutorialGridData.SetNutType(pos, GridUtils.CachedNutGrid[x, y]);
                tutorialGridData.SetGoalType(pos, GridUtils.CachedGoalGrid[x, y]);
                tutorialGridData.SetObstacleType(pos, GridUtils.CachedObstacleGrid[x, y]);
                tutorialGridData.SetRoadType(pos, GridUtils.CachedRoadGrid[x, y]);
            }
        }

        // Create spawn configuration with tutorial prefabs
        var config = new GridSpawner.SpawnConfig
        {
            // Road prefabs
            emptyPrefab = this.emptyPrefab,
            roadPrefab = this.roadPrefab,
            stoneRoadPrefab = this.stoneRoadPrefab,
            stoneFilledHolePrefab = this.stoneFilledHolePrefab,
            roadGoldenGoalScoredPrefab = this.roadGoldenGoalScoredPrefab,

            // Character prefab (tutorial variant)
            veverkaPrefab = this.veverkaPrefab,

            // Nut prefabs (tutorial variants)
            nutPrefab = this.nutPrefab,
            nutStonePrefab = this.nutStonePrefab,
            nutWaterPrefab = this.nutWaterPrefab,
            nutGoldenPrefab = this.nutGoldenPrefab,

            // Obstacle prefabs
            wallPrefab = this.wallPrefab,
            wallStonePrefab = this.wallStonePrefab,
            holePrefab = this.holePrefab,
            waterHolePrefab = this.waterHolePrefab,
            goldenStatuePrefab = this.goldenStatuePrefab,

            // Goal prefabs (tutorial variants)
            goalPrefab = this.goalPrefab,
            goldenGoalPrefab = this.goldenGoalPrefab,

            // Other prefabs
            backgroundPrefab = this.backgroundPrefab,

            // Context - NO LevelInitData for tutorial (null = tutorial mode)
            gridRoot = this.gridRoot,
            levelInitData = null,  // Tutorial doesn't affect main game state
            backgroundPool = this.backgroundPool,
            surroundingObstacles = null  // Tutorial doesn't need surrounding obstacles
        };

        // Use shared GridSpawner helper to spawn grid
        DebugLogger.Log(DebugLogCategory.Tutorial, "Spawning tutorial grid using GridSpawner helper", this);
        GridSpawner.SpawnGrid(tutorialGridData, config);

        DebugLogger.Log(DebugLogCategory.Tutorial, "Tutorial grid build complete", this);
    }

    /// <summary>
    /// Clears the tutorial grid, destroying all non-pooled objects.
    /// </summary>
    public void ClearGrid()
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, "Clearing tutorial grid", this);

        // Use GridSpawner helper to clear non-pooled objects
        // No surroundingObstacles for tutorial (pass null)
        GridSpawner.ClearNonPooledObjects(gridRoot, backgroundPool, null);

        // Clear tutorial GridData
        tutorialGridData?.ClearGrids();

        DebugLogger.Log(DebugLogCategory.Tutorial, "Tutorial grid cleared", this);
    }

    /// <summary>
    /// Finds the tutorial character (Veverka) in the grid.
    /// Used by TutorialDirector to get reference for scripted movement.
    /// </summary>
    public CharVeverka FindTutorialCharacter()
    {
        CharVeverka character = gridRoot.GetComponentInChildren<CharVeverka>();

        if (character != null)
        {
            DebugLogger.Log(DebugLogCategory.Tutorial, $"Found tutorial character at position {character.GridPosition}", this);
        }
        else
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial, "Tutorial character (Veverka) not found in grid!", this);
        }

        return character;
    }

    /// <summary>
    /// Gets the tutorial GridData instance.
    /// Useful for debugging or advanced tutorial scenarios.
    /// </summary>
    public GridData GetTutorialGridData()
    {
        return tutorialGridData;
    }

    #region Unity Lifecycle
    private void Start()
    {
        InitializeGridRootPosition();
    }

    private void OnDestroy()
    {
        // Clean up tutorial GridData GameObject when component is destroyed
        if (tutorialGridDataObject != null)
        {
            Destroy(tutorialGridDataObject);
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Sets the grid root position from DisplayConfig.
    /// </summary>
    private void InitializeGridRootPosition()
    {
        if (gridRoot == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "GridRoot is not assigned!", this);
            return;
        }

        if (DisplaySettings.Instance == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.Tutorial,
                "DisplaySettings.Instance not available - using default grid root position", this);
            return;
        }

        Vector2 position = DisplaySettings.Instance.ActiveProfile.tutorialGridRootPosition;
        gridRoot.localPosition = new Vector3(position.x, position.y, 0f);

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"Tutorial grid root position set to: {position}", this);
    }
    #endregion
}
