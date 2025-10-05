using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnRecorder : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private CharacterEvents characterEvents;
    [SerializeField] private NutEvents nutEvents;
    [SerializeField] private GoalEvents goalEvents;

    [Header("Debug")]
    [SerializeField] private DebugLogConfig debugConfig;

    [Header("Config")]
    [SerializeField] private GameplayConfig gameplayConfig;

    private List<UndoData> currentTurnData;
    private HashSet<string> expectedSources = new();
    private HashSet<string> completedSources = new();
    private bool isRecording = false;

    public event Action<List<UndoData>> OnTurnCompleted;

    private void Awake()
    {
        DebugLogger.Log(DebugLogCategory.TurnRecord, "EventBasedTurnRecorder initialized", this);
    }

    private void OnEnable()
    {
        characterEvents?.AddListener(OnCharacterEvent);
        nutEvents?.AddListener(OnNutEvent);
        goalEvents?.AddListener(OnGoalEvent);

        DebugLogger.Log(DebugLogCategory.TurnRecord, "Event listeners registered", this);
    }

    private void OnDisable()
    {
        characterEvents?.RemoveListener(OnCharacterEvent);
        nutEvents?.RemoveListener(OnNutEvent);
        goalEvents?.RemoveListener(OnGoalEvent);
    }

    public void StartTurn()
    {
        if (isRecording)
        {
            DebugLogger.LogWarning(DebugLogCategory.TurnRecord, "Turn already in progress, cancelling previous", this);
            CancelTurn();
        }

        currentTurnData = new List<UndoData>();
        expectedSources.Clear();
        completedSources.Clear();
        isRecording = true;

        DebugLogger.Log(DebugLogCategory.TurnRecord, "Turn recording started", this);
    }

    public void ExpectSource(string sourceId)
    {
        if (!isRecording)
        {
            StartTurn();
        }

        expectedSources.Add(sourceId);
        DebugLogger.Log(DebugLogCategory.TurnRecord, $"Expecting source: {sourceId}", this);
    }

    public void AddUndoData(UndoData undoData)
    {
        if (!isRecording)
        {
            DebugLogger.LogWarning(DebugLogCategory.TurnRecord, "Not recording, ignoring undo data", this);
            return;
        }

        currentTurnData.Add(undoData);
    }

    public void CompleteSource(string sourceId)
    {
        if (!isRecording)
        {
            DebugLogger.LogWarning(DebugLogCategory.TurnRecord, "Not recording, ignoring source completion", this);
            return;
        }

        completedSources.Add(sourceId);
        DebugLogger.Log(DebugLogCategory.TurnRecord, $"Source completed: {sourceId}", this);

        TryFinalizeTurn();
    }

    public void CancelTurn()
    {
        isRecording = false;
        currentTurnData = null;
        expectedSources.Clear();
        completedSources.Clear();

        DebugLogger.Log(DebugLogCategory.TurnRecord, "Turn cancelled", this);
    }

    private void TryFinalizeTurn()
    {
        if (!isRecording) return;

        DebugLogger.Log(DebugLogCategory.TurnRecord,
            $"Checking completion: Expected={string.Join(", ", expectedSources)}, Completed={string.Join(", ", completedSources)}", this);

        if (expectedSources.SetEquals(completedSources))
        {
            isRecording = false;

            DebugLogger.Log(DebugLogCategory.TurnRecord,
                $"Turn finalized with {currentTurnData.Count} actions", this);

            OnTurnCompleted?.Invoke(currentTurnData);

            currentTurnData = null;
            expectedSources.Clear();
            completedSources.Clear();
        }
    }

    private void OnCharacterEvent(CharacterEventPayload payload)
    {
        switch (payload.EventType)
        {
            case CharacterEventType.MoveStarted:
                // Use PreviousPosition (start position) for consistent tracking
                var moveSourceId = $"Character-{payload.PreviousPosition.x}x{payload.PreviousPosition.y}";
                ExpectSource(moveSourceId);
                break;

            case CharacterEventType.RotateStarted:
                // Use CurrentPosition for rotation (position doesn't change)
                var rotateSourceId = $"Character-{payload.CurrentPosition.x}x{payload.CurrentPosition.y}";
                ExpectSource(rotateSourceId);
                break;

            case CharacterEventType.MoveCompleted:
                var moveData = UndoData.CreateCharacterMove(payload.Character, payload.CurrentPosition, payload.PreviousPosition, payload.CurrentDirection, gameplayConfig.undoStepTime);
                AddUndoData(moveData);

                var completedSourceId = $"Character-{payload.PreviousPosition.x}x{payload.PreviousPosition.y}";
                CompleteSource(completedSourceId);
                break;

            case CharacterEventType.RotateCompleted:
                var rotationData = UndoData.CreateCharacterRotation(payload.Character, payload.CurrentPosition, payload.CurrentDirection, payload.PreviousDirection, gameplayConfig.undoStepTime);
                AddUndoData(rotationData);

                var rotationSourceId = $"Character-{payload.CurrentPosition.x}x{payload.CurrentPosition.y}";
                CompleteSource(rotationSourceId);
                break;

            case CharacterEventType.MoveFailed:
                // If move failed, complete source without adding undo data
                var failedSourceId = $"Character-{payload.CurrentPosition.x}x{payload.CurrentPosition.y}";
                CompleteSource(failedSourceId);
                break;
        }
    }

    private void OnNutEvent(NutEventPayload payload)
    {
        switch (payload.EventType)
        {
            case NutEventType.NutPush:
                var sourceId = $"Nut-{payload.PreviousPosition.x}x{payload.PreviousPosition.y}";
                ExpectSource(sourceId);
                break;

            case NutEventType.NutMoved:
                var moveData = UndoData.CreateNutMove(payload.NutTile, payload.CurrentPosition, payload.PreviousPosition, payload.Duration);
                AddUndoData(moveData);

                var completedSourceId = $"Nut-{payload.PreviousPosition.x}x{payload.PreviousPosition.y}";
                CompleteSource(completedSourceId);
                break;
        }
    }

    private void OnGoalEvent(GoalEventPayload payload)
    {
        switch (payload.EventType)
        {
            case GoalEventsType.NutInGoalDone:
                // Goal achieved - cancel current turn and clear history
                DebugLogger.Log(DebugLogCategory.TurnRecord, "Goal achieved - cancelling turn", this);
                CancelTurn();
                break;
        }
    }
}