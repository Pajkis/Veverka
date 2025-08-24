using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LayoutStyleManager is responsible for managing the layout style of the UI elements.
/// </summary>
public class LayoutStyleManager : MonoBehaviour
{
    #region Fields
    [Header("UI Button bidnings")]
    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;
    [SerializeField] private Button buttonUp;
    [SerializeField] private Button buttonDown;

    [SerializeField] private Button undo;
    [SerializeField] private Button action;

    [Header("Events ")]
    [SerializeField] private LayoutStyleChangeEvent layoutStyleChangeEvent;


    [Header("Configs ")]
    [SerializeField] private InGameDisplayConfig inGameDisplayConfig;

    private LayoutStyleTypes currentLayoutStyle;

    private float leftColumnButtonPosX;
    private float middleColumnButtonPosX;
    private float rightColumnButtonPosX;
    private float bottomRowButtonPosY;
    private float middleRowButtonPosY;
    private float topRowButtonPosY;
    private float androidMiddleRowButtonPosY;
    Transform buttonTransform;
    #endregion

    #region Initialization
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Get the current layout style from GameSettings.
    /// </summary>
    void Awake()
    {
        if (GameSettings.Instance == null)
        {
            Debug.LogError($"[{nameof(LayoutStyleManager)}] GameSettings instance not found in scene.");
            enabled = false;
            return;
        }
        currentLayoutStyle = (LayoutStyleTypes)GameSettings.Instance.Get(GameSettingsEnum.LayoutStyle);
        Debug.Log($"[{nameof(LayoutStyleManager)}] Current layout style: {currentLayoutStyle}");

        if (inGameDisplayConfig == null)
        {
            Debug.LogError($"[{nameof(LayoutStyleManager)}] InGameDisplayConfig not set in Inspector.");
            return;
        }

        // Load button positions from config
        leftColumnButtonPosX = inGameDisplayConfig.LeftColumnButtonPosX;
        middleColumnButtonPosX = inGameDisplayConfig.MiddleColumnButtonPosX;
        rightColumnButtonPosX = inGameDisplayConfig.RightColumnButtonPosX;
        bottomRowButtonPosY = inGameDisplayConfig.BottomRowButtonPosY;
        middleRowButtonPosY = inGameDisplayConfig.MiddleRowButtonPosY;
        topRowButtonPosY = inGameDisplayConfig.TopRowButtonPosY;
        androidMiddleRowButtonPosY = inGameDisplayConfig.AndroidMiddleRowButtonPosY;

        // Set initial layout
        setLayout();
    }

    /// <summary>
    /// Add listener to the layout style change event.
    /// </summary>
    private void OnEnable()
    {
        layoutStyleChangeEvent.AddListener(onLayoutStyleChange);
    }

    /// <summary>
    /// remove listener to the layout style change event.
    /// </summary>
    private void OnDisable()
    {
        layoutStyleChangeEvent.RemoveListener(onLayoutStyleChange);
    }

    #endregion

    #region layout style methods
    /// <summary>
    /// Apply the layout style to the UI elements.
    /// </summary>
    /// <param name="newLayoutStyle"></param>
    private void onLayoutStyleChange(LayoutStyleTypes newLayoutStyle)
    {
        if (currentLayoutStyle != newLayoutStyle)
        {
            currentLayoutStyle = newLayoutStyle;
            Debug.Log($"[{nameof(LayoutStyleManager)}] Layout style changed to: {currentLayoutStyle}");

            setLayout();
        }
    }

    /// <summary>
    /// Set the layout based on the current layout style.
    /// </summary>
    private void setLayout()
    {
        switch (currentLayoutStyle)
        {
            case LayoutStyleTypes.WindowsRight:
                windowslayoutSet();
                break;
            case LayoutStyleTypes.AndroidRight:
                androidLayoutSet();
                break;
            default:
                Debug.LogWarning($"[{nameof(LayoutStyleManager)}] Unknown layout style: {currentLayoutStyle}");
                break;
        }
    }

    /// <summary>
    ///  Windows layout set
    /// </summary>
    private void windowslayoutSet()
    {
        //left button
        buttonTransform = buttonLeft.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(leftColumnButtonPosX, bottomRowButtonPosY, 0);

        //right button
        buttonTransform = buttonRight.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(rightColumnButtonPosX, bottomRowButtonPosY, 0);

        //up button
        buttonTransform = buttonUp.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(middleColumnButtonPosX, middleRowButtonPosY, 0);

        //down button
        buttonTransform = buttonDown.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(middleColumnButtonPosX, bottomRowButtonPosY, 0);

        //undo button
        buttonTransform = undo.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(leftColumnButtonPosX, topRowButtonPosY, 0);

        //action button
        buttonTransform = action.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(rightColumnButtonPosX, topRowButtonPosY, 0);
    }

    /// <summary>
    /// Android layout set
    /// </summary>
    private void androidLayoutSet()
    {
        //left button
        buttonTransform = buttonLeft.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(leftColumnButtonPosX, androidMiddleRowButtonPosY, 0);

        //right button
        buttonTransform = buttonRight.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(rightColumnButtonPosX, androidMiddleRowButtonPosY, 0);

        //up button
        buttonTransform = buttonUp.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(middleColumnButtonPosX, topRowButtonPosY, 0);

        //down button
        buttonTransform = buttonDown.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(middleColumnButtonPosX, middleRowButtonPosY, 0);

        //undo button
        buttonTransform = undo.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(leftColumnButtonPosX, bottomRowButtonPosY, 0);

        //action button
        buttonTransform = action.GetComponent<RectTransform>();
        buttonTransform.localPosition = new Vector3(rightColumnButtonPosX, bottomRowButtonPosY, 0);
    }
    #endregion
}