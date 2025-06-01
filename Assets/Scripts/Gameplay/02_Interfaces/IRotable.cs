using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// add rotable feature to tile
/// </summary>
public interface IRotable
{
   public Direction FacingDirection { get; set; }

   public Direction Rotate(Direction direction);       

}
