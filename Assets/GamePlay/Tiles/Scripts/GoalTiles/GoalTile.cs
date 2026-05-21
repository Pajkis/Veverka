using System.Collections;
using UnityEngine;

/// <summary>
/// goal tile abstract class
/// </summary>
public abstract class GoalTile : TileObject
{
    protected GoalType goalType;
    protected bool shouldRemoveGoal = false;

    #region Popout Configuration
    [Header("Popout Animation Settings")]
    [Tooltip("Offset from grid position (in world units)")]
    [SerializeField] protected Vector2 popoutPositionOffset = Vector2.zero;

    [Tooltip("Delay before popout animation starts (in seconds)")]
    [SerializeField] protected float popoutStartDelay = 0f;

    [Tooltip("Scale multiplier for the popout image")]
    [SerializeField] protected float popoutScale = 1.0f;

    [Tooltip("Tint color for the popout image")]
    [SerializeField] protected Color popoutTintColor = Color.white;
    #endregion

    #region events
    [SerializeField] protected NutEvents nutEvents;
    [SerializeField] protected GoalEvents goalEvents;
    [SerializeField] protected ObstacleEvents obstacleEvents;
    [SerializeField] protected RoadEvents roadEvents; 
    #endregion
   
    #region Init
    /// <summary>
    /// Init goals and register them in GameGrid
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    /// <param name="goalType"></param>
    public virtual void Init(TileType tileType, Vector2Int gridPosition, GoalType goalType)
    {
        base.Init(tileType, gridPosition);
        this.goalType = goalType;

        DebugLogger.Log(DebugLogCategory.GoalSystem, $"Initialized {goalType} goal at position: {gridPosition}", this);
    }

    /// <summary>
    /// On enable add listener
    /// </summary>
    protected void OnEnable()
    {
        nutEvents.AddListener(OnNutEvent);        
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    protected void OnDisable()
    {
        nutEvents.RemoveListener(OnNutEvent);
    }
    #endregion

    #region methods
    /// <summary>
    /// Recaction on nut reaching the goal
    /// </summary>
    /// <param name="payload"></param>
    private void OnNutEvent(NutEventPayload payload)
    {
        if (payload.EventType == NutEventType.NutInGoal)
        {
            DebugLogger.Log(DebugLogCategory.GoalSystem, $"{goalType} goal received NutInGoal event - Nut: {payload.NutType} at position: {payload.CurrentPosition}", this);
            OnNutInGoal(payload);
        }
    }

    /// <summary>
    /// Invoked when a nut reaches the goal. This method can be overridden to provide custom behavior.
    /// </summary>
    /// <remarks>The default implementation starts a coroutine to handle post-goal processing based on the
    /// nut's current position. Override this method to customize the behavior when a nut reaches the goal.</remarks>
    /// <param name="payload">The event payload containing information about the nut, including its current position.</param>
    protected virtual void OnNutInGoal(NutEventPayload payload)
    {
        StartCoroutine(OnNutInGoalDone(payload.CurrentPosition));
        DebugLogger.Log(DebugLogCategory.GoalSystem, $"{goalType} goal processing {payload.NutType} nut - sending NutInGoalDone event", this);
    }
        
    /// <summary>
    /// Handles the completion of processing a nut in the goal and raises the corresponding event.
    /// </summary>
    /// <remarks>This method ensures that all processing related to the nut in the goal is complete before
    /// raising the <see cref="GoalEventsType.NutInGoalDone"/> event. The event notifies listeners that the nut's
    /// interaction with the goal has concluded. If shouldRemoveGoal is true, destroys the goal tile.</remarks>
    /// <param name="position">The grid position of the goal where the nut was processed.</param>
    /// <returns>An enumerator for the coroutine, which waits a frame before raising the event.</returns>
    private IEnumerator OnNutInGoalDone(Vector2Int position)
    {
        //wait a frame to ensure all processing is complete before continuing
        yield return null;

        // Raise nut in goal done event to notify that all interaction with goal are complete
        goalEvents.Raise(new GoalEventPayload
        {
            EventType = GoalEventsType.NutInGoalDone,
            Position = position,
            ScoreValue = 0,
            GoalRemove = shouldRemoveGoal,
        });

        // Destroy goal tile if marked for removal
        if (shouldRemoveGoal)
        {
            DebugLogger.Log(DebugLogCategory.GoalSystem, $"{goalType} goal at {position} marked for removal - destroying", this);
            Destroy(gameObject);
        }
    }
    #endregion
}



