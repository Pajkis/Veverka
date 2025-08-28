using UnityEngine;

/// <summary>
/// Control of Veverka
/// </summary>
/// 
namespace Veverka.Characters.Veverka
 { 
    public class CharVeverka : Character
    {
        #region events 
        [SerializeField] DirectionEvent onArrowPressed;
        [SerializeField] CharacterMovedEvent veverkaMoved;
        [SerializeField] CanPushQueryEvent canPushQueryEvent;
        #endregion

        #region event handling
        /// <summary>
        /// on enable - add listeners
        /// </summary>
        private void OnEnable()
        {
            onArrowPressed.AddListener(HandleInput);
            settingDataBroadcastEvent.AddListener(OnSettingData);
            settingDataRequestEvent.Raise(GameSettingsEnum.AnimationSpeed);
        }

        /// <summary>
        /// on disable - remove listeners
        /// </summary>
        private void OnDisable()
        {
            onArrowPressed.RemoveListener(HandleInput);
            settingDataBroadcastEvent.RemoveListener(OnSettingData);
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
                Debug.Log($"target position to move (x,y): {targetPos.x}, {targetPos.y}");

                // Check if target position is in grid
                var gridQuery = new TileQueryPayload { Position = targetPos };
                tileQueryEvent.Raise(gridQuery);
                if (!gridQuery.IsInGrid) return;

                // Check if there's a movable object at target position
                if (gridQuery.IsMovable)
                {
                    // Query if the nut can be pushed in this direction
                    var pushQuery = new CanPushQueryPayload
                    {
                        Position = targetPos,
                        Direction = inputDirection,
                        Distance = distance,
                        Duration = moveDuration,
                        CanBePushed = false,
                    };

                    // Push query and push if possible
                    canPushQueryEvent.Raise(pushQuery);

                    // If there's a nut and it can be pushed
                    if (pushQuery.CanBePushed)
                    {
                        // Move the character
                        Move(inputDirection, distance, moveDuration);  
                    }
                    else
                    {
                        // Cannot push - optionally animate failed push
                        Debug.Log("Cannot push nut in this direction");
                    }
                }
                // Move character to empty walkable tile
                else if (gridQuery.IsWalkable)
                {
                    Move(inputDirection, distance, moveDuration);
                }
                else
                {
                    Debug.Log("Cannot move - tile not walkable");
                }
            }
            // Rotate to input arrow direction
            else
            {
                facingDirection = Rotate(inputDirection);
                playSfxEvent.Raise(SfxType.VeverkaRotate);
            }
        }
        
        /// <summary>
        /// On Move start action
        /// </summary>
        protected override void OnMoveStart()
        {
            playSfxEvent.Raise(SfxType.VeverkaMove);
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