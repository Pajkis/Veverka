using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single action in a tutorial sequence
/// All action types are sequence-based, supporting multiple operations per element.
/// </summary>
[System.Serializable]
public struct TutorialAction
{
    public TutorialActionType ActionType;

    [Tooltip("Movement sequence steps - list of directions and repeat counts")]
    public List<MovementStep> MovementSteps;

    [Tooltip("Message sequence steps - list of messages with display times")]
    public List<MessageStep> MessageSteps;

    [Tooltip("Number of undos to perform in sequence")]
    [Min(1)]
    public int UndoCount;

    [Tooltip("Image action (single for now, designed for future list support)")]
    public ImageStep ImageAction;
}
