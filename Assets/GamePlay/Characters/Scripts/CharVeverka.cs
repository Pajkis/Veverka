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
        settingEvents.AddListener(OnSettingEvent);
        characterEvents.AddListener(OnCharacterEvent);
        settingEvents.Raise(new SettingEventPayload
        {
            EventType = SettingsEventType.DataRequest,
            Setting = GameSettingsEnum.AnimationSpeed
        });

        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// on disable - remove listeners
    /// </summary>
    private void OnDisable()
    {
        onArrowPressed.RemoveListener(HandleInput);
        settingEvents.RemoveListener(OnSettingEvent);
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
                // If turn is active, queue the request
                if (TurnControl.Instance != null && TurnControl.Instance.IsInputLocked)
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
        // Use TurnControl instead of SmoothMover for input locking
        if (TurnControl.Instance != null && TurnControl.Instance.IsInputLocked) return;
        
        // Notify TurnControl that input was triggered
        TurnControl.Instance?.OnInputTriggered();

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
            if (!gridQuery.IsInGrid) return;

            // If tile is walkable, move character
            if (gridQuery.IsWalkable)
            {
                Move(inputDirection, distance, moveDuration);
                return;
            }

            // Check if tile is pushable (only push nuts)
            if (gridQuery.TileType != TileType.Nut)
            {
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

            if (!pushQuery.IsInGrid) return;

            // If the nut can be pushed, move both the nut and the character
            if (pushQuery.IsPushable)
            {
                Move(inputDirection, distance, moveDuration);
                nutEvents.Raise(new NutEventPayload
                {
                    EventType = NutEventType.NutPush,
                    Position = targetPos,
                    CurrentPosition = targetPos,
                    Direction = inputDirection,
                    Distance = distance,
                    Duration = moveDuration
                });

            }
            else
            {
                // Cannot push - optionally animate failed push
                Debug.Log("Cannot push nut in this direction");
            }
        }
        // Rotate to input arrow direction
        else
        {

            characterEvents.Raise(new CharacterEventPayload
            {
                EventType = CharacterEventType.MoveStarted
            });
            
            facingDirection = Rotate(inputDirection);
            audioEvents.Raise(new AudioEventPayload { EventType = AudioEventType.PlaySfx, Sfx = SfxType.VeverkaRotate });

            characterEvents.Raise(new CharacterEventPayload
            {
                EventType = CharacterEventType.MoveCompleted
            });
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
