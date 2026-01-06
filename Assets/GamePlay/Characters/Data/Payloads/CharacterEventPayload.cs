using UnityEngine;

/// <summary>
/// Character basic payload - for register turn records and undo logic
/// </summary>
[System.Serializable]
public struct CharacterEventPayload
{  
    [Header("Core Character Data")]
    public Character Character;
    public CharacterEventType EventType;
    public Vector2Int CurrentPosition;
    public Vector2Int PreviousPosition;
    public Direction CurrentDirection;
    public Direction PreviousDirection;
    public float Duration;

    [Header("UI Effects")]
    public BubbleMessageEventPayload BubbleMessage;
    public PopoutImageEventPayload PopoutImage;

    // data request and response flags
    [Header("Request/response system")]
    public bool RequestData;
    public bool ResponseData;
}