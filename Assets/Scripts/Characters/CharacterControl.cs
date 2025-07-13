using UnityEngine;
using Veverka.Movement.SmoothMover;

/// <summary>
/// Character Control anstract class 
/// </summary>
public abstract class CharacterControl : MonoBehaviour
{
    #region Fields
    protected TileType tileType;
    protected CharacterType characterType;
    protected Vector2Int gridPosition;
    protected Direction facingDirection;

    [SerializeField] protected float moveDuration = 0.15f;
    [SerializeField] protected int moveDistance = 1;

    protected SmoothMover smoothMover;

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


    #region Methods
    /// <summary>
    /// Init Character in the grid
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="characterType"></param>
    /// <param name="gridPosition"></param>
    /// <param name="facingDirection"></param>
    public virtual void Init(TileType tileType, CharacterType characterType, Vector2Int gridPosition, Direction facingDirection = Direction.Down)
    {
        this.tileType = tileType;
        this.characterType = characterType;
        this.gridPosition = gridPosition;
        this.facingDirection = facingDirection;

        smoothMover = GetComponent<SmoothMover>();
        Rotate(facingDirection);
    }

    /// <summary>
    /// Move of a character over a distance in set direction
    /// </summary>
    /// <param name="direction">direction of movemebt</param>
    /// <param name="distance">distance of movement in tiles</param>
    /// <param name="duration">duration of movement</param>
    protected virtual void Move(Direction direction, int distance, float duration = 0.15f)
    {
        if (smoothMover.IsMoving) return;
        Vector3 currentPosition = transform.localPosition;
        Vector2Int targetPosVec2Int = GridUtils.GetPositionInDir(gridPosition, direction, moveDistance);
        Vector3 targetPosition = GridUtils.GridToWorld(targetPosVec2Int);
        float moveDuration = (duration * distance) / GameSettings.Instance.Get(GameSettingsEnum.AnimationSpeed) ;

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
