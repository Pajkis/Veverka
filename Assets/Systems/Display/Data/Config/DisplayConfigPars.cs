
using UnityEngine;
/// <summary>
/// Represents the configuration parameters for display settings, including platform mapping, UI scaling, aspect ratio,
/// orientation, performance, safe area handling, camera settings, grid settings, and UI layout positions.
/// </summary>
/// <remarks>This class provides a comprehensive set of parameters to configure display-related settings for an
/// application. It includes options for platform-specific adjustments, UI scaling preferences, aspect ratio management,
/// orientation enforcement, performance tuning, safe area application, camera behavior, grid layout, and UI button
/// positioning.</remarks>
[System.Serializable]
public class DisplayConfigPars
{
    [Header("Platform mapping")]
    public DevicePlatformType platform = DevicePlatformType.Auto;

    [Header("UI scaling")]
    public Vector2Int referenceResolution = new Vector2Int(1920, 1080);
    [Range(0f, 1f)] public float uiMatchWidthOrHeight = 1f; // 0=width, 1 = height, 0.5 = both equally

    [Header("Aspect & Bars")]
    public bool useLetterboxing = true;
    public Vector2 targetAspect = new Vector2(16, 9); // 16:9 aspect ratio

    [Header("Orientation")]
    public bool forceOrientation = true;
    public ScreenOrientation orientation = ScreenOrientation.LandscapeLeft;

    [Header("Performance")]
    [Tooltip("0 = do not touch Application.targetFrameRate")]
    public int targetFrameRate = 60;
    [Tooltip("0..4; 0 = no VSync (controls targetFrameRate)")]
    public int vSyncCount = 0;

    [Header("Safe Area")]
    public bool applySafeArea = true;

    [Header("Auto-Apply")]
    [Tooltip("re-apply after new screen is loaded")]
    public bool autoApplyOnSceneLoaded = true;

    // === GAME CAMERA SETTINGS ===
    [Header("Camera Settings")]
    [Tooltip("Camera smooth follow time")]
    public float cameraFollowTime = 0.2f;
    [Tooltip("Camera follow offset when character moves")]
    public Vector3 cameraFollowOffset = new Vector3(2.6666f, 0, -10f);
    [Tooltip("Screen offset for dual-screen layout")]
    public Vector3 screenToPlayOffset = new Vector3(2.1666f, -0.5f, -10f);
    [Tooltip("Offset for even tile count grids")]
    public float evenTilesOffset = 0.5f;

    // === GRID & TILE SETTINGS ===
    [Header("Grid Settings")]
    [Tooltip("Size of each tile in world units")]
    public float tileSize = 1f;
    [Tooltip("Max grid size before camera becomes static")]
    public Vector2Int maxStaticScreenSize = new Vector2Int(15, 11);

    // === UI LAYOUT POSITIONS ===
    [Header("UI Layout - Button Columns")]
    [Tooltip("Left column button X position for this platform")]
    public float leftColumnButtonX = 585f;
    [Tooltip("Middle column button X position for this platform")]
    public float middleColumnButtonX = 720f;
    [Tooltip("Right column button X position for this platform")]
    public float rightColumnButtonX = 855f;

    [Header("UI Layout - Button Rows")]
    [Tooltip("Top row button Y position for this platform")]
    public float topRowButtonY = -155f;
    [Tooltip("Middle row button Y position for this platform")]
    public float middleRowButtonY = -290f;
    [Tooltip("Bottom row button Y position for this platform")]
    public float bottomRowButtonY = -425f;
    [Tooltip("Android specific middle row position")]
    public float androidMiddleRowButtonY = -225f;
}
