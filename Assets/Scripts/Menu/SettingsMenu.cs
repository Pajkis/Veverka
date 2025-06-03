using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        Debug.Log("Back button clicked");
        Destroy(gameObject);
    }

}
