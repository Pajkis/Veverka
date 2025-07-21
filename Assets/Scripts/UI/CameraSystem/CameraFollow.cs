using UnityEngine;
using Veverka.GridSystem.GameGrid;

namespace Veverka.CameraSystem
{
    /// <summary>
    /// Camera follows target and clamps within GameGrid bounds.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 0.15f;
        // adjust camera follow of character when it crosses over half the screen
        [SerializeField] private Vector3 FollowOffset = new Vector3(2.6666f, 0, -10f);
        // offset to switch center of the scene in the center of the left screen window
        [SerializeField] private Vector3 ScreenToPlayScreenOffset = new Vector3(2.1666f, -0.5f, -10f);
                
        [Header("Tile Settings")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private Vector2Int maxStaticTileSize = new Vector2Int(15, 11);
  

        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private bool initialized = false;

        private CameraMode mode;

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
           transform.position = Vector3.Lerp(transform.position, clamped, smoothSpeed);
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
    }
}