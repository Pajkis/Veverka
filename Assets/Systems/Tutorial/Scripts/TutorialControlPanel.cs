using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI control panel for tutorial playback controls.
/// Handles autoplay toggle, speed controls, and manual mode Next button.
/// </summary>
public class TutorialControlPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialActionSequencer tutorialActionSequencer;

    [Header("Buttons")]
    [SerializeField] private Button autoplayToggleButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button textSpeedButton;
    [SerializeField] private Button animSpeedButton;

    [Header("Display Labels")]
    [SerializeField] private TextMeshProUGUI modeDisplayText;
    [SerializeField] private TextMeshProUGUI textSpeedLabel;
    [SerializeField] private TextMeshProUGUI animSpeedLabel;

    private void Awake()
    {
        // Hook up button onClick listeners
        if (autoplayToggleButton != null)
            autoplayToggleButton.onClick.AddListener(OnAutoplayToggleClicked);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        if (textSpeedButton != null)
            textSpeedButton.onClick.AddListener(OnTextSpeedClicked);

        if (animSpeedButton != null)
            animSpeedButton.onClick.AddListener(OnAnimSpeedClicked);
    }

    private void OnEnable()
    {
        // Refresh UI from TutorialSettingsConfig
        RefreshUI();
    }

    private void Update()
    {
        // Update Next button state based on manual mode and sequencer state
        if (nextButton != null && tutorialActionSequencer != null)
        {
            bool shouldBeActive = tutorialActionSequencer.IsNextButtonActive();
            nextButton.interactable = shouldBeActive;
        }
    }

    /// <summary>
    /// Toggle autoplay mode between Auto and Manual.
    /// </summary>
    private void OnAutoplayToggleClicked()
    {
        if (tutorialActionSequencer != null)
        {
            tutorialActionSequencer.ToggleAutoplay();
            RefreshUI();
        }
    }

    /// <summary>
    /// Request next action in manual mode.
    /// </summary>
    private void OnNextClicked()
    {
        if (tutorialActionSequencer != null)
        {
            tutorialActionSequencer.RequestNext();
        }
    }

    /// <summary>
    /// Cycle text speed: Normal → Fast → VeryFast → Normal.
    /// </summary>
    private void OnTextSpeedClicked()
    {
        if (tutorialActionSequencer != null)
        {
            TutorialAnimSpeedType currentSpeed = tutorialActionSequencer.GetCurrentTextSpeed();
            TutorialAnimSpeedType newSpeed = CycleSpeed(currentSpeed);
            tutorialActionSequencer.SetTextSpeed(newSpeed);
            RefreshUI();
        }
    }

    /// <summary>
    /// Cycle animation speed: Normal → Fast → VeryFast → Normal.
    /// </summary>
    private void OnAnimSpeedClicked()
    {
        if (tutorialActionSequencer != null)
        {
            TutorialAnimSpeedType currentSpeed = tutorialActionSequencer.GetCurrentAnimSpeed();
            TutorialAnimSpeedType newSpeed = CycleSpeed(currentSpeed);
            tutorialActionSequencer.SetAnimSpeed(newSpeed);
            RefreshUI();
        }
    }

    /// <summary>
    /// Refresh UI labels from TutorialSettingsConfig.
    /// </summary>
    private void RefreshUI()
    {
        if (TutorialSettingsConfig.Instance == null) return;

        // Update mode display
        if (modeDisplayText != null)
        {
            bool isAutoplay = TutorialSettingsConfig.Instance.AutoplayEnable;
            modeDisplayText.text = isAutoplay ? "Mode: Auto" : "Mode: Manual";
        }

        // Update text speed label
        if (textSpeedLabel != null)
        {
            TutorialAnimSpeedType textSpeed = TutorialSettingsConfig.Instance.TextSpeedIndex;
            textSpeedLabel.text = $"Text: {SpeedToString(textSpeed)}";
        }

        // Update animation speed label
        if (animSpeedLabel != null)
        {
            TutorialAnimSpeedType animSpeed = TutorialSettingsConfig.Instance.AnimSpeedIndex;
            animSpeedLabel.text = $"Anim: {SpeedToString(animSpeed)}";
        }
    }

    /// <summary>
    /// Cycle speed to next value.
    /// </summary>
    private TutorialAnimSpeedType CycleSpeed(TutorialAnimSpeedType current)
    {
        return current switch
        {
            TutorialAnimSpeedType.Normal => TutorialAnimSpeedType.Fast,
            TutorialAnimSpeedType.Fast => TutorialAnimSpeedType.VeryFast,
            TutorialAnimSpeedType.VeryFast => TutorialAnimSpeedType.Normal,
            _ => TutorialAnimSpeedType.Normal
        };
    }

    /// <summary>
    /// Convert speed enum to display string.
    /// </summary>
    private string SpeedToString(TutorialAnimSpeedType speed)
    {
        return speed switch
        {
            TutorialAnimSpeedType.Normal => "1x",
            TutorialAnimSpeedType.Fast => "3x",
            TutorialAnimSpeedType.VeryFast => "5x",
            _ => "1x"
        };
    }
}
