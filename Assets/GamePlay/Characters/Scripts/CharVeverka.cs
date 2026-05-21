using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
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
        // Check if this is the GamePlay scene using Addressables
        if (SceneRefProvider.Instance != null)
        {
            AssetReference gamePlayScene = SceneRefProvider.Instance.GetScene(SceneType.GamePlay);
            if (gamePlayScene != null && gamePlayScene.RuntimeKeyIsValid())
            {
                DebugLogger.Log(DebugLogCategory.SceneManager, $"GamePlay scene loaded: {scene.name}, preparing startup message", this);
                hasShownStartupMessage = false;
                StartCoroutine(SendStartupMessageAfterDelay());
            }
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
            DebugLogger.Log(DebugLogCategory.BubbleMessage, $"Sending startup bubble message: {BubbleMessageType.LetsStart}", this);

            characterEvents.Raise(new CharacterEventPayload
            {
                Character = this,
                EventType = CharacterEventType.DataResponse,
                CurrentPosition = gridPosition,
                CurrentDirection = facingDirection,
               // Duration = baseMoveDuration,
                RequestData = false,
                ResponseData = true,
                BubbleMessage = BubbleMessageEventPayload.Create(BubbleMessageType.LetsStart, gameplayConfig.characterBubbleTime),
                PopoutImage = PopoutImageEventPayload.None
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
            DebugLogger.Log(DebugLogCategory.EventSystem, "Received DataRequest event", this);

            // If turn is active, queue the request
            if (FindObjectOfType<TurnControl>()?.IsInputLocked == true)
            {
                DebugLogger.Log(DebugLogCategory.TurnControl, "Input locked - queuing data request", this);
                hasPendingDataRequest = true;
                return;
            }

            // Send current position immediately if turn not active
            DebugLogger.Log(DebugLogCategory.EventSystem, "Sending character data response", this);
            SendCharacterData();
        }
    }

    /// <summary>
    /// Send character data response (for regular requests, not startup)
    /// </summary>
    private void SendCharacterData()
    {
        DebugLogger.Log(DebugLogCategory.EventSystem, $"Sending character data - Position: {gridPosition}, Direction: {facingDirection}", this);

        characterEvents.Raise(new CharacterEventPayload
        {
            Character = this,
            EventType = CharacterEventType.DataResponse,
            CurrentPosition = gridPosition,
            CurrentDirection = facingDirection,
           // Duration = baseMoveDuration,
            RequestData = false,
            ResponseData = true,
            BubbleMessage = BubbleMessageEventPayload.None,
            PopoutImage = PopoutImageEventPayload.None
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
    void HandleInput(DirectionPayload InputDirPayload)
    {
        DebugLogger.Log(DebugLogCategory.Input, $"HandleInput called - Direction: {InputDirPayload.direction}", this);
        
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
        if (InputDirPayload.direction == facingDirection)
        {
            float moveDuration = baseMoveDuration / turnState.CurrentSpeedMultiplier;
            TryMoveToTarget(InputDirPayload, moveDuration);
        }
        // Rotate to input arrow direction
        else
        {
            ExecuteRotation(InputDirPayload);
        }
    }

    /// <summary>
    /// Raises MoveFailed event for character
    /// </summary>
    /// <param name="direction">Direction of failed movement attempt</param>
    private void RaiseMoveFailed(Direction direction)
    {
        DebugLogger.Log(DebugLogCategory.CharacterMovement, "Sending MoveFailed event", this);
        characterEvents.Raise(new CharacterEventPayload
        {
            EventType = CharacterEventType.MoveFailed,
            CurrentPosition = gridPosition,
            CurrentDirection = direction,
        });
        DebugLogger.Log(DebugLogCategory.EventSystem, "MoveFailed event sent", this);
    }

    /// <summary>
    /// Executes character movement with logging
    /// </summary>
    /// <param name="direction">Direction to move</param>
    /// <param name="distance">Distance to move</param>
    /// <param name="effectiveDuration">Effective duration (already adjusted by speedMultiplier)</param>
    private void ExecuteMove(Direction direction, int distance)
    {
        DebugLogger.Log(DebugLogCategory.CharacterMovement,
            $"Calling Move() in {direction}", this);
        Move(direction, distance, baseMoveDuration / turnState.CurrentSpeedMultiplier);
    }

    /// <summary>
    /// Checks if position is valid in grid
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="query">Output query payload with results</param>
    /// <returns>True if position is in grid, false otherwise</returns>
    private bool IsPositionValid(Vector2Int position, out TileQueryPayload query)
    {
        query = new TileQueryPayload { Position = position };
        gridEvents.Raise(new GridEventPayload
        {
            EventType = GridEventType.TileQuery,
            Query = query
        });
        return query.IsInGrid;
    }

    /// <summary>
    /// Executes character rotation to input direction
    /// </summary>
    /// <param name="inputPayload">Input direction payload</param>
    private void ExecuteRotation(DirectionPayload inputPayload)
    {
        float rotationDuration = baseRotationDuration / turnState.CurrentSpeedMultiplier;
        DebugLogger.Log(DebugLogCategory.Rotation, $"Need to rotate - calling Rotate() with Duration: {rotationDuration})", this);
        // Rotate character to input arrow direction
        Rotate(facingDirection, inputPayload.direction, rotationDuration);
    }

    /// <summary>
    /// Executes both character move and nut push
    /// </summary>
    /// <param name="nutPos">Current nut position</param>
    /// <param name="targetPos">Target nut position</param>
    /// <param name="direction">Direction of push</param>
    /// <param name="distance">Distance to push</param>
    /// <param name="effectiveDuration">Effective duration for move</param>
    /// <param name="speedMultiplier">Speed multiplier for nut animation</param>
    private void ExecutePushMove(Vector2Int nutPos, Vector2Int targetPos, Direction direction, int distance)
    {
        ExecuteMove(direction, distance);

        DebugLogger.Log(DebugLogCategory.NutMovement, $"Sending NutPush event - From: {nutPos} To: {targetPos}", this);
        nutEvents.Raise(new NutEventPayload
        {
            EventType = NutEventType.NutPush,
            PreviousPosition = nutPos,
            CurrentPosition = targetPos,
            Direction = direction,
            Distance = distance,
         
        });
    }

    /// <summary>
    /// Tries to push a nut in the specified direction
    /// </summary>
    /// <param name="nutPosition">Current nut position</param>
    /// <param name="direction">Direction to push</param>
    /// <param name="distance">Distance to push</param>
    /// <param name="effectiveDuration">Effective duration for movement</param>
    /// <param name="speedMultiplier">Speed multiplier for animation</param>
    /// <returns>True if push succeeded, false otherwise</returns>
    private bool TryPushNut(Vector2Int nutPosition, Direction direction, int distance)
    {
        // Query if the nut can be pushed in this direction
        Vector2Int targetPushPos = GridUtils.GetPositionInDir(nutPosition, direction, distance);
        DebugLogger.Log(DebugLogCategory.NutMovement, $"Checking if nut can be pushed to: {targetPushPos}", this);

        if (!IsPositionValid(targetPushPos, out TileQueryPayload pushQuery))
        {
            DebugLogger.Log(DebugLogCategory.NutMovement, "Push target not in grid - sending MoveFailed event", this);
            RaiseMoveFailed(direction);
            return false;
        }

        // If the nut can be pushed, move both the nut and the character
        if (pushQuery.IsPushable)
        {
            DebugLogger.Log(DebugLogCategory.NutMovement, $"Nut is pushable from {nutPosition} to {targetPushPos}", this);
            ExecutePushMove(nutPosition, targetPushPos, direction, distance);
            return true;
        }
        else
        {
            DebugLogger.Log(DebugLogCategory.NutMovement, $"Nut cannot be pushed to {targetPushPos} - sending MoveFailed event", this);
            RaiseMoveFailed(direction);

            // Cannot push - optionally animate failed push
            DebugLogger.Log(DebugLogCategory.NutMovement, "Cannot push nut in this direction", this);
            return false;
        }
    }

    /// <summary>
    /// Attempts to move character to target position in given direction
    /// Handles walkable tiles and nut pushing
    /// </summary>
    /// <param name="inputPayload">Input direction payload</param>
    /// <param name="effectiveDuration">Effective duration for movement</param>
    private void TryMoveToTarget(DirectionPayload inputPayload, float effectiveDuration)
    {
        int distance = moveDistance;
        Vector2Int targetPos = GridUtils.GetPositionInDir(gridPosition, inputPayload.direction, distance);
        DebugLogger.Log(DebugLogCategory.CharacterMovement, $"Movement attempt - From: {gridPosition} To: {targetPos} Direction: {inputPayload.direction}", this);

        // Check if target position is in grid
        if (!IsPositionValid(targetPos, out TileQueryPayload gridQuery))
        {
            DebugLogger.Log(DebugLogCategory.CharacterMovement, "Target not in grid - sending MoveFailed event", this);
            RaiseMoveFailed(inputPayload.direction);
            return;
        }

        // If tile is walkable, move character
        if (gridQuery.IsWalkable)
        {
            DebugLogger.Log(DebugLogCategory.CharacterMovement, $"Tile is walkable", this);
            ExecuteMove(inputPayload.direction, distance);
            return;
        }

        // Check if tile is pushable (only push nuts)
        if (gridQuery.TileType != TileType.Nut)
        {
            DebugLogger.Log(DebugLogCategory.TileInteraction, "Tile is not a nut - sending MoveFailed event", this);
            RaiseMoveFailed(inputPayload.direction);
            return;
        }

        // Try to push the nut
        TryPushNut(targetPos, inputPayload.direction, distance);
    }

    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        DebugLogger.Log(DebugLogCategory.CharacterMovement, "Move started - resetting pending request flag", this);
        hasPendingDataRequest = false;

        DebugLogger.Log(DebugLogCategory.Audio, "Playing Veverka move sound", this);
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.VeverkaMove });
    }

    /// <summary>
    /// Called after the character finishes moving; raises the movement event.
    /// </summary>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        DebugLogger.Log(DebugLogCategory.CharacterMovement, $"Move completed to position: {targetPosition}", this);
        base.OnMoveComplete(targetPosition);

        // Handle any pending character data requests now that move is complete
        if (hasPendingDataRequest)
        {
            DebugLogger.Log(DebugLogCategory.EventSystem, "Processing pending data request after move completion", this);
            hasPendingDataRequest = false;
            SendCharacterData();
        }
    }

    #endregion

    #region rotate

    /// <summary>
    /// rotation of object to the direction
    /// </summary>
    /// <param name="currentDirection">Current facing direction</param>
    /// <param name="targetDirection">Target direction to rotate to</param>
    /// <param name="duration">effective rotation duration (already adjusted by speedMultiplier in caller)</param>
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
        DebugLogger.Log(DebugLogCategory.Audio, "Playing Veverka rotate sound", this);
        audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.VeverkaRotate });
    }

    /// <summary>
    /// Handles the completion of a rotation operation.
    /// </summary>
    /// <param name="targetTransform">The grid position (unchanged during rotation).</param>
    protected override void OnRotateComplete(Vector2Int targetTransform)
    {
        DebugLogger.Log(DebugLogCategory.Rotation, $"Rotation completed - Direction: {rotationPreviousDirection} -> {facingDirection}", this);

        characterEvents.Raise(new CharacterEventPayload
        {
            Character = this,
            EventType = CharacterEventType.RotateCompleted,
            CurrentPosition = gridPosition,
            CurrentDirection = facingDirection,
            PreviousDirection = rotationPreviousDirection
        });

        DebugLogger.Log(DebugLogCategory.EventSystem, "RotateCompleted event sent", this);
    }

    #endregion
}
