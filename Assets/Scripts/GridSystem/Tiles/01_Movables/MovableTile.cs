using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veverka.Movement.SmoothMover;

/// <summary>
/// Movable object setup and control
/// </summary>
public class MovableTile : TileObject
{
    #region Fields
    [SerializeField] protected int moveDistance = 1; // 1 tile
    [SerializeField] protected float moveDuration = 0.3f;

    protected SmoothMover smoothMover;
    #endregion

    #region Propeties
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
    /// Initialize tile type on grid position, init smooth mover
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        smoothMover = GetComponent<SmoothMover>();
        base.Init(tileType, gridPosition);      
    }

    /// <summary>
    /// Move of a object over a distance in set direction
    /// </summary>
    /// <param name="direction">direction of movemebt</param>
    /// <param name="distance">distance of movement in tiles</param>
    /// <param name="duration">duration of movement</param>
    public virtual void Move(Direction direction, int distance, float duration = 0.15f)
    {
        if (smoothMover.IsMoving) return;
        Vector3 currentPosition = transform.localPosition;
        Vector2Int targetPosVec2Int = GridUtils.GetPositionInDir(gridPosition, direction, moveDistance);
        Vector3 targetPosition = GridUtils.GridToWorld(targetPosVec2Int);
        float moveDuration = (duration * distance) / GameSettings.Instance.Get(GameSettingsEnum.AnimationSpeed);

        smoothMover.Move(currentPosition, targetPosition, moveDuration, OnMoveStart, () => OnMoveComplete(targetPosVec2Int));
    }
     
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
    }

    #endregion
}
