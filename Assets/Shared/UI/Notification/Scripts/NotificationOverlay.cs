using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Notification overlay that displays message and allows user to close it
/// Works with NotificationManager and CloseOverlay system
/// </summary>
public class NotificationOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite successIcon;
    [SerializeField] private Sprite errorIcon;
    [SerializeField] private Sprite infoIcon;

    void Start()
    {
        DisplayNotification();
    }

    void OnDestroy()
    {
        // Notify manager that notification was closed
        NotificationManager.OnNotificationClosed();
    }

    private void DisplayNotification()
    {
        var notification = NotificationManager.CurrentNotification;

        if (notification == null)
        {
            DebugLogger.LogError(DebugLogCategory.UI, "NotificationOverlay: No notification data available!");
            return;
        }

        // Set title text (e.g. "Success!" or "Error! #123")
        if (titleText != null)
        {
            string title = notification.Type.ToString();
            if (!string.IsNullOrEmpty(notification.ErrorCode))
            {
                title += $" #{notification.ErrorCode}";
            }
            title += "!";

            titleText.text = title;            
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.UI, "NotificationOverlay: TitleText not assigned!");
        }

        // Set message text
        if (messageText != null)
        {
            messageText.text = notification.Message;
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.UI, "NotificationOverlay: MessageText not assigned!");
        }

        // Set icon based on type
        if (iconImage != null)
        {
            iconImage.sprite = GetIconForType(notification.Type);       
        }

        DebugLogger.Log(DebugLogCategory.UI, $"NotificationOverlay displayed: [{notification.Type}] {notification.Message}");
    }

    private Sprite GetIconForType(NotificationType type)
    {
        return type switch
        {
            NotificationType.Success => successIcon,
            NotificationType.Error => errorIcon,
            NotificationType.Info => infoIcon,
            _ => null
        };
    } 
}
