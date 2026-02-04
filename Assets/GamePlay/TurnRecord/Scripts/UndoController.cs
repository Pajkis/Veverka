using System.Collections.Generic;
using UnityEngine;

public class UndoController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TurnRecorder turnRecorder;
    [SerializeField] private TurnControl turnControl;

    [Header("Events")]
    [SerializeField] private UndoEvents undoEvents;
    [SerializeField] private AudioEvents audioEvents;
    [SerializeField] private SettingEvents settingEvents;

    [Header("Debug")]
    [SerializeField] private DebugLogConfig debugConfig;

    private TurnHistory turnHistory;
    private HashSet<string> expectedUndoCompletions = new();
    private bool isUndoInProgress = false;
    private float gameplaySpeedMultiplier = 1f;

    private void Awake()
    {
        turnHistory = new TurnHistory();
        DebugLogger.Log(DebugLogCategory.UndoLogic, "EventBasedUndoController initialized", this);
    }

    private void OnEnable()
    {
        if (turnRecorder != null)
        {
            turnRecorder.OnTurnCompleted += OnTurnCompleted;
        }

        undoEvents?.AddListener(OnUndoEvent);
        settingEvents?.AddListener(OnSettingEvent);

        // Request current animation speed value
        settingEvents?.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataRequest,
            Setting = GameSettingsEnum.AnimationSpeed
        });
    }

    private void OnDisable()
    {
        if (turnRecorder != null)
        {
            turnRecorder.OnTurnCompleted -= OnTurnCompleted;
        }

        undoEvents?.RemoveListener(OnUndoEvent);
        settingEvents?.RemoveListener(OnSettingEvent);
    }

    public bool CanUndo()
    {
        return turnHistory.TurnCount > 0 && !turnControl.IsInputLocked && !isUndoInProgress;
    }

    public void RequestUndo()
    {
        if (!CanUndo())
        {
            DebugLogger.Log(DebugLogCategory.UndoLogic,
                $"Cannot undo: TurnCount={turnHistory.TurnCount}, InputLocked={turnControl.IsInputLocked}, UndoInProgress={isUndoInProgress}", this);
            return;
        }

        var lastTurn = turnHistory.GetLastTurn();
        if (lastTurn == null)
        {
            DebugLogger.LogWarning(DebugLogCategory.UndoLogic, "No turn to undo", this);
            return;
        }

        StartUndoProcess(lastTurn);
    }

    public void ClearHistory()
    {
        turnHistory.ClearHistory();
        turnRecorder.CancelTurn();
        DebugLogger.Log(DebugLogCategory.UndoLogic, "History and current turn cleared", this);
    }

    private void OnTurnCompleted(List<UndoData> turnData)
    {
        turnHistory.RegisterTurn(turnData);
        DebugLogger.Log(DebugLogCategory.TurnRecord, "Turn completed and registered in history", this);
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
            DebugLogger.Log(DebugLogCategory.Settings, $"UndoController received animation speed update: {gameplaySpeedMultiplier}", this);
        }
    }

    private void StartUndoProcess(List<UndoData> turnData)
    {
        isUndoInProgress = true;
        expectedUndoCompletions.Clear();

        DebugLogger.Log(DebugLogCategory.UndoLogic, "Starting undo process", this);

        // Play undo sound
        audioEvents?.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.Undo });

        // Lock input during undo without timeout mechanism
        turnControl.LockInputForUndo();

        // Send undo events for all data in reverse order
        for (int i = turnData.Count - 1; i >= 0; i--)
        {
            var undoData = turnData[i];
            var requestId = $"Undo-{i}-{System.Guid.NewGuid().ToString("N")[..8]}";
            expectedUndoCompletions.Add(requestId);

            var eventType = undoData.ActionType switch
            {
                UndoActionType.CharacterMove => UndoEventType.RequestCharacterUndo,
                UndoActionType.CharacterRotation => UndoEventType.RequestCharacterUndo,
                UndoActionType.NutMove => UndoEventType.RequestNutUndo,
                _ => UndoEventType.RequestCharacterUndo
            };

            undoEvents?.Raise(new UndoEventPayload
            {
                EventType = eventType,
                UndoData = undoData,
                RequestId = requestId,
                SpeedMultiplier = gameplaySpeedMultiplier
            });

            DebugLogger.Log(DebugLogCategory.UndoLogic,
                $"Sent {eventType} request with ID {requestId} for {undoData.ActionType}", this);
        }

        DebugLogger.Log(DebugLogCategory.UndoLogic, $"Undo process started with {expectedUndoCompletions.Count} expected completions", this);
    }

    private void OnUndoEvent(UndoEventPayload payload)
    {
        if (payload.EventType == UndoEventType.UndoCompleted && isUndoInProgress)
        {
            if (expectedUndoCompletions.Remove(payload.RequestId))
            {
                DebugLogger.Log(DebugLogCategory.UndoLogic,
                    $"Undo completion received for {payload.RequestId}. Remaining: {expectedUndoCompletions.Count}", this);

                if (expectedUndoCompletions.Count == 0)
                {
                    CompleteUndoProcess();
                }
            }
            else
            {
                DebugLogger.LogWarning(DebugLogCategory.UndoLogic,
                    $"Received unexpected undo completion for {payload.RequestId}", this);
            }
        }
    }

    private void CompleteUndoProcess()
    {
        isUndoInProgress = false;
        expectedUndoCompletions.Clear();

        // Unlock input after undo completion
        turnControl.UnlockInputAfterUndo();

        DebugLogger.Log(DebugLogCategory.UndoLogic, "Undo process completed", this);
    }
}