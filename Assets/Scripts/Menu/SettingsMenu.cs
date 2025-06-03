using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    #region serialized fields
    [SerializeField] private string animationSpeed;
    [SerializeField] private string musicVolume;
    [SerializeField] private string effectVolume;

    #endregion




    /// <summary>
    /// Handle on click quit button event
    /// </summary>
    public void HandleQuitButtonOnClickEvent()
    {
        Debug.Log("Back button clicked");
        Destroy(gameObject);
    }

}
