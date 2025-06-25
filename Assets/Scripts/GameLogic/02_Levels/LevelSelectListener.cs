using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectListener : MonoBehaviour
{
    #region fields
    [SerializeField]
    private LevelSelectEvent levelSelectEvent;

    [SerializeField]
    private LevelDatabase levelDatabase;

    #endregion

    #region methods
    /// <summary>
    /// This method is called when object becames enabled or active
    /// </summary>
    private void OnEnable()
    {
        levelSelectEvent.AddListener(LevelSelect);
    }

    /// <summary>
    /// this method is called when obect is disabled or inactive
    /// </summary>
    private void OnDisable()
    {
       levelSelectEvent.RemoveListener(LevelSelect);
    }

    /// <summary>
    /// level start event execute
    /// </summary>
    /// <param name="payload"></param>
    private void LevelSelect(LevelSelectPayload payload)
    {
        if(!payload.resetRequested)
        { 
           levelDatabase.CurrentLevelIndex = payload.levelNumber;
           SceneManager.LoadScene("LevelLoading");
        }
    }
    #endregion
}
