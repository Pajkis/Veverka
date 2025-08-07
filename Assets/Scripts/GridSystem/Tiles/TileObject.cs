using UnityEngine;
/// <summary>
/// tile object basic abstract class
/// </summary>
public abstract class TileObject : MonoBehaviour
{
    #region fields
    protected TileType tileType;
    protected Vector2Int gridPosition;

    protected string UndoId;
    #endregion

    #region events
    [SerializeField]
    protected TileObjectAtEvent tileObjectAt;
    #endregion

    #region propeties
    /// <summary>
    /// Get grid position of the object
    /// </summary>
    public virtual Vector2Int GridPosition 
    { 
        get { return gridPosition; }  // public virtual Vector2Int GridPosition => gridPosition;
        //set { gridPosition = value; } 
    }

    /// <summary>
    /// Get object tile type
    /// </summary>
    public virtual TileType PosTileType
    {
        get {return tileType;}
    }
    #endregion

    #region Initialiazation
    /// <summary>
    /// Initialize tile type on grid position
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public virtual void Init(TileType tileType, Vector2Int gridPosition)
    {
        this.tileType = tileType;
        this.gridPosition = gridPosition;
    }

    /// <summary>
    /// Undo action of an object 
    /// </summary>
    public virtual void UndoAction() { }
    #endregion
}
