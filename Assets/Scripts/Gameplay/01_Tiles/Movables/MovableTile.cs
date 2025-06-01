using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movable object setup and control
/// </summary>
public class MovableTile : TileObject
{
    #region Fields
    protected bool isMoving = false;
    [SerializeField] protected int moveDistance = 1; // 1 tile
    [SerializeField] protected float moveDuration = 0.3f;
    #endregion

    #region Propeties
    /// <summary>
    /// get property isMoving state
    /// </summary>
    public bool IsMoving => isMoving;

    /// <summary>
    /// Move duration property
    /// </summary>
    public float MoveDuration
    {
        get {return moveDuration;}
        set { moveDuration = value;}
    }

    /// <summary>
    /// Move distance property
    /// </summary>
    public int MoveDistance
    {
        get { return moveDistance; }
        set { moveDistance = value;}
    }
    #endregion


    #region Methods
    /// <summary>
    /// Move coroutine execution
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="moveDistance"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public virtual void Move(Direction direction, int moveDistance = 1, float duration = 0.15f)
    {
        if (!isMoving)
        {          
            OnMoveStart();
            Vector2Int targetPosition = GridUtils.GetAdjacentPosition(gridPosition, direction, moveDistance);
            StartCoroutine(MoveRoutine(targetPosition , duration));           
        }
    }

    /// <summary>
    /// Move routine execution
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected virtual IEnumerator MoveRoutine(Vector2Int targetPosition, float duration)
    {
        isMoving = true;
                
        // Animate from world position A to B
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = GridUtils.GridToWorld(targetPosition);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.localPosition = targetPos;

        OnMoveComplete(targetPosition);
    }
    #endregion

    /// <summary>
    /// On Move start action
    /// </summary>
    protected virtual void OnMoveStart()
    { 
    
    }

    /// <summary>
    /// On move complete action 
    /// </summary>
    /// <param name="targetPosition"></param>
    protected virtual void OnMoveComplete(Vector2Int targetPosition)
    {
        gridPosition = targetPosition;
        isMoving = false;
    }


}
