using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Select sprite based on the level selected
/// </summary>
public class SelectLevelSprite : MonoBehaviour
{
    #region fields
    [SerializeField] private Sprite[] selectSprites;

    [SerializeField]
    LevelDatabase levelDatabase;

    readonly int doubleDigit = 10;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (selectSprites.Length == 0) return;

        Image levelSelect = GetComponent<Image>();
        levelSelect.sprite = selectSprites[levelDatabase.CurrentLevelIndex - 1];
       
        RectTransform rt = GetComponent<RectTransform>();
        if (levelDatabase.CurrentLevelIndex < doubleDigit) 
        {
            rt.sizeDelta = new Vector2(50f, 100f);
        }
        else
        {
            rt.sizeDelta = new Vector2(100f, 100f);
        }

      //  Debug.Log($"Select level sprite {levelDatabase.CurrentLevelIndex}");
    }
}
