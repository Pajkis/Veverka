using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Execution of Smooth movement, backcalling OnStart and OnComplete methods
/// </summary>
public class SmoothRotate : MonoBehaviour
{
    #region propeties
    public bool IsRotating { get; private set; }
    #endregion

    #region methods
    /// <summary>
    /// object move method in set direction and distance
    /// </summary>
    /// <param name="currentDirection"> current object direction</param>
    /// <param name="targetDirection">target direction</param>
    /// <param name="duration"> duration of movement</param>
    /// <param name="onStart">callback on start method</param>
    /// <param name="onComplete">callback on complete method</param>
    public void Rotate(Vector2Int currentPosition, Direction currentDirection, Direction targetDirection, float duration, Action onStart = null, Action onComplete = null)
    {
        if (IsRotating) return;            
        StartCoroutine(RotateRoutine(currentPosition, currentDirection, targetDirection, duration, onStart, onComplete));
    }

    /// <summary>
    /// smooth execution of rotation, on start and on complete methods 
    /// </summary>
    /// <param name="currentDirection"> current object direction</param>
    /// <param name="targetDirection">target direction</param>
    /// <param name="duration">duration of rotation</param>
    /// <param name="onStart">method executed before start of rotation</param>
    /// <param name="onComplete">method executed after end of rotation</param>
    /// <returns></returns>
    private IEnumerator RotateRoutine(Vector2Int currentPosition, Direction currentDirection, Direction targetDirection, float duration, Action onStart, Action onComplete)
    {
        IsRotating = true;

        onStart?.Invoke();

        Vector3 targetRotation;

        // If duration is 0, rotate instantly
        if (duration <= 0f)
        {
            targetRotation = GetRotation(targetDirection);
            transform.eulerAngles = targetRotation;
            onComplete?.Invoke();
            IsRotating = false;
            yield break;
        }

        //calculate rotation angles
        Vector3 startRotation = GetRotation(currentDirection);
        targetRotation = GetRotation(targetDirection);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // Handle 360-degree wrapping for smooth rotation
            Vector3 currentRotation = new Vector3(
                Mathf.LerpAngle(startRotation.x, targetRotation.x, t),
                Mathf.LerpAngle(startRotation.y, targetRotation.y, t),
                Mathf.LerpAngle(startRotation.z, targetRotation.z, t)
            );
            
            transform.eulerAngles = currentRotation;
            yield return null;
        }
        
        // Ensure final rotation is exact
        transform.eulerAngles = targetRotation;

        onComplete?.Invoke();

        IsRotating = false;
    }

    /// <summary>
    /// Get rotation vector for set direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector3 GetRotation(Direction direction)
    {
        Vector3 rotate = transform.eulerAngles;

        switch (direction)
        {
            case Direction.Up: rotate.z = 180; break;
            case Direction.Left: rotate.z = 270; break;
            case Direction.Right: rotate.z = 90; break;
            case Direction.Down: rotate.z = 0; break;
        }
        return rotate;
    }

}
#endregion
