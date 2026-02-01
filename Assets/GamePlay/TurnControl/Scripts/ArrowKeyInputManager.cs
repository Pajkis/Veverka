using UnityEngine;

/// <summary>
/// Polls keyboard arrow keys and raises a direction event with gameplay speed multiplier.
/// </summary>
public class ArrowKeyInputManager : MonoBehaviour
{
    [SerializeField] private DirectionEvent arrowPressed;
    [SerializeField] private SettingEvents settingEvents;

    private float gameplaySpeedMultiplier = 1f;

    /// <summary>
    /// Subscribe to setting events and request initial animation speed
    /// </summary>
    private void OnEnable()
    {
        settingEvents?.AddListener(OnSettingEvent);

        // Request current animation speed value
        settingEvents?.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataRequest,
            Setting = GameSettingsEnum.AnimationSpeed
        });
    }

    /// <summary>
    /// Unsubscribe from setting events
    /// </summary>
    private void OnDisable()
    {
        settingEvents?.RemoveListener(OnSettingEvent);
    }

    /// <summary>
    /// Handle setting change events - update cached speed multiplier
    /// </summary>
    private void OnSettingEvent(SettingEventPayload payload)
    {
        if (payload.EventType == SettingsEventType.DataBroadcast &&
            payload.Setting == GameSettingsEnum.AnimationSpeed)
        {
            gameplaySpeedMultiplier = payload.Value;
            DebugLogger.Log(DebugLogCategory.Settings, $"ArrowKeyInputManager received animation speed update: {gameplaySpeedMultiplier}", this);
        }
    }

    void Update()
    {
        // Don't process input if pause menu is open
        if (FindAnyObjectByType<PauseMenu>() != null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            arrowPressed.Raise(new DirectionPayload { direction = Direction.Up, SpeedMultiplier = gameplaySpeedMultiplier });
        if (Input.GetKeyDown(KeyCode.DownArrow))
            arrowPressed.Raise(new DirectionPayload { direction = Direction.Down, SpeedMultiplier = gameplaySpeedMultiplier });
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            arrowPressed.Raise(new DirectionPayload { direction = Direction.Left, SpeedMultiplier = gameplaySpeedMultiplier });
        if (Input.GetKeyDown(KeyCode.RightArrow))
            arrowPressed.Raise(new DirectionPayload { direction = Direction.Right, SpeedMultiplier = gameplaySpeedMultiplier });
    }
}

