using UnityEditor.XR;
using UnityEngine;

/// <summary>
/// Specifies the type of action to perform on an image, such as showing, closing, or replacing it.
/// </summary>
/// <remarks>Use this enumeration to indicate the intended image operation in APIs that support multiple image
/// actions.</remarks>
public enum ActionImageType
{
   Show,
   Close,
   Replace,
}

/// <summary>
/// Single action in a tutorial sequence.
/// </summary>
[System.Serializable]
public struct TutorialAction
{
    public TutorialActionType ActionType;

    [Header("Animation")]
    [Tooltip("Direction of Tutorial character animation")]
    public Direction AnimationDirection;

    [Header("Message")]
    [TextArea(2, 6)]
    [Tooltip("Text message to display in tutorial overlay")]
    public string MessageText;
    public float MessageDisplayTime;
    
    [Header("Action Image")]
    [Tooltip("Image to display in tutorial overlay")]
    public Sprite ActionImage;
    public bool ExecuteWithMessage;
    public ActionImageType ImageActionType;
    public float ActionImageDisplayTime;
}
