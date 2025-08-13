using UnityEngine;
using Veverka.GridSystem.GameGrid;

namespace Veverka.CameraSystem
{
    /// <summary>
    /// Camera follows target and clamps within GameGrid bounds.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Config (SO)")]
        [SerializeField] private InGameDisplayConfig inGameConfig; 

        #region fields
        [Header("Follow Settings")]
        [SerializeField] private Transform target;
        // camera smooth follow time -> smoothspeed = tilesize / smoothmovetime
        [SerializeField] private float smoothMoveTime = 0.2f; 
        // adjust camera follow of character when it crosses over half the screen
        [SerializeField] private Vector3 FollowOffset = new Vector3(2.6666f, 0, -10f);
        // offset to switch center of the scene in the center of the left screen window
        [SerializeField] private Vector3 ScreenToPlayScreenOffset = new Vector3(2.1666f, -0.5f, -10f);
                
        [Header("Tile Settings")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private Vector2Int maxStaticTileSize = new Vector2Int(15, 11);

        [Header("Camera bounds")]
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;
        [SerializeField] private float evenTilesOffset = 0.5f;

        private bool initialized = false;

        private CameraMode mode;

        #endregion

        #region methods
        /// <summary>
        /// awake is called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            ApplyConfig();
        }

#if UNITY_EDITOR
        /// <summary>
        /// called when the script is loaded or a value is changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            // changes in editor are applied immediately
            if (!Application.isPlaying)
                ApplyConfig();
        }
#endif
        /// <summary>
        /// apply configuration from InGameDisplayConfigSO
        /// </summary>
        private void ApplyConfig()
        {
            if (inGameConfig == null) return;

            smoothMoveTime = inGameConfig.smoothMoveTime;
            FollowOffset = inGameConfig.FollowOffset;
            ScreenToPlayScreenOffset = inGameConfig.ScreenToPlayScreenOffset;
            tileSize = inGameConfig.tileSize;
            maxStaticTileSize = inGameConfig.MaxStatisScreenSize;
        }

        /// <summary>
        /// Initialize Camera and centers it on set target
        /// </summary>
        /// <param name="target">set target of camera to follow</param>
        /// <param name="gridSize">size of the grid</param>
        public void Init(Transform target, Vector2Int gridSize)
        {
            this.target = target;

            Vector3 gridOrigin = GameGrid.Instance.transform.position;

            Vector2 levelSize = new Vector2(gridSize.x * tileSize, gridSize.y * tileSize);
            Vector2 visibleSize = new Vector2(maxStaticTileSize.x * tileSize, maxStaticTileSize.y * tileSize);
            Vector2 halfVisibleSize = visibleSize / 2f;

            minBounds = gridOrigin + (Vector3)halfVisibleSize + ScreenToPlayScreenOffset;
            maxBounds = gridOrigin + (Vector3)levelSize - (Vector3)halfVisibleSize + ScreenToPlayScreenOffset;

            // Camera mode setup
            if (gridSize.x <= maxStaticTileSize.x && gridSize.y <= maxStaticTileSize.y)
            {
                mode = CameraMode.StaticCenter;
                Vector3 center = gridOrigin + (Vector3)(levelSize / 2f);
              
                // adjust to even tile numbered grid
                if ((gridSize.x % 2) == 0)
                {
                    center += new Vector3(evenTilesOffset, 0, 0);
                }

                if ((gridSize.y % 2) == 0)
                {
                    center += new Vector3(0, evenTilesOffset, 0);
                }

                transform.position = center + ScreenToPlayScreenOffset;
            }
            else
            {
                mode = CameraMode.FollowWithClamp;
                initialized = true;
            }
        }

        /// <summary>
        /// Late update is called every frame if Behaviour is enabled
        /// </summary>
        private void LateUpdate()
        {
            if (mode == CameraMode.StaticCenter) return;
            if (!initialized || target == null) return;
            
            Vector3 desired = target.position + FollowOffset;
            Vector3 clamped = ClampToBounds(desired);
            transform.position = Vector3.Lerp(transform.position, clamped, smoothMoveTime);
        }

        /// <summary>
        /// Clamp bounds to the size of grid
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        private Vector3 ClampToBounds(Vector3 targetPos)
        {
          
            float clampedX = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
           
            return new Vector3(clampedX, clampedY, targetPos.z);         
        }

        #endregion
    }
}