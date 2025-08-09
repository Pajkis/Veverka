using UnityEngine;
using Veverka.GridSystem.GameGrid;

/// <summary>
/// Class for the nut Tile - basic push to goal tile.
/// </summary>
public class NutTile : PushableTile
{
    [SerializeField] private PushableSetEvent pushableSetEvent;
    [SerializeField] private PushableRemovedEvent pushableRemovedEvent;

    #region Initialization
    public override void Init(TileType tileType, Vector2Int gridPosition)
    {
        base.Init(tileType, gridPosition);
        pushableSetEvent.Raise(new PushableSetPayload
        {
            Position = gridPosition,
            Pushable = this,
        });
    }
    #endregion

    #region methods
    /// <summary>
    /// On Move start action
    /// </summary>
    protected override void OnMoveStart()
    {
        AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.NutMove);
        base.OnMoveStart();
    }

    /// <summary>
    /// On move complete method
    /// </summary>
    /// <param name="targetPosition"></param>
    protected override void OnMoveComplete(Vector2Int targetPosition)
    {
        // remove pushable from previous position
        pushableRemovedEvent.Raise(new PushableRemovedPayload
        {
            Position = GridPosition,
        });

        // the nut does not reach goal
        if (!GameGrid.Instance.IsGoalAt(targetPosition))
        {
            pushableSetEvent.Raise(new PushableSetPayload
            {
                Position = targetPosition,
                Pushable = this,
            });
        }
        else
        {
            // nut enters goal event raise
            pushableInGoalEvent.Raise(new PushableInGoalPayload
            {
                Position = targetPosition,
                PushableTileType = this,
                GoalReduction = 1,
            });

            Destroy(gameObject);

            AudioManager.Instance.PlaySound(SoundChannel.SoundEffect, SfxEnum.GoalReached);
        }

        GridPosition = targetPosition;

        //Complete turn record
        TurnRecordAddandComplete();
    }
    #endregion
}

