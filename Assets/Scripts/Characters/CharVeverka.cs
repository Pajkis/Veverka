using UnityEngine;
using Veverka.GridSystem.GameGrid;

/// <summary>
/// Control of Veverka
/// </summary>
/// 
namespace Veverka.Characters.Veverka
 { 
    public class CharVeverka : Character
    {
        #region methods
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
            if (smoothMover.IsMoving) return;

            // Try to move in input arrow direction
            if (inputDirection == facingDirection)
            {
                Vector2Int targetPos = GridUtils.GetPositionInDir(gridPosition, inputDirection);
                Debug.Log($"target position to move (x,y): {targetPos.x}, {targetPos.y} ");
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
                        pushableObject.Move(inputDirection, moveDistance);
                        Move(inputDirection, moveDistance);                
                    }
                    else
                    {
                        // Optionally animate failed push
                    }
                }
                else if (GameGrid.Instance.IsWalkableAt(targetPos))
                {
                    Move(inputDirection, moveDistance);
                }              
            }

            // rotate to input arrow direction
            else
            {          
                facingDirection = Rotate(inputDirection);
                AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.VeverkaRotate);
            }
        }

        /// <summary>
        /// On Move start action
        /// </summary>
        protected override void OnMoveStart()
        {
            AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.VeverkaMove);
        }

        #endregion
    }

}