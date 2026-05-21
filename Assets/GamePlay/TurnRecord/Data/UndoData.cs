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
    public float SpeedMultiplier;
    public string ObjectId; // For identifying which object to undo

    public static UndoData CreateCharacterMove(Character character, Vector2Int current, Vector2Int previous, Direction direction, float speedMultiplier = 1.0f)
    {
        return new UndoData
        {
            ActionType = UndoActionType.CharacterMove,
            CurrentPosition = current,
            PreviousPosition = previous,
            CurrentDirection = direction,
            PreviousDirection = direction,
            SpeedMultiplier = speedMultiplier,
            ObjectId = $"Character-{character.GetInstanceID()}"
        };
    }

    public static UndoData CreateCharacterRotation(Character character, Vector2Int position, Direction current, Direction previous, float speedMultiplier = 1.0f)
    {
        return new UndoData
        {
            ActionType = UndoActionType.CharacterRotation,
            CurrentPosition = position,
            PreviousPosition = position,
            CurrentDirection = current,
            PreviousDirection = previous,
            SpeedMultiplier = speedMultiplier,
            ObjectId = $"Character-{character.GetInstanceID()}"
        };
    }

    public static UndoData CreateNutMove(NutTile nut, Vector2Int current, Vector2Int previous, float speedMultiplier)
    {
        return new UndoData
        {
            ActionType = UndoActionType.NutMove,
            CurrentPosition = current,
            PreviousPosition = previous,
            SpeedMultiplier = speedMultiplier,
            ObjectId = $"Nut-{nut.GetInstanceID()}"
        };
    }
}