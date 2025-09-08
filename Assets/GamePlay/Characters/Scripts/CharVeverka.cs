using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
          [SerializeField] NutEvents nutEvents;
        #endregion

        // Queue for pending character data requests
        private bool hasPendingDataRequest = false;
        private bool hasShownStartupMessage = false;

        #region event handling
        /// <summary>
        /// on enable - add listeners
        /// </summary>
        private void OnEnable()
        {
            onArrowPressed.AddListener(HandleInput);
            settingDataBroadcastEvent.AddListener(OnSettingData);
              characterEvents.AddListener(OnCharacterEvent);
            settingDataRequestEvent.Raise(GameSettingsEnum.AnimationSpeed);

            // Listen for scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// on disable - remove listeners
        /// </summary>
        private void OnDisable()
        {
            onArrowPressed.RemoveListener(HandleInput);
            settingDataBroadcastEvent.RemoveListener(OnSettingData);
              characterEvents.RemoveListener(OnCharacterEvent);

            // Remove scene change listener
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Called when a new scene is loaded
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Check if this is a game scene (you can adjust the condition as needed)
            if (scene.name.Contains("Game") || scene.name.Contains("Level") || scene.name.Contains("InGame"))
            {
                Debug.Log($"Game scene loaded: {scene.name}, sending startup message");
                hasShownStartupMessage = false; // Reset flag for new level
                StartCoroutine(SendStartupMessageAfterDelay());
            }
        }

        /// <summary>
        /// Send startup message after a short delay to ensure all systems are ready
        /// </summary>
        private IEnumerator SendStartupMessageAfterDelay()
        {
            // Wait a few frames for everything to initialize
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Send startup message
            if (!hasShownStartupMessage)
            {
                hasShownStartupMessage = true;
                Debug.Log("Sending startup message from Veverka");

                    characterEvents.Raise(new CharacterEventPayload
                    {
                        Character = this,
                        EventType = CharacterEventType.DataResponse,
                        CurrentPosition = gridPosition,
                        Direction = facingDirection,
                        Duration = moveDuration,
                        RequestData = false,
                        ResponseData = true,
                        CharacterBubbleMessage = BubbleMessageType.LetsStart,
                        CharacterBubbleMessageTime = 2.0f
                    });
            }
        }

        /// <summary>
        /// on character data request - respond with character data
        /// </summary>
        /// <param name="payload"></param>
          private void OnCharacterEvent(CharacterEventPayload payload)
          {
              if (payload.EventType == CharacterEventType.DataRequest)
              {
                  // If character is currently moving, queue the request
                  if (smoothMover.IsMoving)
                  {
                      hasPendingDataRequest = true;
                      return;
                  }

                  // Send current position immediately if not moving
                  SendCharacterData();
              }
          }

        /// <summary>
        /// Send character data response (for regular requests, not startup)
        /// </summary>
        private void SendCharacterData()
        {
                characterEvents.Raise(new CharacterEventPayload
                {
                    Character = this,
                    EventType = CharacterEventType.DataResponse,
                    CurrentPosition = gridPosition,
                    Direction = facingDirection,
                    Duration = moveDuration,
                    RequestData = false,
                    ResponseData = true,
                    CharacterBubbleMessage = BubbleMessageType.LetsStart, // Default - won't be used
                    CharacterBubbleMessageTime = 0f
                });
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
                gridEvents.Raise(new GridEventPayload
                {
                    EventType = GridEventType.TileQuery,
                    Query = gridQuery
                });
                if (!gridQuery.IsInGrid) return;

                // Check if there's a pushable object at target position
                if (gridQuery.TileType == TileType.Nut)
                {
                    // Query if the nut can be pushed in this direction
                    var pushQuery = new NutEventPayload
                    {
                        EventType = NutEventType.CanPushQuery,
                        Position = targetPos,
                        Direction = inputDirection,
                        Distance = distance,
                        Duration = moveDuration,
                        CanBePushed = false,
                    };

                    // Push query and push if possible
                    nutEvents.Raise(pushQuery);

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
              payload.EventType = CharacterEventType.MoveCompleted;
              characterEvents.Raise(payload);

            // Handle any pending character data requests now that move is complete
            if (hasPendingDataRequest)
            {
                hasPendingDataRequest = false;
                SendCharacterData(); // Use default values for movement completion
            }
        }
        #endregion
    }
}