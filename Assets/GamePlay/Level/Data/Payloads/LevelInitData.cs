using UnityEngine;

/// <summary>
/// Level initialization data
/// Contains data needed to initialize and setup a level (goals, grid, camera)
/// </summary>
[CreateAssetMenu(menuName = "Data/LevelInitData")]
public class LevelInitData : ScriptableObject
{
    [Header("Level Game Data")]
    [Tooltip("Initial number of goals at level start")]
    public int StartGoalsCount;

    [Header("Level Grid Data")]
    [Tooltip("Size of the grid (width x height)")]
    public Vector2Int gridSize;

    [Tooltip("Target position for camera center at level start")]
    public Transform gridCenterStartTarget;

    [Tooltip("Origin point of the grid")]
    public Transform gridOrigin;
}
