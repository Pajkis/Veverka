using UnityEngine;

/// <summary>
/// Character Control anstract class 
/// </summary>
public abstract class Character : MonoBehaviour
{
    #region Fields
    protected TileType tileType;
    protected CharacterType characterType;
    protected Vector2Int gridPosition;
    protected Direction facingDirection;

    [SerializeField] protected float moveDuration = 0.15f;
    [SerializeField] protected int moveDistance = 1;
    protected float animationSpeed;
    protected CharacterBasicPayload payload = new();

    protected SmoothMover smoothMover;

    protected string UndoId;
    #endregion

    #region Events
    [Header("Events")]
    [SerializeField] protected TileQueryEvent tileQueryEvent;
    [SerializeField] protected PlaySfxEvent playSfxEvent;    
    [SerializeField] protected SettingDataRequestEvent settingDataRequestEvent;
    [SerializeField] protected SettingDataBroadcastEvent settingDataBroadcastEvent;
    [SerializeField] protected CharacterDataRequestEvent characterDataRequestEvent;
    #endregion

    #region Configs
    [Header("Configs")]
    [SerializeField] protected GameplayConfig gameplayConfig;
    #endregion


    #region properties
    /// <summary>
    /// Facing direction property
    /// </summary>
    public Direction FacingDirection
    {
        get { return facingDirection; }      
    }
    #endregion


    #region event handling
    /// <summary>
    /// Get animation speed from settings
    /// </summary>
    /// <param name="payload"></param>
    protected void OnSettingData(SettingDataPayload payload)
    {
        if (payload.Setting == GameSettingsEnum.AnimationSpeed)
        {
            animationSpeed = payload.Value;
        }
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
        moveDuration = gameplayConfig.moveTime;
     
        smoothMover = GetComponent<SmoothMover>();
        Rotate(facingDirection);
        Debug.Log($"[Character INIT] tilePos: {gridPosition}, worldPos: {transform.position}");
    }
  

    /// <summary>
    /// Undo move of character, set direction
    /// </summary>
    /// <param name="from"> current position</param>
    /// <param name="to">previous position</param>
    /// <param name="direction">direction of movement</param>
    /// <param name="duration"> duration of movement</param>
    public void UndoMove(Vector2Int current, Vector2Int previous, Direction direction, float duration = 0.15f)
    {
        if (smoothMover.IsMoving) return;

        Vector3 fromWorld = GridUtils.GridToWorld(current);
        Vector3 toWorld = GridUtils.GridToWorld(previous);

        Rotate(direction);
        smoothMover.Move(fromWorld, toWorld, duration,
            onStart: null, onComplete: null);

        Debug.Log($"Undo character: {previous} → {current}");
        gridPosition = previous;
    }


    /// <summary>
    /// Move of a character over a distance in set direction
    /// </summary>
    /// <param name="direction">direction of movement</param>
    /// <param name="distance">distance of movement in tiles</param>
    /// <param name="duration">duration of movement</param>
    protected virtual void Move(Direction direction, int distance, float duration)
    {
        //do not execute move when already moving
        if (smoothMover.IsMoving) return;

        // UndoID for turn history record
        UndoId = $"{GetType().Name}-{gridPosition.x}x{gridPosition.y}";
        // inform turn builder, that this component is going to register a action into turn record
        TurnBuilder.Instance.ExpectSource(UndoId);

        // decide position to move
        Vector3 currentPosition = transform.position;
        Vector2Int targetPosVec2Int = GridUtils.GetPositionInDir(gridPosition, direction, distance);
        Vector3 targetPosition = GridUtils.GridToWorld(targetPosVec2Int);
        float moveDuration = (duration * distance) / animationSpeed;

        // fill character moved payload for events - calling events in children classes 
        payload = new CharacterBasicPayload
        {
            Character = this,
            Current = targetPosVec2Int,
            Previous = gridPosition,
            Direction = direction,
            RequestData = false,
            ResponseData = true,
        };

        // execute smooth movement
        smoothMover.Move(currentPosition, targetPosition, moveDuration, OnMoveStart, () => OnMoveComplete(targetPosVec2Int));
    }

    /// <summary>
    /// On move start action
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
         Debug.Log($"[{this.GetType().Name} Move Complete] Now at grid pos: {gridPosition}");
       
        // register movement as action into turn record 
        TurnBuilder.Instance.AddAction(
         new UndoCharacterAction(this, payload.Current, payload.Previous, payload.Direction)
           );

        // inform turn builder, tht this component has registered an action into turn record
        TurnBuilder.Instance.NotifySourceComplete(UndoId);
        Debug.Log($"[{this.GetType().Name} move Complete] Added + Notified {UndoId}");

    }

    /// <summary>
    /// rotation of object to the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual Direction Rotate(Direction direction)
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

}
