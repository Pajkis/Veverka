using UnityEngine;

/// <summary>
/// Interface for tiles that interact with nuts
/// </summary>
public interface INutInteractive 
{
    /// <summary>
    /// Position of the tile on the grid
    /// </summary>
    Vector2Int GridPos { get; }

    /// <summary>
    /// Events system for nut interactions
    /// </summary>
    NutEvents NutEvents { get; }

    /// <summary>
    /// Gets the collection of events associated with the grid.
    /// </summary>
    GridEvents GridEvents { get; }
 }
