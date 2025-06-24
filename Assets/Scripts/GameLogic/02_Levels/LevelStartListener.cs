using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStartListener : MonoBehaviour
{
    #region fields
    [SerializeField]
    private LevelStartEvent levelStartEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;

    #endregion

    #region methods
    /// <summary>
    /// This method is called when object becames enabled or active
    /// </summary>
    private void OnEnable()
    {
        levelStartEvent.AddListener(LevelStart);
    }

    /// <summary>
    /// this method is called when obect is disabled or inactive
    /// </summary>
    private void OnDisable()
    {
       levelStartEvent.RemoveListener(LevelStart);
    }

    /// <summary>
    /// level start event execute
    /// </summary>
    /// <param name="payload"></param>
    private void LevelStart(LevelStartPayload payload)
    {
        levelDatabase.CurrentLevelIndex = payload.levelNumber;
        SceneManager.LoadScene("LevelLoading");
    }
    #endregion
}
