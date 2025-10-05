using UnityEngine;

/// <summary>
/// Centralized debug logging configuration for all game systems.
/// Create via Assets > Create > Debug > Debug Log Config
/// </summary>
[CreateAssetMenu(fileName = "DebugLogConfig", menuName = "Debug/Debug Log Config")]
public class DebugLogConfig : ScriptableObject 
{
    [Header("Core Gameplay")]
    [Tooltip("Enable gameplay system logging")]
    public bool gameplayLog = false;

    [Tooltip("Enable detailed turn control flow logging")]
    public bool turnControlLog = false;

    [Tooltip("Enable turn record and history logging")]
    public bool turnRecordLog = false;

    [Tooltip("Enable undo logic and operations logging")]
    public bool undoLogicLog = false;

    [Tooltip("Enable bubble message system logging")]
    public bool bubbleMessageLog = false;

    [Header("Movement & Physics")]
    [Tooltip("Enable character movement and rotation logging")]
    public bool characterMovementLog = false;

    [Tooltip("Enable nut/tile movement logging")]
    public bool nutMovementLog = false;

    [Tooltip("Enable rotation animation logging")]
    public bool rotationLog = false;

    [Header("Grid & Tiles")]
    [Tooltip("Enable grid system and queries logging")]
    public bool gridSystemLog = false;

    [Tooltip("Enable tile interaction logging")]
    public bool tileInteractionLog = false;

    [Tooltip("Enable goal system logging")]
    public bool goalSystemLog = false;

    [Header("Level Loading")]
    [Tooltip("Enable level system logging")]
    public bool levelSystemLog = false;

    [Header("User Interface")]
    [Tooltip("Enable UI system logging")]
    public bool uiLog = false;

    [Tooltip("Enable input handling logging")]
    public bool inputLog = false;

    [Tooltip("Enable scene manager logging")]
    public bool sceneManagerLog = false;

    [Header("Systems & Services")]
    [Tooltip("Enable event system flow logging")]
    public bool eventSystemLog = false;

    [Tooltip("Enable settings system logging")]
    public bool settingsLog = false;

    [Tooltip("Enable audio system logging")]
    public bool audioLog = false;

    [Header("Debug Utilities")]
    [Tooltip("Enable all debug logs at once (overrides individual settings)")]
    public bool enableAllLogs = false;
    
    [Tooltip("Use colored debug messages for better readability")]
    public bool useColoredLogs = true;
    
    /// <summary>
    /// Check if a specific log category is enabled
    /// </summary>
    public bool IsLogEnabled(DebugLogCategory category)
    {
        if (enableAllLogs) return true;

        return category switch
        {
            DebugLogCategory.TurnControl => turnControlLog,
            DebugLogCategory.TurnRecord => turnRecordLog,
            DebugLogCategory.UndoLogic => undoLogicLog,
            DebugLogCategory.CharacterMovement => characterMovementLog,
            DebugLogCategory.NutMovement => nutMovementLog,
            DebugLogCategory.Rotation => rotationLog,
            DebugLogCategory.EventSystem => eventSystemLog,
            DebugLogCategory.Audio => audioLog,
            DebugLogCategory.GridSystem => gridSystemLog,
            DebugLogCategory.TileInteraction => tileInteractionLog,
            DebugLogCategory.Input => inputLog,
            DebugLogCategory.UI => uiLog,
            DebugLogCategory.Settings => settingsLog,
            DebugLogCategory.Gameplay => gameplayLog,
            DebugLogCategory.LevelSystem => levelSystemLog,
            DebugLogCategory.BubbleMessage => bubbleMessageLog,
            DebugLogCategory.GoalSystem => goalSystemLog,
            DebugLogCategory.SceneManager => sceneManagerLog,
            _ => false
        };
    }

    /// <summary>
    /// Get color for debug category (if colored logs enabled)
    /// </summary>
    public string GetCategoryColor(DebugLogCategory category)
    {
        if (!useColoredLogs) return "";

        return category switch
        {
            DebugLogCategory.TurnControl => "#00FF00", // Green
            DebugLogCategory.TurnRecord => "#FFA500",  // Orange
            DebugLogCategory.UndoLogic => "#FF69B4",   // Pink
            DebugLogCategory.CharacterMovement => "#87CEEB", // Sky Blue
            DebugLogCategory.NutMovement => "#DDA0DD", // Plum
            DebugLogCategory.Rotation => "#98FB98",    // Pale Green
            DebugLogCategory.EventSystem => "#FFD700", // Gold
            DebugLogCategory.Audio => "#FF6347",       // Tomato
            DebugLogCategory.GridSystem => "#40E0D0",  // Turquoise
            DebugLogCategory.TileInteraction => "#EE82EE", // Violet
            DebugLogCategory.Input => "#32CD32",       // Lime Green
            DebugLogCategory.UI => "#FF1493",          // Deep Pink
            DebugLogCategory.Settings => "#FFB6C1",    // Light Pink
            DebugLogCategory.Gameplay => "#98FB98",    // Pale Green
            DebugLogCategory.LevelSystem => "#00CED1", // Dark Turquoise
            DebugLogCategory.BubbleMessage => "#FF69B4", // Hot Pink
            DebugLogCategory.GoalSystem => "#FFD700",  // Gold
            DebugLogCategory.SceneManager => "#9370DB", // Medium Purple
            _ => "#FFFFFF" // White
        };
    }
}

/// <summary>
/// Debug log categories for organized logging
/// </summary>
public enum DebugLogCategory
{
    TurnControl,
    TurnRecord,
    UndoLogic,
    CharacterMovement,
    NutMovement,
    Rotation,
    EventSystem,
    Audio,
    GridSystem,
    TileInteraction,
    Input,
    UI,
    Settings,
    Gameplay,
    LevelSystem,
    BubbleMessage,
    GoalSystem,
    SceneManager
}