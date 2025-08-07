
using UnityEngine;

[CreateAssetMenu(fileName = "DisplayConfig", menuName = "Configs/Display Config")]
public class DisplayConfigSO : ScriptableObject
{
    [Header("Grid & Tile Settings")]
    // Max number of tiles so the camera is static (does not move with character)
    public Vector2Int MaxStatisScreenSize = new Vector2Int(15, 11);
    public float tileSize = 1f;

    [Header("Resolution")]
    public Vector2Int referenceResolution = new(1920, 1080);
    public float aspectRatio => (float)referenceResolution.x / referenceResolution.y;

    [Header("Camera Settings")]
    // adjust camera follow of character when it crosses over half the screen
    public Vector3 FollowOffset = new Vector3(2.6666f, 0, -10f);
    // offset to switch center of the scene in the center of the left screen window
    public Vector3 ScreenToPlayScreenOffset = new Vector3(2.1666f, -0.5f, -10f);
    // camera smooth follow time -> smoothspeed = tilesize / smoothmovetime
    public float smoothMoveTime = 0.3f;

    [Header("UI Scaling")]
    public float canvasScaleFactor = 1f;

    [Header("Platform Specifics (Optional)")]
    public bool isMobile = false;
   
}