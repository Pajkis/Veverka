using UnityEngine;

/// <summary>
///  bubble message manager - handle goal resolved and character data to show bubble messages
/// </summary>
public class BubbleMessageManager : MonoBehaviour
{
    #region Fields
    [Header("Events")]
    [SerializeField] GoalResolvedEvent goalResolvedEvent;
    [SerializeField] CharacterDataRequestEvent characterDataRequestEvent;

    [Header("Prefab bubble")]
    [SerializeField] GameObject prefabBubbleBox;

    Transform gridRoot;
    Vector2Int characterPosition;
    Vector2Int goalPosition;
    Vector2Int bubbleBoxPosition;

    // Pending goal data - waiting for character position
    private GoalBasicPayload pendingGoalPayload;
    private BubbleMessageType characterMessage;
    private bool waitingForCharacterData = false;
    #endregion

    #region methods

    private void Start()
    {
        gridRoot = GameObject.Find("GridRoot").transform;
        if (gridRoot == null)
        {
            Debug.LogError("GridRoot not found in the scene. Please make sure there is a GameObject named 'GridRoot'.");
        }

        // No need to request startup data - Veverka will send it automatically on scene load
    }

    /// <summary>
    /// Add listeners to events
    /// </summary>
    private void OnEnable()
    {
        goalResolvedEvent.AddListener(OnGoalResolved);
        characterDataRequestEvent.AddListener(OnCharacterDataReceived);
    }

    /// <summary>
    /// Remove listeners from events
    /// </summary>
    private void OnDisable()
    {
        goalResolvedEvent.RemoveListener(OnGoalResolved);
        characterDataRequestEvent.RemoveListener(OnCharacterDataReceived);
    }

    /// <summary>
    /// Handle goal resolved event - get goal position and request character data
    /// </summary>
    /// <param name="payload"></param>
    private void OnGoalResolved(GoalBasicPayload payload)
    {
        Debug.Log("Goal resolved at position: " + payload.Position.ToString());

        // Store goal data and position
        pendingGoalPayload = payload;
        goalPosition = payload.Position;
        waitingForCharacterData = true;

        // Wait a frame to ensure character has finished moving before requesting data
        StartCoroutine(RequestCharacterDataWithDelay());
    }

    /// <summary>
    /// Request character data with a small delay to ensure character has finished moving
    /// </summary>
    private System.Collections.IEnumerator RequestCharacterDataWithDelay()
    {
        // Wait a frame to ensure all movement is complete
        yield return new WaitForEndOfFrame();

        // Request character data
        characterDataRequestEvent.Raise(new CharacterBasicPayload
        {
            RequestData = true,
            ResponseData = false
        });
    }

    /// <summary>
    /// Handle character data response
    /// </summary>
    /// <param name="payload"></param>
    private void OnCharacterDataReceived(CharacterBasicPayload payload)
    {
        // Only process responses, not requests
        if (!payload.ResponseData) return;

        Debug.Log($"Character data received - Message: {payload.CharacterBubbleMessage}, Position: {payload.Current}");

        // Handle startup message (LetsStart) - separate from goal resolved logic
        if (payload.CharacterBubbleMessage == BubbleMessageType.LetsStart)
        {
            // For startup message, we don't need goal position - just show above character
            characterPosition = payload.Current;
            goalPosition = characterPosition; // Set same position so bubble appears above
            characterMessage = payload.CharacterBubbleMessage;

            // Create fake goal payload for startup message
            pendingGoalPayload = new GoalBasicPayload
            {
                VeverkaMessage = BubbleMessageType.LetsStart,
                VeverkaMessageTime = payload.CharacterBubbleMessageTime,
                Position = characterPosition
            };

            Debug.Log($"Processing startup message at position: {characterPosition}");
            CalculateBubblePositionAndInstantiate();
            return;
        }

        // Only process if we're waiting for character data (original logic for goal resolved)
        if (!waitingForCharacterData) return;

        // Get character position
        characterPosition = payload.Current;
        characterMessage = payload.CharacterBubbleMessage;
        waitingForCharacterData = false;

        Debug.Log($"Character position received: {characterPosition}, Goal position: {goalPosition}");

        // Now we have both positions, calculate bubble position and instantiate
        CalculateBubblePositionAndInstantiate();
    }

    /// <summary>
    /// Calculate bubble position based on goal and character positions, then instantiate
    /// </summary>
    private void CalculateBubblePositionAndInstantiate()
    {
        Vector2Int bubbleOffset = CalculateBubbleOffset();
        bubbleBoxPosition = characterPosition + bubbleOffset;

        Debug.Log($"Bubble will be placed at: {bubbleBoxPosition} (Character: {characterPosition}, Offset: {bubbleOffset})");

        // Instantiate bubble message
        BubbleMsgBox bubbleBox = Instantiate(prefabBubbleBox, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<BubbleMsgBox>();
        bubbleBox.transform.SetParent(gridRoot, false);
        bubbleBox.transform.localPosition = GridUtils.GridToWorld(bubbleBoxPosition);

        string message = GetMessage(pendingGoalPayload.VeverkaMessage);
        bubbleBox.Init(message, pendingGoalPayload.VeverkaMessageTime);
    }

    /// <summary>
    /// Calculate bubble offset based on relative positions of goal and character
    /// </summary>
    /// <returns>Offset vector for bubble position</returns>
    private Vector2Int CalculateBubbleOffset()
    {
        Vector2Int offset;

        // Calculate relative position of goal to character
        Vector2Int relativePos = goalPosition - characterPosition;

        // Determine bubble position based on goal relative to character

        if (relativePos.x != 0 && relativePos.y != 0 || characterMessage == BubbleMessageType.LetsStart) // Diagonal case
        {
            offset = new Vector2Int(0, 1); // Place bubble above
        }
        else if (relativePos.x < 0) // Goal is to the left
        {
            offset = new Vector2Int(1, 0); // Place bubble to the right
        }
        else if (relativePos.x > 0) // Goal is to the right
        {
            offset = new Vector2Int(-1, 0); // Place bubble to the left
        }
        else if (relativePos.y > 0) // Goal is above
        {
            offset = new Vector2Int(0, -1); // Place bubble below
        }
        else if (relativePos.y < 0) // Goal is below
        {
            offset = new Vector2Int(0, 1); // Place bubble above
        }
        else // Goal and character are at same position (shouldn't happen, but fallback)
        {
            offset = new Vector2Int(0, 1); // Default: place bubble above
        }

        Debug.Log($"Goal relative to character: {relativePos}, Bubble offset: {offset}");
        return offset;
    }

    /// <summary>
    /// Get message string based on message type
    /// </summary>
    /// <param name="messageType"></param>
    private string GetMessage(BubbleMessageType messageType)
    {
        string message = messageType switch
        {
            BubbleMessageType.LetsStart => "Ready!",
            BubbleMessageType.Yatta => "Yatta!",
            BubbleMessageType.Ooops => "Ooops!",
            _ => "error",
        };

        return message;
    }
    #endregion
}