using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Character Control anstract class 
/// gameplay/Characters/Scripts/Character
/// </summary>
public abstract class Character : MonoBehaviour
{
    #region Fields
    protected TileType tileType;
    protected CharacterType characterType;
    protected Vector2Int gridPosition;
    protected Direction facingDirection;

    [SerializeField] protected float moveDuration = 0.15f;
    [SerializeField] protected float rotationDuration = 0.15f;
    [SerializeField] protected int moveDistance = 1;
    protected CharacterEventPayload payload = new();

    protected SmoothMover smoothMover;
    protected SmoothRotate smoothRotate;
   
    #endregion

    #region Events
    [Header("Events")]
    [SerializeField] protected GridEvents gridEvents;
    [SerializeField] protected AudioEvents audioEvents;   
    [SerializeField] protected CharacterEvents characterEvents;
    [SerializeField] protected UndoEvents undoEvents;
    #endregion

    #region Configs
    [Header("Gameplay Configuration")]
    [SerializeField] protected GameplayConfig gameplayConfig;
    
    [Header("Debug")]
    [SerializeField] protected DebugLogConfig debugConfig;
    #endregion


    #region Init

    /// <summary>
    /// Initialize character with basic gameplay logging
    /// </summary>
    protected virtual void Awake()
    {
        DebugLogger.Log(DebugLogCategory.Gameplay, $"Character {characterType} created", this);
        smoothMover = GetComponent<SmoothMover>();
        smoothRotate = GetComponent<SmoothRotate>();
    }

    #endregion

    #region properties
    /// <summary>
    /// Facing direction property
    /// </summary>
    public Direction FacingDirection
    {
        get { return facingDirection; }
    }

    public Vector2Int GridPosition
    {
        get { return gridPosition; }
    }

    /// <summary>
    /// Public accessor for SmoothMover component
    /// </summary>
    public SmoothMover SmoothMover => smoothMover;

    /// <summary>
    /// Public accessor for SmoothRotate component
    /// </summary>
    public SmoothRotate SmoothRotate => smoothRotate;
    #endregion

    #region event handling

