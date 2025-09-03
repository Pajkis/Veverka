using Unity.VisualScripting;
using UnityEngine;

public class BubbleMessageManager : MonoBehaviour
{
    #region Fields
    [Header("Events")]
    [SerializeField] GoalResolvedEvent goalResolvedEvent;
    [SerializeField] CharacterMovedEvent veverkaMovedEvent;

    [Header("Prefab bubble")]
    [SerializeField] GameObject prefabBubbleBox;
     
    Transform gridRoot;
    Vector2Int bubbleBoxPosition;
    readonly Vector2Int bubbleBoxOffset = new Vector2Int(0, 1);
    #endregion

    #region methods

    private void Start()
    {
        gridRoot = GameObject.Find("GridRoot").transform;
        if (gridRoot == null)
        {
            Debug.LogError("GridRoot not found in the scene. Please make sure there is a GameObject named 'GridRoot'.");
        }
    }

    /// <summary>
    /// Add listener to goal removed event
    /// </summary>
    private void OnEnable()
    {
        goalResolvedEvent.AddListener(OnGoalResolved);
        veverkaMovedEvent.AddListener(UpdatePosition);
    }

    /// <summary>
    /// remove listener from goal removed event
    /// </summary>
    private void OnDisable()
    {
        goalResolvedEvent.RemoveListener(OnGoalResolved);
        veverkaMovedEvent.AddListener(UpdatePosition);
    }

    /// <summary>
    /// Instantiate message prefab on goal removed event
    /// </summary>
    /// <param name="payload"></param>
    private void OnGoalResolved(GoalBasicPayload payload)
    {
        Debug.Log("Veverka message: " + payload.VeverkaMessage.ToString());
        BubbleMsgBox bubbleBox;
        bubbleBox = Instantiate(prefabBubbleBox, Vector3.zero, Quaternion.identity, gridRoot).GetComponent<BubbleMsgBox>();
        bubbleBox.transform.SetParent(gridRoot, false);

        // set bubble box position above the character
        bubbleBoxPosition.x = (bubbleBoxPosition.x + payload.Position.x) / 2 + bubbleBoxOffset.x;
        bubbleBoxPosition.y = (bubbleBoxPosition.y + payload.Position.y) / 2 + bubbleBoxOffset.y;
        bubbleBox.transform.localPosition = GridUtils.GridToWorld(bubbleBoxPosition);

        string message = GetMessage(payload.VeverkaMessage);
        bubbleBox.Init(message, payload.VeverkaMessageTime);
    }

    /// <summary>
    /// update bubble box position on character moved event
    /// </summary>
    /// <param name="payload"></param>
    private void UpdatePosition(CharacterMovedPayload payload)
    {
        bubbleBoxPosition = payload.current;
    }

    /// <summary>
    ///  get message string based on message type
    /// </summary>
    /// <param name="messageType"></param>
    private string GetMessage(BubbleMessageType messageType)
    {

        string message = messageType switch
        {
            BubbleMessageType.LetsStart => "Let's Start!",
            BubbleMessageType.Yatta => "Yatta!",
            BubbleMessageType.Ooops => "Ooops!",
            _ => "error",
        };

        return message;
    }

    #endregion

}
