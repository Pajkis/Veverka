using UnityEngine;

/// <summary>
/// Level selection and navigation data
/// Handles which level set and level is currently selected, and what action to perform
/// </summary>
[CreateAssetMenu(menuName = "Data/LevelSelection")]
public class LevelSelection : ScriptableObject
{
    [Header("Level Selection")]
    [Tooltip("Currently selected level set")]
    public LevelSetType LevelSetType;

    [Tooltip("Currently selected level index (0-based)")]
    public int CurrentLevelIndex;

    [Tooltip("Action to perform when entering LoadLevel scene")]
    public LevelAction CurrentAction;
}
