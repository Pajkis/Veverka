using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veverka.GridSystem.GameGrid;

/// <summary>
/// Builds the level from grid
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    /// <summary>
    /// Initializes the game grid using the cached level layout.
    /// </summary>
    void Start()
    {
     //   GameGrid.Instance.InitializeGrid(LevelUtils.CachedGrid);
    }
}
