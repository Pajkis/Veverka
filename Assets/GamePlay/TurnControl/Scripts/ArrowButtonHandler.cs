
using UnityEngine;

/// <summary>
/// Handling of arrow button trigger
/// </summary>
public class ArrowButtonHandler : MonoBehaviour
{
    [SerializeField] private Direction direction;
    public DirectionEvent directionPressed;

    /// <summary>
    /// raise arrow trigger event
    /// </summary>
    public void OnClick()
    {
        directionPressed.Raise(direction);
        DebugLogger.Log(DebugLogCategory.General, $"Arrow pressed: {direction}", this);
    }
    
}
