using UnityEngine;

/// <summary>
/// Handles displaying bubble messages for character events and goal resolutions.
/// </summary>
public class BubbleMessageManager : MonoBehaviour
{
    #region Fields
    [Header("Events")]
    [SerializeField] GoalEvents goalEvents;
    [SerializeField] ObstacleEvents obstacleEvents;
    [SerializeField] CharacterEvents characterEvents;

    [Header("Prefab bubble")]
    [SerializeField] GameObject prefabBubbleBox;

    Transform gridRoot;
    Vector2Int characterPosition;
    bool hasShownMessageThisTurn = false;
    #endregion

    #region Methods
    /// <summary>
    /// Finds the GridRoot in the scene.
    /// </summary>
    private void Start()
    {
        gridRoot = GameObject.Find("GridRoot")?.transform;
        if (gridRoot == null)
        {
            Debug.LogError("GridRoot not found in the scene. Please make sure there is a GameObject named 'GridRoot'.");
        }
    }

    /// <summary>
    /// event subscriptions
    /// </summary>
    private void OnEnable()
    {
        goalEvents.AddListener(OnGoalEvent);
        obstacleEvents.AddListener(OnObstacleEvent);
        characterEvents.AddListener(OnCharacterEvent);
    }

    /// <summary>
    /// event unsubscriptions
    /// </summary>
    private void OnDisable()
    {
        goalEvents.RemoveListener(OnGoalEvent);
        obstacleEvents.RemoveListener(OnObstacleEvent);
        characterEvents.RemoveListener(OnCharacterEvent);
    }

    /// <summary>
    /// Handles character events for movement and data responses.
    /// </summary>
    /// <param name="payload"></param>
    private void OnCharacterEvent(CharacterEventPayload payload)
    {
        if (payload.EventType == CharacterEventType.MoveCompleted)
        {
            characterPosition = payload.CurrentPosition;
            // Reset message flag for new turn
            hasShownMessageThisTurn = false;
        }
        else if (payload.EventType == CharacterEventType.DataResponse)
        {
            characterPosition = payload.CurrentPosition;
            if (payload.CharacterBubbleMessage == BubbleMessageType.NoMessage) return;

            Vector2Int bubblePos = characterPosition + new Vector2Int(0, 1);
            CreateBubble(bubblePos, payload.CharacterBubbleMessage, payload.CharacterBubbleMessageTime);
        }
    }

    /// <summary>
    /// on goal resolved event handler
    /// </summary>
    /// <param name="payload"></param>
    private void OnGoalEvent(GoalEventPayload payload)
    {
        if (payload.EventType != GoalEventsType.GoalResolve) return;

        // Show only first message per turn (shared flag with obstacles)
        if (hasShownMessageThisTurn) return;
        hasShownMessageThisTurn = true;

        // Calculate bubble position behind character relative to goal
        Vector2Int bubbleOffset = CalculateBubbleOffset(payload.Position, characterPosition);
        Vector2Int bubblePos = characterPosition + bubbleOffset;
        CreateBubble(bubblePos, payload.GoalMessage, payload.GoalMessageTime);
    }

    /// <summary>
    /// on obstacle resolved event handler
    /// </summary>
    /// <param name="payload"></param>
    private void OnObstacleEvent(ObstacleEventPayload payload)
    {
        if (payload.EventType != ObstacleEventsType.ObstacleResolve) return;

        // Show only first message per turn (shared flag with goals)
        if (hasShownMessageThisTurn) return;
        hasShownMessageThisTurn = true;

        // Calculate bubble position behind character relative to obstacle
        Vector2Int bubbleOffset = CalculateBubbleOffset(payload.Position, characterPosition);
        Vector2Int bubblePos = characterPosition + bubbleOffset;
        CreateBubble(bubblePos, payload.ObstacleMessage, payload.ObstacleMessageTime);
    }


    /// <summary>
    /// creates a bubble message at the specified grid position with the given type and display time.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    /// <param name="time"></param>
    private void CreateBubble(Vector2Int position, BubbleMessageType type, float time)
    {
        BubbleMsgBox bubbleBox = Instantiate(prefabBubbleBox, Vector3.zero, Quaternion.identity, gridRoot)
            .GetComponent<BubbleMsgBox>();
        bubbleBox.transform.localPosition = GridUtils.GridToWorld(position);
        bubbleBox.Init(GetMessage(type), time);
    }

    /// <summary>
    /// Calculates the offset for the bubble message to appear behind the character relative to the goal.
    /// </summary>
    /// <param name="goalPos"></param>
    /// <param name="charPos"></param>
    /// <returns></returns>
    private Vector2Int CalculateBubbleOffset(Vector2Int goalPos, Vector2Int charPos)
    {
        Vector2Int relativePos = goalPos - charPos;

        // Position bubble behind character (opposite direction from goal)
        if (relativePos.x > 0) return new Vector2Int(-1, 0); // Goal is right, bubble left
        if (relativePos.x < 0) return new Vector2Int(1, 0);  // Goal is left, bubble right
        if (relativePos.y > 0) return new Vector2Int(0, -1); // Goal is up, bubble down
        if (relativePos.y < 0) return new Vector2Int(0, 1);  // Goal is down, bubble up

        // If diagonal or same position, default to above character
        return new Vector2Int(0, 1);
    }


    /// <summary>
    /// gets the message string corresponding to the given BubbleMessageType.
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    private string GetMessage(BubbleMessageType messageType)
    {
        return messageType switch
        {
            BubbleMessageType.LetsStart => "Ready!",
            BubbleMessageType.Yatta => "Yatta!",
            BubbleMessageType.Road => "Road!",
            BubbleMessageType.Splash => "Splash!",
            BubbleMessageType.Lake => "Lake!",
            BubbleMessageType.Ooops => "Ooops!",
            _ => "error",
        };
    }
    #endregion
}

