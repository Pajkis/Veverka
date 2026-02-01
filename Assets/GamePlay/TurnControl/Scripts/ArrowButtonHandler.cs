
using UnityEngine;

/// <summary>
/// Handling of arrow button trigger
/// </summary>
public class ArrowButtonHandler : MonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private SettingEvents settingEvents;

    public DirectionEvent directionPressed;
    private float gameplaySpeedMultiplier = 1f;

    /// <summary>
    /// Subscribe to setting events and request initial animation speed
    /// </summary>
    private void OnEnable()
    {
        // Subscribe first, then request - ensures we receive the DataBroadcast response
        settingEvents?.AddListener(OnSettingEvent);

        // Request current animation speed value from GameSettings
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
            DebugLogger.Log(DebugLogCategory.Settings, $"ArrowButtonHandler received animation speed update: {gameplaySpeedMultiplier}", this);
        }
    }

    /// <summary>
    /// Raises arrow trigger event with gameplay speed multiplier
    /// </summary>
    public void OnClick()
    {
        // Don't process input if pause menu is open
        if (FindAnyObjectByType<PauseMenu>() != null)
        {
            DebugLogger.Log(DebugLogCategory.Input, $"Arrow button {direction} clicked but pause menu is open - ignoring", this);
            return;
        }

        directionPressed.Raise(new DirectionPayload
        {
            direction = direction,
            SpeedMultiplier = gameplaySpeedMultiplier
        });
        DebugLogger.Log(DebugLogCategory.Input, $"Arrow pressed: {direction} with speedMultiplier: {gameplaySpeedMultiplier}", this);
    }

}
