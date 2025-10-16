using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Overlay that displays up to MAX_VALIDATION_ERRORS level validation errors
/// Shows in LevelSelect scene when validation fails
/// </summary>
public class LevelValidationErrorOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI[] errorTexts;

    void Start()
    {
        DisplayErrors();
    }

    private void DisplayErrors()
    {
        List<string> errors = LevelValidationErrorManager.GetErrors();

        if (errors.Count == 0)
        {
            DebugLogger.LogError(DebugLogCategory.UI, "LevelValidationErrorOverlay: No errors to display!");
            return;
        }

        // Validate array size
        if (errorTexts == null || errorTexts.Length < ErrorMessages.MAX_VALIDATION_ERRORS)
        {
            DebugLogger.LogError(DebugLogCategory.UI, $"LevelValidationErrorOverlay: errorTexts array must have {ErrorMessages.MAX_VALIDATION_ERRORS} elements!");
            return;
        }

        // Display errors using loop
        for (int i = 0; i < ErrorMessages.MAX_VALIDATION_ERRORS; i++)
        {
            if (errorTexts[i] != null)
            {
                if (i < errors.Count)
                {
                    errorTexts[i].text = errors[i];
                    errorTexts[i].gameObject.SetActive(true);
                }
                else
                {
                    errorTexts[i].text = "";
                    errorTexts[i].gameObject.SetActive(false);
                }
            }
            else
            {
                DebugLogger.LogWarning(DebugLogCategory.UI, $"LevelValidationErrorOverlay: errorTexts[{i}] is null!");
            }
        }

        DebugLogger.Log(DebugLogCategory.UI, $"LevelValidationErrorOverlay displayed {errors.Count} error(s)");
    }
}
