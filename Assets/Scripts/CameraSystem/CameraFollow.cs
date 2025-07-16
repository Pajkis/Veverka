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
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

        [Header("Tile Settings")]
        [SerializeField] private float tileSize = 1f;
        
        private float camHeight;
        private float camWidth;

        private Vector2 minBounds;
        private Vector2 maxBounds;

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

            Camera cam = Camera.main;
            camHeight = 2f * cam.orthographicSize;
            camWidth = camHeight * cam.aspect;

            Vector2 levelSize = new Vector2(gridSize.x * tileSize, gridSize.y * tileSize);

            Vector3 gridOrigin = GameGrid.Instance.transform.position;

            minBounds = gridOrigin + new Vector3(camWidth / 2f, camHeight / 2f);
            maxBounds = gridOrigin + (Vector3)(levelSize - new Vector2(camWidth / 2f, camHeight / 2f));

            // Rozhodni režim podle velikosti gridu vs. kamery
            if (levelSize.x <= camWidth && levelSize.y <= camHeight)
            {
                mode = CameraMode.StaticCenter;
                // Umísti kameru doprost?ed gridu
                Vector3 center = gridOrigin + (Vector3)(levelSize / 2f);
                transform.position = center + offset;
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
            
            Vector3 desired = target.position + offset;
            Vector3 clamped = ClampToBounds(desired);
           transform.position = Vector3.Lerp(transform.position, clamped, smoothSpeed);
            
          //  transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed);
        }

        /// <summary>
        /// Clamp bounds to the size of grid
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        private Vector3 ClampToBounds(Vector3 targetPos)
        {
            Vector2Int gridSize = GameGrid.Instance.GridSize;

            float halfWidth = camWidth / 2f;
            float halfHeight = camHeight / 2f;

            float minX = tileSize * halfWidth;
            float maxX = tileSize * (gridSize.x - halfWidth);

            float minY = tileSize * halfHeight;
            float maxY = tileSize * (gridSize.y - halfHeight);

            float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

            return new Vector3(clampedX, clampedY, targetPos.z);         
        }
    }
}