using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Execution of Smooth movement, backcalling OnStart and OnComplete methods
/// </summary>
public class SmoothMover : MonoBehaviour
{
    #region propeties
    public bool IsMoving { get; private set; }
    #endregion

    #region methods
    /// <summary>
    /// object move method in set direction and distance
    /// </summary>
    /// <param name="currentPosition"> current object position</param>
    /// <param name="targetPosition">target position</param>
    /// <param name="duration"> duration of movement</param>
    /// <param name="onStart">callback on start method</param>
    /// <param name="onComplete">callback on complete method</param>
    public void Move(Vector3 currentPosition,Vector3 targetPosition, float duration, Action onStart = null, Action onComplete = null)
    {
        if (IsMoving) return;            
        StartCoroutine(MoveRoutine(currentPosition, targetPosition, duration, onStart, onComplete));
    }

    /// <summary>
    /// smooth execution of movement, on start and on complete methods 
    /// </summary>
    /// <param name="currentPosition"> start position for movement</param>
    /// <param name="targetPosition">End position for movement</param>
    /// <param name="duration">duration of movement</param>
    /// <param name="onStart">method executed before start of movement</param>
    /// <param name="onComplete">method executed after end of movement</param>
    /// <returns></returns>
    private IEnumerator MoveRoutine(Vector3 currentPosition, Vector3 targetPosition, float duration, Action onStart, Action onComplete)
    {
        IsMoving = true;

        onStart?.Invoke();            

        Vector3 startPos = currentPosition;
        Vector3 targetPos = targetPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.localPosition = targetPos;

        onComplete?.Invoke();

        IsMoving = false;
    }
}
#endregion
