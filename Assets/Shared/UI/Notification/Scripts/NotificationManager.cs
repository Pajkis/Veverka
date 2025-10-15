using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages notification overlays for user feedback
/// Integrates with the existing overlay system
/// </summary>
public class NotificationManager : MonoBehaviour
{
    private static NotificationManager instance;

    [Header("Events")]
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;

    private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
    private bool isShowingNotification = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Show a success notification
    /// </summary>
    public static void ShowSuccess(string message, string errorCode = null)
    {
        if (instance != null)
        {
            instance.QueueNotification(message, NotificationType.Success, errorCode);
        }
        else
        {
            Debug.LogWarning($"NotificationManager not found. Success: {message}");
        }
    }

    /// <summary>
    /// Show an error notification
    /// </summary>
    public static void ShowError(string message, string errorCode = null)
    {
        if (instance != null)
        {
            instance.QueueNotification(message, NotificationType.Error, errorCode);
        }
        else
        {
            Debug.LogError($"NotificationManager not found. Error: {message}");
        }
    }

    /// <summary>
    /// Show an error notification using ErrorCode enum
    /// </summary>
    public static void ShowError(ErrorCode code, string context = "")
    {
        string message = ErrorMessages.Get(code, context);
        string codeString = ErrorMessages.GetCodeString(code);
        ShowError(message, codeString);
    }

    /// <summary>
    /// Show an info notification
    /// </summary>
    public static void ShowInfo(string message, string errorCode = null)
    {
        if (instance != null)
        {
            instance.QueueNotification(message, NotificationType.Info, errorCode);
        }
        else
        {
            Debug.Log($"NotificationManager not found. Info: {message}");
        }
    }

    private void QueueNotification(string message, NotificationType type, string errorCode = null)
    {
        var notificationData = new NotificationData
        {
            Message = message,
            Type = type,
            ErrorCode = errorCode
        };

        notificationQueue.Enqueue(notificationData);

        DebugLogger.Log(DebugLogCategory.UI, $"Notification queued: [{type}] {message}");

        // If not currently showing a notification, show the next one
        if (!isShowingNotification)
        {
            ShowNextNotification();
        }
    }

    private void ShowNextNotification()
    {
        if (notificationQueue.Count == 0)
        {
            isShowingNotification = false;
            return;
        }

        isShowingNotification = true;
        var data = notificationQueue.Dequeue();

        // Store current notification data for the overlay to access
        CurrentNotification = data;

        // Open notification overlay via your system
        if (sceneNavigationEvents != null)
        {
            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.OpenOverlay,
                Overlay = OverlayType.Notification
            });

            DebugLogger.Log(DebugLogCategory.UI, $"Showing notification: [{data.Type}] {data.Message}");
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.UI, "SceneNavigationEvents not assigned in NotificationManager!");
            isShowingNotification = false;
        }
    }

    /// <summary>
    /// Called by NotificationOverlay when user closes it
    /// </summary>
    public static void OnNotificationClosed()
    {
        if (instance != null)
        {
            instance.ShowNextNotification();
        }
    }

    /// <summary>
    /// Current notification data for overlay to display
    /// </summary>
    public static NotificationData CurrentNotification { get; private set; }
}

/// <summary>
/// Data for a single notification
/// </summary>
public class NotificationData
{
    public string Message;
    public NotificationType Type;
    public string ErrorCode;  // Optional error code for debugging (e.g. "123")
}

