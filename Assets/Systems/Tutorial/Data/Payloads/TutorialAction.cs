using UnityEngine;

/// <summary>
/// Single action in a tutorial sequence.
/// </summary>
[System.Serializable]
public struct TutorialAction
{
    public TutorialActionType ActionType;

    [Header("Animation")]
    public Direction AnimationDirection;

    [Header("Message")]
    [TextArea(2, 6)]
    public string MessageText;
    public float MessageDisplayTime;
}
