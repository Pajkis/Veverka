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

            Vector2 gridSizeInWorld = new Vector2(gridSize.x * tileSize, gridSize.y * tileSize);
            Vector2 halfCam = new Vector2(camWidth, camHeight) / 2f;

            Vector3 gridOrigin = GameGrid.Instance.transform.position;

            minBounds = gridOrigin + (Vector3)halfCam;
            maxBounds = gridOrigin + (Vector3)(gridSizeInWorld - halfCam);

            initialized = true;
        }



        /// <summary>
        /// Late update is called every frame if Behaviour is enabled
        /// </summary>
        private void LateUpdate()
        {
            if (!initialized)
            {               
                if (!initialized) return;
            }

            if (target == null) return;

            //Vector3 desired = target.position + offset;
            //Vector3 clamped = ClampToBounds(desired);
            //transform.position = Vector3.Lerp(transform.position, clamped, smoothSpeed);

            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed);
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