using UnityEngine;

public class BackOnlyMenu : MonoBehaviour
{
    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {       
        Destroy(gameObject);
    }

}
