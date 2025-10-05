using UnityEngine;

/// <summary>
/// Action to perform when entering LoadLevel scene
/// </summary>
public enum LevelAction
{
    None,
    Load,
    Unload
}

/// <summary>
/// Level data
/// </summary>
[CreateAssetMenu(menuName = "Data/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    [Header("Level selection")]
    public LevelSetType LevelSetType;
    public int CurrentLevelIndex;
    public LevelAction CurrentAction;

    [Header("Level game data")]
    public int GoalsCount;

    [Header("Level grid data")]
    public Vector2Int gridSize;
    public Transform gridCenterStartTarget;
    public Transform gridOrigin;
}