using UnityEngine;

/// <summary>
/// Initializes the TurnControl system in a scene.
/// Attach this to a GameObject in your game scene to ensure TurnControl is available.
/// </summary>
public class TurnControlInitializer : MonoBehaviour
{
    [Header("Event References")]
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private NutEvents nutEvents;
    [SerializeField] private GoalEvents goalEvents;

    private void Awake()
    {
        // Check if TurnControl already exists
        if (TurnControl.Instance == null)
        {
            // Create TurnControl GameObject
            GameObject turnControlObj = new GameObject("TurnControl");
            TurnControl turnControl = turnControlObj.AddComponent<TurnControl>();
            
            // Set up event references
            turnControl.characterEvents = characterEvents;
            turnControl.nutEvents = nutEvents;
            turnControl.goalEvents = goalEvents;

            // Force TurnControl to refresh its event listeners since they were NULL during OnEnable
            turnControl.RefreshEventListeners();

            Debug.Log("[TurnControlInitializer] TurnControl system initialized");
        }
        else
        {
            Debug.Log("[TurnControlInitializer] TurnControl already exists");
        }
    }
}