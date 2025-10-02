using UnityEngine;

/// <summary>
/// Lightweight coordinator for grid system components.
/// Provides backwards compatible API and delegates to specialized components:
/// - GridData: Data storage and queries
/// - GridBuilder: Level building and resetting
/// - GridRuntime: Runtime updates during gameplay
/// Gameplay/Grid/Scripts/GameGrid
/// </summary>
public class GameGrid : MonoBehaviour
{
    #region Singleton
    public static GameGrid Instance { get; private set; }
    #endregion

    #region Component References
    [Header("Grid Components")]
    [SerializeField] private GridData gridData;
    [SerializeField] private GridBuilder gridBuilder;
    [SerializeField] private GridRuntime gridRuntime;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        // Singleton routine
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Backwards Compatible API - Data Queries
    /// <summary>
    /// Get tile type at position
    /// </summary>
    public TileType GetTileType(Vector2Int position)
    {
        return gridData.GetTileType(position);
    }

    /// <summary>
    /// Set tile type at position
    /// </summary>
    public void SetTileType(Vector2Int position, TileType tileType)
    {
        gridData.SetTileType(position, tileType);
    }

    /// <summary>
    /// Check if position is within grid bounds
    /// </summary>
    public bool IsInGrid(Vector2Int position)
    {
        return gridData.IsInGrid(position);
    }

    /// <summary>
    /// Check if tile at position can be walked on by a character
    /// </summary>
    public bool IsWalkableAt(Vector2Int position)
    {
        return gridData.IsWalkableAt(position);
    }

    /// <summary>
    /// Check if tile at position can be entered by a nut
    /// </summary>
    public bool IsPushableAt(Vector2Int position)
    {
        return gridData.IsPushableAt(position);
    }
    #endregion

    #region Direct Component Access (Optional)
    /// <summary>
    /// Direct access to GridData component
    /// </summary>
    public GridData Data => gridData;

    /// <summary>
    /// Direct access to GridBuilder component
    /// </summary>
    public GridBuilder Builder => gridBuilder;

    /// <summary>
    /// Direct access to GridRuntime component
    /// </summary>
    public GridRuntime Runtime => gridRuntime;
    #endregion
}
