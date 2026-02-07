using UnityEngine;

/// <summary>
/// Centralized turn state - holds current speed multiplier and other turn-based parameters
/// </summary>
[CreateAssetMenu(fileName = "TurnState", menuName = "Gameplay/Runtime/TurnState")]
public class TurnState : ScriptableObject
{
    #region fields
    [SerializeField] float defaultSpeedMultiplier = 1f;
    float currentSpeedMultiplier;
    #endregion

    #region properties
    /// <summary>
    /// SpeedMultiplier for current turn
    /// </summary>
    public float CurrentSpeedMultiplier
        { get { return currentSpeedMultiplier; } 
        set { currentSpeedMultiplier = value; }}
    #endregion

    #region methods
    /// <summary>
    /// Reset speed multiplier
    /// </summary>
    public void ResetState()
    { 
        currentSpeedMultiplier = defaultSpeedMultiplier;
    }
    #endregion
}