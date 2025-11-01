using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages level validation errors during level loading
/// Collects up to 3 errors and displays them in LevelValidationErrorOverlay
/// </summary>
public class LevelValidationErrorManager : MonoBehaviour
{
    private static LevelValidationErrorManager instance;

    [Header("Events")]
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;
    [SerializeField] private AudioEvents audioEvents;

    private static List<string> currentErrors = new List<string>();
    private static bool shouldShowValidationErrors = false;

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
    /// Clear all collected errors (call at start of validation)
    /// </summary>
    public static void ClearErrors()
    {
        currentErrors.Clear();
        shouldShowValidationErrors = false;
    }

    /// <summary>
    /// Add a level validation error message
    /// Only stores up to ErrorMessages.MAX_VALIDATION_ERRORS errors
    /// </summary>
    public static void AddError(string errorMessage)
    {
        if (currentErrors.Count < ErrorMessages.MAX_VALIDATION_ERRORS)
        {
            currentErrors.Add(errorMessage);
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"LevelValidationError added: {errorMessage}");
        }
    }

    /// <summary>
    /// Get current collected errors
    /// </summary>
    public static List<string> GetErrors()
    {
        return new List<string>(currentErrors);
    }

    /// <summary>
    /// Check if there are any errors
    /// </summary>
    public static bool HasErrors()
    {
        return currentErrors.Count > 0;
    }

    /// <summary>
    /// Mark that validation error overlay should be shown on next scene load (LevelSelect)
    /// Call this when validation fails before scene transition
    /// </summary>
    public static void MarkForDisplay()
    {
        if (HasErrors())
        {
            shouldShowValidationErrors = true;
            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Marked {currentErrors.Count} validation error(s) for display in LevelSelect");
        }
    }

    /// <summary>
    /// Check if validation error overlay should be shown and display it
    /// Called by LevelSelect scene on Start()
    /// </summary>
    public static void ShowErrorOverlayIfPending()
    {
        if (!shouldShowValidationErrors)
        {
            return;
        }

        if (instance == null)
        {
            Debug.LogError("LevelValidationErrorManager instance not found!");
            return;
        }

        if (!HasErrors())
        {
            DebugLogger.LogWarning(DebugLogCategory.LevelSystem, "Validation errors marked for display but no errors found");
            shouldShowValidationErrors = false;
            return;
        }

        // Play error sound
        if (instance.audioEvents != null)
        {
            instance.audioEvents.Raise(new AudioEventPayload
            {
                EventType = AudioEventType.PlayUi,
                Ui = UiType.Error
            });
        }

        if (instance.sceneNavigationEvents != null)
        {
            instance.sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.OpenOverlay,
                Overlay = OverlayType.LevelValidationError
            });

            DebugLogger.Log(DebugLogCategory.LevelSystem, $"Showing level validation error overlay with {currentErrors.Count} error(s)");
            shouldShowValidationErrors = false; // Reset flag after showing
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.LevelSystem, "SceneNavigationEvents not assigned in LevelValidationErrorManager!");
        }
    }
}
