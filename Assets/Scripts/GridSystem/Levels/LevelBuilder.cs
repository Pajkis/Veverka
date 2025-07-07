using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds the level from grid
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameGrid.Instance.InitializeGrid(LevelUtils.CachedGrid);
    }  
}
