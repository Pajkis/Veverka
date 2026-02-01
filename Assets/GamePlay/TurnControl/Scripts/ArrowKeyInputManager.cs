using System;
using UnityEngine;

/// <summary>
/// Polls keyboard arrow keys and UI button arrows. Raises a direction event with gameplay speed multiplier.
/// </summary>
public class ArrowKeyInputManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private DirectionEvent arrowPressed;
    [SerializeField] private SettingEvents settingEvents;
    [SerializeField] private DirectionEvent directionPressed;

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
    /// <summary>
    /// register input from arrow keys and raise direction event
    /// </summary>
    void Update()
    {
        // Don't process input if pause menu is open
        if (FindAnyObjectByType<PauseMenu>() != null)
        {
            DebugLogger.Log(DebugLogCategory.Input, $"Arrow key input blocker because pause menu is open", this);
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

    /// <summary>
    /// Raises UI button arrow trigger direction event with gameplay speed multiplier
    /// </summary>
    public void ArrowButtonOnClickEvent(int directionInt)
    {

        // get direction from int
        Direction direction = directionInt switch
        {
            0 => Direction.Up,
            1 => Direction.Down,
            2 => Direction.Left,
            3 => Direction.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(directionInt), $"Invalid direction int value: {directionInt}")
        };

        // Don't process input if pause menu is open
        if (FindAnyObjectByType<PauseMenu>() != null)
        {
            DebugLogger.Log(DebugLogCategory.Input, $"Arrow button {direction} clicked but pause menu is open - ignoring", this);
            return;
        }
              

        // raise direction event
        directionPressed.Raise(new DirectionPayload
        {
            direction = direction,
            SpeedMultiplier = gameplaySpeedMultiplier
        });
        DebugLogger.Log(DebugLogCategory.Input, $"Arrow pressed: {direction} with speedMultiplier: {gameplaySpeedMultiplier}", this);
    }
}

