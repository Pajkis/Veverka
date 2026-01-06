using System.Collections;
using UnityEngine;

/// <summary>
/// Obstacle tile abstract class - base for all interactive obstacles (walls, holes, etc.)
/// </summary>
public abstract class ObstacleTile : TileObject
{
    protected ObstacleType obstacleType;
    protected bool shouldRemoveObstacle = false;

    #region events
    [SerializeField] protected NutEvents nutEvents;
    [SerializeField] protected ObstacleEvents obstacleEvents;
    [SerializeField] protected RoadEvents roadEvents;
    [SerializeField] protected GoalEvents goalEvents;
    #endregion

    #region Init
    /// <summary>
    /// Init obstacles and register them in GameGrid
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    /// <param name="obstacleType"></param>
    public virtual void Init(TileType tileType, Vector2Int gridPosition, ObstacleType obstacleType)
    {
        base.Init(tileType, gridPosition);
        this.obstacleType = obstacleType;

        DebugLogger.Log(DebugLogCategory.TileInteraction, $"Initialized {obstacleType} at position: {gridPosition}", this);
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
    /// Reaction on nut reaching the obstacle
    /// </summary>
    /// <param name="payload"></param>
    private void OnNutEvent(NutEventPayload payload)
    {
        if (payload.EventType == NutEventType.NutInGoal)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{obstacleType} obstacle received NutInGoal event - Nut: {payload.NutType} at position: {payload.CurrentPosition}", this);
            OnNutInObstacle(payload);
        }
    }

    /// <summary>
    /// Invoked when a nut reaches the obstacle. This method can be overridden to provide custom behavior.
    /// </summary>
    /// <remarks>The default implementation starts a coroutine to handle post-obstacle processing based on the
    /// nut's current position. Override this method to customize the behavior when a nut reaches the obstacle.</remarks>
    /// <param name="payload">The event payload containing information about the nut, including its current position.</param>
    protected virtual void OnNutInObstacle(NutEventPayload payload)
    {
        StartCoroutine(OnObstacleActionDone(payload.CurrentPosition));
        DebugLogger.Log(DebugLogCategory.TileInteraction, $"{obstacleType} obstacle processing {payload.NutType} nut - sending ObstacleActionDone event", this);
    }

    /// <summary>
    /// Handles the completion of processing a nut in the obstacle and raises the corresponding event.
    /// </summary>
    /// <remarks>This method ensures that all processing related to the nut in the obstacle is complete before
    /// raising the <see cref="ObstacleEventsType.ObstacleActionDone"/> event. The event notifies listeners that the nut's
    /// interaction with the obstacle has concluded. If shouldRemoveObstacle is true, destroys the obstacle tile.</remarks>
    /// <param name="position">The grid position of the obstacle where the nut was processed.</param>
    /// <returns>An enumerator for the coroutine, which waits a frame before raising the event.</returns>
    private IEnumerator OnObstacleActionDone(Vector2Int position)
    {
        //wait a frame to ensure all processing is complete before continuing
        yield return null;

        // Raise obstacle action done event to notify that all interaction with obstacle are complete
        obstacleEvents.Raise(new ObstacleEventPayload
        {
            EventType = ObstacleEventsType.ObstacleActionDone,
            Position = position,
            ObstacleRemove = shouldRemoveObstacle,
        });

        // Destroy obstacle tile if marked for removal
        if (shouldRemoveObstacle)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, $"{obstacleType} obstacle at {position} marked for removal - destroying", this);
            Destroy(gameObject);
        }
    }
    #endregion
}
