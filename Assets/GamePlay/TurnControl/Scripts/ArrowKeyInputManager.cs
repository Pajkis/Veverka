using UnityEngine;

/// <summary>
/// Polls keyboard arrow keys and raises a direction event.
/// </summary>
public class ArrowKeyInputManager : MonoBehaviour
{
    [SerializeField] private DirectionEvent arrowPressed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) arrowPressed.Raise(Direction.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) arrowPressed.Raise(Direction.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) arrowPressed.Raise(Direction.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) arrowPressed.Raise(Direction.Right);
    }
}

