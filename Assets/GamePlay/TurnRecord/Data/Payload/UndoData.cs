using System;
using UnityEngine;

[System.Serializable]
public enum UndoActionType
{
    CharacterMove,
    CharacterRotation,
    NutMove
}

[System.Serializable]
public struct UndoData
{
    public UndoActionType ActionType;
    public Vector2Int CurrentPosition;
    public Vector2Int PreviousPosition;
    public Direction CurrentDirection;
    public Direction PreviousDirection;
    public float Duration;
    public string ObjectId; // For identifying which object to undo

    public static UndoData CreateCharacterMove(Character character, Vector2Int current, Vector2Int previous, Direction direction, float duration = 0.15f)
    {
        return new UndoData
        {
            ActionType = UndoActionType.CharacterMove,
            CurrentPosition = current,
            PreviousPosition = previous,
            CurrentDirection = direction,
            PreviousDirection = direction,
            Duration = duration,
            ObjectId = $"Character-{character.GetInstanceID()}"
        };
    }

    public static UndoData CreateCharacterRotation(Character character, Vector2Int position, Direction current, Direction previous, float duration = 0.15f)
    {
        return new UndoData
        {
            ActionType = UndoActionType.CharacterRotation,
            CurrentPosition = position,
            PreviousPosition = position,
            CurrentDirection = current,
            PreviousDirection = previous,
            Duration = duration,
            ObjectId = $"Character-{character.GetInstanceID()}"
        };
    }

    public static UndoData CreateNutMove(NutTile nut, Vector2Int current, Vector2Int previous, float duration)
    {
        return new UndoData
        {
            ActionType = UndoActionType.NutMove,
            CurrentPosition = current,
            PreviousPosition = previous,
            Duration = duration,
            ObjectId = $"Nut-{nut.GetInstanceID()}"
        };
    }
}