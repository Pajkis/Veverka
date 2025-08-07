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

        #region fields
        [SerializeField] DirectionEvent onArrowPressed;
        [SerializeField] CharacterMovedEvent veverkaMoved;        
        #endregion

        #region event handling
        /// <summary>
        /// on enable - add listeners
        /// </summary>
        private void OnEnable()
        {
           onArrowPressed.AddListener(HandleInput);          
        }
        
        /// <summary>
        /// on disable - remove listeners
        /// </summary>
        private void OnDisable() 
        {
         onArrowPressed.RemoveListener(HandleInput);
        }        
        #endregion

        #region methods
        /// <summary>
        /// Polls arrow key input and forwards it to the handler.
        /// </summary>
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

                if (pushableObject != null)
                {                   
                    //try to push
                    if (pushableObject.CanBePushed(inputDirection))
                    {
                        Move(inputDirection, moveDistance);
                        pushableObject.Move(inputDirection, moveDistance);
                    }
                    else
                    {
                        // Optionally animate failed push
                    }
                }
                // move character to empty tile
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
        /// Move update in character movement 
        /// Add expect source to registrer, that object is going to move and this action will be registered via TurnBuilder into SingleTurnRecord
       /// </summary>
        /// <param name="direction">direction of movement</param>
        /// <param name="distance">number of tiles to move</param>
        /// <param name="duration">duration of movement</param>
        protected override void Move(Direction direction, int distance, float duration = 0.15F)
        {           
            base.Move(direction, distance, duration);    
        }

        /// <summary>
        /// On Move start action
        /// </summary>
        protected override void OnMoveStart()
        {
            AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.VeverkaMove);
        }

        /// <summary>
        /// Called after the character finishes moving; raises the movement event.
        /// </summary>
        protected override void OnMoveComplete(Vector2Int targetPosition)
        {
            base.OnMoveComplete(targetPosition);
            // raise event, that character made a turn -> turn count
            veverkaMoved.Raise(payload);
        }
        #endregion
    }

}