using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Control of Veverka
/// </summary>
public class Veverka : MovableTile, IRotable
{
    #region Fields
    Direction facingDirection;
    #endregion

    #region IRotable interface
    /// <summary>
    /// Facing direction propety
    /// </summary>
    public Direction FacingDirection 
    { 
        get { return facingDirection; }
        set { facingDirection = value; }
    }

    /// <summary>
    /// rotation of object to the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Direction Rotate(Direction direction)
    {
        facingDirection = direction;

        Vector3 rotate = transform.eulerAngles;

        switch (direction)
        {
            case Direction.Up: rotate.z = 180; break;
            case Direction.Left: rotate.z = 270; break;
            case Direction.Right: rotate.z = 90; break;
            case Direction.Down: rotate.z = 0; break;
        }

        transform.eulerAngles = rotate;
        return direction;
    }
    #endregion

    #region Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        facingDirection = Direction.Down;
        base.Init(tileType, gridPosition);        
    }

    // Update is called once per frame
    void Update()
    {    
        if (Input.GetKeyDown(KeyCode.UpArrow)) HandleInput(Direction.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) HandleInput(Direction.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) HandleInput(Direction.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) HandleInput(Direction.Right);
    }

    /// <summary>
    /// Handles input from keyboard
    ///     - direction of object and input arrow are same -> tries to move in that direction
    ///     - direction of object and input arrow does not match -> rotates to input arrow direction 
    /// </summary>
    /// <param name="direction"></param>
    void HandleInput(Direction inputDirection)
    {
        if (IsMoving) return;

        // Try to move in input arrow direction
        if (inputDirection == facingDirection)
        {
            Vector2Int targetPos = GridUtils.GetAdjacentPosition(gridPosition, inputDirection);
            if (!GameGrid.Instance.IsInGrid(targetPos)) return;

            // I should not get only I movable, but also Ipushable
            PushableTile pushableObject = null;
            TileObject targetObject = GameGrid.Instance.GetPushableAt(targetPos);            
            if (targetObject != null && (targetObject is PushableTile))
            {
                pushableObject = targetObject as PushableTile;
            }
           
            // check for pushable objects
            if (pushableObject != null)
            {                          
                //try to push
                if (pushableObject.CanBePushed(inputDirection))
                {
                    pushableObject.Move(inputDirection);
                    Move(inputDirection);
                }
                else
                {
                    // Optionally animate failed push
                }
            }
            else if (GameGrid.Instance.IsWalkableAt(targetPos))
            {
                Move(inputDirection);
            }
          //  GameGrid.Instance.LogMovablePositions();
        }

        // rotate to input arrow direction
        else
        {          
            facingDirection = Rotate(inputDirection); 
        }
    }
    
    #endregion
}
