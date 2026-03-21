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
    [SerializeField] private TutorialLoader tutorialLoader;
    [SerializeField] private TutorialActionSequencer tutorialActionSequencer;
    [SerializeField] private TutorialSettingsConfig tutorialSettingsConfig;

    [Header("Buttons")]
    [SerializeField] private Button autoplayToggleButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button textSpeedButton;
    [SerializeField] private Button animSpeedButton;
    [SerializeField] private Button soundButton;

    [Header("Display Labels")]
    [SerializeField] private TextMeshProUGUI modeDisplayText;
    [SerializeField] private TextMeshProUGUI textSpeedLabel;
    [SerializeField] private TextMeshProUGUI animSpeedLabel;
    [SerializeField] private TextMeshProUGUI soundLabel;

    private void Awake()
    {
        // Hook up button onClick listeners
        if (autoplayToggleButton != null)
            autoplayToggleButton.onClick.AddListener(OnAutoplayToggleClicked);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (textSpeedButton != null)
            textSpeedButton.onClick.AddListener(OnTextSpeedClicked);

        if (animSpeedButton != null)
            animSpeedButton.onClick.AddListener(OnAnimSpeedClicked);

        if (soundButton != null)
            soundButton.onClick.AddListener(OnSoundToggleClicked);
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
    /// Restart tutorial from beginning.
    /// </summary>
    private void OnRestartClicked()
    {
        DebugLogger.Log(DebugLogCategory.Tutorial, $"OnRestartClicked: tutorialLoader={tutorialLoader != null}", this);
        if (tutorialLoader != null)
        {
            tutorialLoader.Restart();
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialLoader not assigned in TutorialControlPanel!", this);
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
    /// Toggle tutorial sound effects on/off.
    /// </summary>
    private void OnSoundToggleClicked()
    {
        if (tutorialSettingsConfig != null)
        {
            tutorialSettingsConfig.TutorialSetSoundEnabled(!tutorialSettingsConfig.SoundEnabled);
            tutorialLoader?.ApplySoundSetting();
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
        if (tutorialSettingsConfig == null) return;

        // Update mode display
        if (modeDisplayText != null)
        {
            bool isAutoplay = tutorialSettingsConfig.AutoplayEnable;
            modeDisplayText.text = isAutoplay ? "Auto" : "Man";
        }

        // Update text speed label
        if (textSpeedLabel != null)
        {
            TutorialAnimSpeedType textSpeed = tutorialSettingsConfig.TextSpeedIndex;
            textSpeedLabel.text = $"{SpeedToString(textSpeed)}";
        }

        // Update animation speed label
        if (animSpeedLabel != null)
        {
            TutorialAnimSpeedType animSpeed = tutorialSettingsConfig.AnimSpeedIndex;
            animSpeedLabel.text = $"{SpeedToString(animSpeed)}";
        }

        // Update sound label
        if (soundLabel != null)
        {
            soundLabel.text = tutorialSettingsConfig.SoundEnabled ? "ON" : "OFF";
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
