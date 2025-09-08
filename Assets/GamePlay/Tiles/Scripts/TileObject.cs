using UnityEngine;
/// <summary>
/// tile object basic abstract class
/// </summary>
public abstract class TileObject : MonoBehaviour
{
    #region fields
    protected string UndoId;
    #endregion

    #region Events  
    
    [Header("Events")]
    [SerializeField] protected SettingDataRequestEvent settingDataRequestEvent;
    [SerializeField] protected SettingDataBroadcastEvent settingDataBroadcastEvent;
    [SerializeField] protected GridEvents gridEvents;
    [SerializeField] protected PlaySfxEvent playSfxEvent;
    #endregion

    #region Configs
    [SerializeField] protected GameplayConfig gameplayConfig;
    #endregion

    #region propeties
    /// <summary>
    /// Get grid position of the object
    /// </summary>
    public virtual Vector2Int GridPosition { get; protected set; }

    /// <summary>
    /// Get object tile type
    /// </summary>
    public virtual TileType PosTileType { get; protected set; }
    #endregion

    #region Initialiazation
    /// <summary>
    /// Initialize tile type on grid position
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="gridPosition"></param>
    public virtual void Init(TileType tileType, Vector2Int gridPosition)
    {
        PosTileType = tileType;
        GridPosition = gridPosition;
    }

    /// <summary>
    /// Undo action of an object 
    /// </summary>
    public virtual void UndoAction() { }
    #endregion
}
