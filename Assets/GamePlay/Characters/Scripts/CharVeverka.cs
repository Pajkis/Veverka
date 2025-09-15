using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Control of Veverka
/// Gampelay/Characters/Scripts/CharVeverka
/// </summary>
/// 
public class CharVeverka : Character
{
    #region events 
    [Header("Character basic Veverka events")]
    [SerializeField] DirectionEvent onArrowPressed;
    [SerializeField] NutEvents nutEvents;
    #endregion
       
    // Queue for pending character data requests
    private bool hasPendingDataRequest = false;
    private bool hasShownStartupMessage = false;

    // Store previous direction for rotation undo
    private Direction rotationPreviousDirection;

    #region event handling
    /// <summary>
    /// on enable - add listeners
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

        // Add listener for handle input
        onArrowPressed.AddListener(HandleInput);
    }

    /// <summary>
    /// on disable - remove listeners
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();

        onArrowPressed.RemoveListener(HandleInput);        
    }

    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
                CurrentDirection = facingDirection,
                Duration = moveDuration,
                RequestData = false,
                ResponseData = true,
                CharacterBubbleMessage = BubbleMessageType.LetsStart,
                CharacterBubbleMessageTime = gameplayConfig.characterBubbleTime
            });
        }
    }

    /// <summary>
    /// on character data request - respond with character data
    /// </summary>
    /// <param name="payload"></param>
    protected override void OnCharacterEvent(CharacterEventPayload payload)
    {
        if (payload.EventType == CharacterEventType.DataRequest)
        {
            // If turn is active, queue the request
            if (FindObjectOfType<TurnControl>()?.IsInputLocked == true)
            {
                hasPendingDataRequest = true;
                return;
            }

            // Send current position immediately if turn not active
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
            CurrentDirection = facingDirection,
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
        DebugLogger.Log(DebugLogCategory.Input, $"HandleInput called with direction: {inputDirection}", this);
        
        // Use TurnControl instead of SmoothMover for input locking
        if (FindObjectOfType<TurnControl>()?.IsInputLocked == true) 
        {
            DebugLogger.Log(DebugLogCategory.Input, "Input is locked by TurnControl, ignoring", this);
            return;
        }

        DebugLogger.Log(DebugLogCategory.Input, "Calling TurnControl.OnInputTriggered()", this);
        // Notify TurnControl that input was triggered
        FindObjectOfType<TurnControl>()?.OnInputTriggered();

        // Try to move in input arrow direction
        if (inputDirection == facingDirection)
        {
            int distance = moveDistance;
            Vector2Int targetPos = GridUtils.GetPositionInDir(gridPosition, inputDirection, distance);
            Debug.Log($"current position (x,y): {gridPosition.x}, {gridPosition.y}");
            Debug.Log($"target position to move (x,y): {targetPos.x}, {targetPos.y}");

            // Check if target position is in grid
            var gridQuery = new TileQueryPayload { Position = targetPos };
            gridEvents.Raise(new GridEventPayload
            {
                EventType = GridEventType.TileQuery,
                Query = gridQuery
            });
            if (!gridQuery.IsInGrid) 
            {
                DebugLogger.Log(DebugLogCategory.CharacterMovement, "Target not in grid - sending MoveFailed event", this);
                characterEvents.Raise(new CharacterEventPayload
                {
                    EventType = CharacterEventType.MoveFailed,
                    CurrentPosition = gridPosition,
                    CurrentDirection = inputDirection,
                });
                DebugLogger.Log(DebugLogCategory.EventSystem, "MoveFailed event sent", this);
                return;
            }

            // If tile is walkable, move character
            if (gridQuery.IsWalkable)
            {
                DebugLogger.Log(DebugLogCategory.CharacterMovement, "Tile is walkable - calling Move() which will send MoveStarted event", this);
                Move(inputDirection, distance, moveDuration);
                return;
            }

            // Check if tile is pushable (only push nuts)
            if (gridQuery.TileType != TileType.Nut)
            {
                DebugLogger.Log(DebugLogCategory.TileInteraction, "Tile is not a nut - sending MoveFailed event", this);
                characterEvents.Raise(new CharacterEventPayload
                {
                    EventType = CharacterEventType.MoveFailed,
                    CurrentPosition = gridPosition,
                    CurrentDirection = inputDirection,
                });
                DebugLogger.Log(DebugLogCategory.EventSystem, "MoveFailed event sent", this);
                return;
            }

            // Query if the nut can be pushed in this direction
            Vector2Int targetPushPos = GridUtils.GetPositionInDir(targetPos, inputDirection, distance);
            var pushQuery = new TileQueryPayload
            {
                Position = targetPushPos,
            };
            Debug.Log($"Querying pushable nut at (x,y): {pushQuery.Position.x}, {pushQuery.Position.y}, is pushable: {pushQuery.IsPushable}");

            gridEvents.Raise(new GridEventPayload
            {
                EventType = GridEventType.TileQuery,
                Query = pushQuery
            });

            if (!pushQuery.IsInGrid) 
            {
                DebugLogger.Log(DebugLogCategory.NutMovement, "Push target not in grid - sending MoveFailed event", this);
                characterEvents.Raise(new CharacterEventPayload
                {
                    EventType = CharacterEventType.MoveFailed,
                    CurrentPosition = gridPosition,
                    CurrentDirection = inputDirection,
                });
                DebugLogger.Log(DebugLogCategory.EventSystem, "MoveFailed event sent", this);
                return;
            }

            // If the nut can be pushed, move both the nut and the character
            if (pushQuery.IsPushable)
            {
                DebugLogger.Log(DebugLogCategory.NutMovement, "Nut is pushable - calling Move() which will send MoveStarted event", this);
                Move(inputDirection, distance, moveDuration);
                nutEvents.Raise(new NutEventPayload
                {
                    EventType = NutEventType.NutPush,
                    PreviousPosition = targetPos,
                    CurrentPosition = targetPushPos,
                    Direction = inputDirection,
                    Distance = distance,
                    Duration = moveDuration
                });
            }
            else
            {
                DebugLogger.Log(DebugLogCategory.NutMovement, "Nut cannot be pushed - sending MoveFailed event", this);
                // move failed
                characterEvents.Raise(new CharacterEventPayload
                {
                    EventType = CharacterEventType.MoveFailed,
                    CurrentPosition = gridPosition,
                    CurrentDirection = inputDirection,
                });
                DebugLogger.Log(DebugLogCategory.EventSystem, "MoveFailed event sent", this);

                // Cannot push - optionally animate failed push
                DebugLogger.Log(DebugLogCategory.NutMovement, "Cannot push nut in this direction", this);
            }
        }
        // Rotate to input arrow direction
        else
        {
            DebugLogger.Log(DebugLogCategory.Rotation, "Need to rotate - calling Rotate() which will send RotateStarted event", this);
            // Rotate character to input arrow direction
            Rotate(facingDirection, inputDirection, rotationDuration);            
        }
    }

    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        //Reset  pending data request flag
        hasPendingDataRequest = false;

        // play move sound
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.VeverkaMove });
    }

    /// <summary>
    /// Called after the character finishes moving; raises the movement event.
    /// </summary>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        base.OnMoveComplete(targetPosition);

        // Note: MoveCompleted event is already raised in base.OnMoveComplete()
        // No need to raise it again here

        // Handle any pending character data requests now that move is complete
        if (hasPendingDataRequest)
        {
            hasPendingDataRequest = false;
            SendCharacterData(); // Use default values for movement completion
        }
    }

    #endregion

    #region rotate

    /// <summary>
    /// rotation of object to the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected override void Rotate(Direction currentDirection, Direction targetDirection, float duration)
    {
        DebugLogger.Log(DebugLogCategory.Rotation, $"Rotate called: {currentDirection} -> {targetDirection}", this);

        // Store previous direction for completion event
        rotationPreviousDirection = currentDirection;

        // start rotate
        DebugLogger.Log(DebugLogCategory.EventSystem, "Sending RotateStarted event", this);
        characterEvents.Raise(new CharacterEventPayload
        {
            Character = this,
            EventType = CharacterEventType.RotateStarted,
            CurrentPosition = gridPosition,
            CurrentDirection = targetDirection,
            PreviousDirection = currentDirection
        });
        DebugLogger.Log(DebugLogCategory.EventSystem, "RotateStarted event sent", this);

        // Update facing direction immediately since this is the new direction
        facingDirection = targetDirection;

        smoothRotate.Rotate(gridPosition, currentDirection, targetDirection, duration, OnRotateStart, () => OnRotateComplete(gridPosition));
    }

    /// <summary>
    /// Handles the start of a rotation event.
    /// </summary>
    /// <remarks>This method raises an audio event to play a sound effect associated with the rotation action.
    /// Override this method to customize behavior when a rotation starts.</remarks>
    protected override void OnRotateStart()
    {
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.VeverkaRotate });
    }

    /// <summary>
    /// Handles the completion of a rotation operation.
    /// </summary>
    /// <param name="targetTransform">The grid position (unchanged during rotation).</param>
    protected override void OnRotateComplete(Vector2Int targetTransform)
    {
       // Grid position doesn't change during rotation, only facing direction changes
       // The facing direction should be updated here to match the target direction
       
        characterEvents.Raise(new CharacterEventPayload
        {
            Character = this,
            EventType = CharacterEventType.RotateCompleted,
            CurrentPosition = gridPosition,
            CurrentDirection = facingDirection,
            PreviousDirection = rotationPreviousDirection
        });
    }

    #endregion
}
