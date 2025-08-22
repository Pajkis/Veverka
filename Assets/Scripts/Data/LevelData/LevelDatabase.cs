using UnityEngine;

/// <summary>
/// Level data 
/// </summary>
[CreateAssetMenu(menuName = "Data/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    [Header("Level game data")]
    public int CurrentLevelIndex;
    public int GoalsCount;

    [Header("Level grid data")]
    public Vector2Int gridSize;
    public Transform gridCenterStartTarget;
    public Transform gridOrigin;
}