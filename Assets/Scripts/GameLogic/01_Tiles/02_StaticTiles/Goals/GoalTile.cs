using UnityEngine;

public class GoalTile : StaticTile
{
    [SerializeField]
    protected PushableInGoalEvent pushableInGoalEvent;

    /// <summary>
    /// On enable add listener
    /// </summary>
    private void OnEnable()
    {
        pushableInGoalEvent.AddListener(OnPushableInGoal);
    }

    /// <summary>
    /// On disable remove listener
    /// </summary>
    private void OnDisable()
    {
        pushableInGoalEvent.RemoveListener(OnPushableInGoal);
    }

    /// <summary>
    /// On pushable in goal event settlement
    /// </summary>
    /// <param name="payload"></param>
    private void OnPushableInGoal(PushableInGoalPayload payload)
    {
        if (GameGrid.Instance.Goals.TryGetValue(payload.Position, out var goal) && gridPosition == payload.Position)
        { 
            GameGrid.Instance.Goals.Remove(payload.Position);          
            Destroy(gameObject);
        }
    }
}
