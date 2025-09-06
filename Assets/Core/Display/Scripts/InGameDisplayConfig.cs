using UnityEngine;

[CreateAssetMenu(fileName = "InGameDisplayConfig", menuName = "Configs/InGameDisplayConfig")]
public class InGameDisplayConfig : ScriptableObject
{
    [Header("Grid & Tile Settings")]
    // Max number of tiles so the camera is static (does not move with character)
    public Vector2Int MaxStatisScreenSize = new Vector2Int(15, 11);
    public float tileSize = 1f;

    [Header("Camera Settings")]
    // adjust camera follow of character when it crosses over half the screen
    public Vector3 FollowOffset = new Vector3(2.6666f, 0, -10f);
    // offset to switch center of the scene in the center of the left screen window
    public Vector3 ScreenToPlayScreenOffset = new Vector3(2.1666f, -0.5f, -10f);
    // camera smooth follow time -> smoothspeed = tilesize / smoothmovetime
    public float smoothMoveTime = 0.2f;

    [Header("UI Layout Style Settings")]
    public float LeftColumnButtonPosX = 585f;
    public float MiddleColumnButtonPosX = 720f;
    public float RightColumnButtonPosX = 855f;

    public float BottomRowButtonPosY = -425f;
    public float MiddleRowButtonPosY = -290f;
    public float TopRowButtonPosY = -155f;

    public float AndroidMiddleRowButtonPosY = -225f;
}
