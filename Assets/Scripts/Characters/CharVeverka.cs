using UnityEngine;

/// <summary>
/// Control of Veverka
/// </summary>
/// 
namespace Veverka.Characters.Veverka
 { 
    public class CharVeverka : Character
    {
        #region events definition
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
        /// Handles input from arrow direction events
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
                int distance = moveDistance;
                Vector2Int targetPos = GridUtils.GetPositionInDir(gridPosition, inputDirection, distance);
                Debug.Log($"target position to move (x,y): {targetPos.x}, {targetPos.y} ");

                // check if target position is in grid and get tile query
                var query = new TileQueryPayload { Position = targetPos };
                tileQueryEvent.Raise(query);
                if (!query.IsInGrid) return;

                PushableTile pushableObject = query.TileObject as PushableTile;

                if (pushableObject != null)
                {
                    //try to push
                    if (pushableObject.CanBePushed(inputDirection))
                    {
                        Move(inputDirection, distance);
                        pushableObject.Move(inputDirection, distance);
                    }
                    else
                    {
                        // Optionally animate failed push
                    }
                }
                // move character to empty tile
                else if (query.IsWalkable)
                {
                    Move(inputDirection, distance);
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