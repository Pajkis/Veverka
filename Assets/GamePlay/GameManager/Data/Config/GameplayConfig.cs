
using UnityEngine;

/// <summary>
/// Gameplay configuration settings for the game.
/// </summary>
[CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/GameplayConfig")]
public class GameplayConfig : ScriptableObject
{
    [Header("Characters")]
    [Tooltip("Time for character movement animation")]
    public float characterMoveTime = 0.15f;
    [Tooltip("Time for character rotation animation")]
    public float characterRotateTime = 0.15f;

    [Header("Tiles")]
    [Tooltip("Fallback move time for nuts when not provided by character")]
    public float nutMoveTime = 0.15f;
    [Tooltip("Animation time for tile transformations (splash, goal->stone, hole->road)")]
    public float tileAnimationTime = 0.15f;
    [Tooltip("Time for splash push effect on nuts")]
    public float splashPushTime = 0.25f;

    [Header("Undo")]
    [Tooltip("Maximum number of undo steps (not implemented yet)")]
    public int maxUndoSteps = 100;
    [Tooltip("Time for each undo step animation")]
    public float undoStepTime = 0.15f;
    
    [Header("UI/Input")]
    [Tooltip("Input debounce time to prevent double input")]
    public float inputDebounceTime = 0.1f;
    [Tooltip("Character trigger timeout in TurnControl")]
    public float characterTriggerTimeout = 0.5f;

    [Header("BubbleMessage")]
    [Tooltip("Display time for character bubble messages")]
    public float characterBubbleTime = 2.0f;
    [Tooltip("Display time for goal bubble messages")]
    public float goalBubbleTime = 1.0f;

    [Header("Scoring")]
    public int reduceGoalCount = 1;

    [Header("PopoutImage")]
    [Tooltip("Default direction for popout image animation")]
    public Direction popoutImageDefaultDirection = Direction.Up;

    [Tooltip("Default distance for popout image to travel in world units")]
    public float popoutImageDefaultDistance = 1.0f;

    [Tooltip("Default duration for entire popout animation")]
    public float popoutImageDefaultDuration = 1.0f;

    [Tooltip("Time to start fading (0 = fade immediately, duration = fade at end)")]
    public float popoutImageDefaultFadeStartTime = 0.5f;

}