    /// <summary>
    /// On enable event handling
    /// </summary>
    protected virtual void OnEnable()
    {
        // Initialize debug logger if config is available
        if (debugConfig != null) DebugLogger.Initialize(debugConfig);

        // Register event listeners       
        undoEvents?.AddListener(OnUndoEvent);       
        characterEvents.AddListener(OnCharacterEvent);
  
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// On disable event handling
    /// </summary>
    protected virtual void OnDisable()
    {       
        undoEvents?.RemoveListener(OnUndoEvent);      
        characterEvents.RemoveListener(OnCharacterEvent);

        // Remove scene change listener
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { }

    /// <summary>
    /// On character event handler
    /// </summary>
    /// <param name="payload"></param>
    protected virtual void OnCharacterEvent(CharacterEventPayload payload)
    { }
    
    /// <summary>
    /// Handle undo events for this character
    /// </summary>
    /// <param name="payload"></param>
    protected void OnUndoEvent(UndoEventPayload payload)
    {
        if (payload.EventType != UndoEventType.RequestCharacterUndo)
            return;

        // Check if this undo request is for this character
        var expectedObjectId = $"Character-{GetInstanceID()}";
        if (payload.UndoData.ObjectId != expectedObjectId)
            return;

        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Processing undo request {payload.RequestId} for {payload.UndoData.ActionType}", this);

        switch (payload.UndoData.ActionType)
        {
            case UndoActionType.CharacterMove:
                ExecuteUndoMove(payload.UndoData, payload.RequestId);
                break;
            case UndoActionType.CharacterRotation:
                ExecuteUndoRotation(payload.UndoData, payload.RequestId);
                break;
        }
    }

    private void ExecuteUndoMove(UndoData undoData, string requestId)
    {
        if (smoothMover.IsMoving)
        {
            DebugLogger.LogWarning(DebugLogCategory.UndoLogic, "Character already moving, skipping undo", this);
            CompleteUndoRequest(requestId);
            return;
        }

        Vector3 fromWorld = GridUtils.GridToWorld(undoData.CurrentPosition);
        Vector3 toWorld = GridUtils.GridToWorld(undoData.PreviousPosition);

        smoothMover.Move(fromWorld, toWorld, undoData.Duration,
            onStart: null,
            onComplete: () => OnUndoMoveComplete(undoData, requestId));

        gridPosition = undoData.PreviousPosition;
        facingDirection = undoData.CurrentDirection;

        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Started undo move from {undoData.CurrentPosition} to {undoData.PreviousPosition}", this);
    }

    private void ExecuteUndoRotation(UndoData undoData, string requestId)
    {
        if (smoothRotate != null)
        {
            smoothRotate.Rotate(undoData.CurrentPosition, undoData.CurrentDirection, undoData.PreviousDirection, undoData.Duration,
                onStart: null,
                onComplete: () => OnUndoRotationComplete(undoData, requestId));

            facingDirection = undoData.PreviousDirection;

            DebugLogger.Log(DebugLogCategory.UndoLogic,
                $"Started undo rotation from {undoData.CurrentDirection} to {undoData.PreviousDirection}", this);
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.UndoLogic, "SmoothRotate component not found", this);
            CompleteUndoRequest(requestId);
        }
    }

    private void OnUndoMoveComplete(UndoData undoData, string requestId)
    {
        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Undo move completed: {undoData.CurrentPosition} → {undoData.PreviousPosition}", this);
        CompleteUndoRequest(requestId);
    }

    private void OnUndoRotationComplete(UndoData undoData, string requestId)
    {
        DebugLogger.Log(DebugLogCategory.UndoLogic,
            $"Undo rotation completed: {undoData.CurrentDirection} → {undoData.PreviousDirection}", this);
        CompleteUndoRequest(requestId);
    }

    private void CompleteUndoRequest(string requestId)
    {
        undoEvents?.Raise(new UndoEventPayload
        {
            EventType = UndoEventType.UndoCompleted,
            RequestId = requestId
        });

        DebugLogger.Log(DebugLogCategory.UndoLogic, $"Undo request {requestId} completed", this);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Init Character in the grid
    /// </summary>
    /// <param name="tileType">type of tile in the grid </param>
    /// <param name="characterType">character type</param>
    /// <param name="gridPosition">position in the grid </param>
    /// <param name="facingDirection">facing direction of the character</param>
    public virtual void Init(TileType tileType, CharacterType characterType, Vector2Int gridPosition, Direction facingDirection = Direction.Down)
    {
        //Set initial values
        this.tileType = tileType;
        this.characterType = characterType;
        this.gridPosition = gridPosition;
        this.facingDirection = facingDirection;

        //set configs
        moveDuration = gameplayConfig.characterMoveTime;
        rotationDuration = gameplayConfig.characterRotateTime;
     
        smoothMover = GetComponent<SmoothMover>();
        if (smoothMover == null)
        {
            DebugLogger.LogError(DebugLogCategory.CharacterMovement, $"SmoothMover component missing on {gameObject.name}", this);
            return;
        }
        smoothRotate = GetComponent<SmoothRotate>();
        if (smoothRotate == null)
        {
            DebugLogger.LogError(DebugLogCategory.Rotation, $"SmoothRotate component missing on {gameObject.name}", this);
            return;
        }
        smoothRotate.Rotate(gridPosition, facingDirection, facingDirection, 0f,
            onStart: null, onComplete: null);

        DebugLogger.Log(DebugLogCategory.CharacterMovement, $"Character initialized - GridPos: {gridPosition}, WorldPos: {transform.position}", this);
  
    }  

    /// <summary>
    /// Move of a character over a distance in set direction
    /// </summary>
    /// <param name="direction">direction of movement</param>
    /// <param name="distance">distance of movement in tiles</param>
    /// <param name="duration">effective duration per tile (already adjusted by speedMultiplier in caller)</param>
    protected virtual void Move(Direction direction, int distance, float duration)
    {
        // decide position to move
        Vector3 currentPosition = transform.localPosition;
        Vector2Int targetPosVec2Int = GridUtils.GetPositionInDir(gridPosition, direction, distance);
        Vector3 targetPosition = GridUtils.GridToWorld(targetPosVec2Int);
        float effectiveDuration = duration * distance;

        DebugLogger.Log(DebugLogCategory.CharacterMovement, $"Move() preparing - gridPos={gridPosition}, transform.localPos={transform.localPosition}, currentPos={currentPosition}, targetPos={targetPosition}, effectiveDur={effectiveDuration}", this);

          // fill character moved payload for events - calling events in children classes
          payload = new CharacterEventPayload
          {
              Character = this,
              EventType = CharacterEventType.MoveStarted,
              CurrentPosition = targetPosVec2Int,
              PreviousPosition = gridPosition,
              CurrentDirection = direction,
              PreviousDirection = facingDirection, // Store current facing as previous for undo
              RequestData = false,
              ResponseData = false,
          };

        //Raise move started event
        characterEvents.Raise(payload);
        DebugLogger.Log(DebugLogCategory.EventSystem, $"{GetType().Name} move start to {gridPosition}", this);
        DebugLogger.Log(DebugLogCategory.CharacterMovement, $"MoveStarted  for {GetType().Name}", this);

        // execute smooth movement
        smoothMover.Move(currentPosition, targetPosition, effectiveDuration, OnMoveStart, () => OnMoveComplete(targetPosVec2Int));
    }

    /// <summary>
    /// On move start actions
    /// </summary>
    protected virtual void OnMoveStart()
    { 
        //Default: nothing
    }

    /// <summary>
    ///On complete move action
    /// </summary>
    /// <param name="targetPosition"></param>
    protected virtual void OnMoveComplete(Vector2Int targetPosition)
    {
        this.gridPosition = targetPosition;
        // Update facing direction to the movement direction
        this.facingDirection = payload.CurrentDirection;

        // Update payload for MoveCompleted event
        payload.EventType = CharacterEventType.MoveCompleted;
        payload.CurrentPosition = targetPosition;
        // CurrentDirection and PreviousDirection are already set from MoveStarted

        // Raise move completed event - this will be handled by EventBasedTurnRecorder    
        characterEvents.Raise(payload);
        DebugLogger.Log(DebugLogCategory.EventSystem, $"MoveCompleted event sent for {GetType().Name}", this);
        DebugLogger.Log(DebugLogCategory.CharacterMovement, $"{GetType().Name} move complete to {gridPosition}", this);

    }
    #endregion

    #region Rotate
    protected virtual void OnRotateStart()
    {
        //Default: nothing
    }

    protected virtual void OnRotateComplete(Vector2Int targetTransform)
    {
        //Default: nothing
    }

    #endregion
    /// <summary>
    /// rotation of object to the direction
    /// </summary>
    /// <param name="currentDirection">Current facing direction</param>
    /// <param name="targetDirection">Target direction to rotate to</param>
    /// <param name="duration">effective rotation duration (already adjusted by speedMultiplier in caller)</param>
    protected virtual void Rotate(Direction currentDirection, Direction targetDirection, float duration)
    {
        //Default: nothing
    }
}
