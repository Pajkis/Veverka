using UnityEngine;

public class LevelBackgroundChange : MonoBehaviour
{

    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private LevelDatabase levelDatabase;

     // Update is called once per frame
    void Update()
    {
        if (backgroundSprites == null) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && backgroundSprites.Length > 0)
        {
            sr.sprite = backgroundSprites[((int)levelDatabase.LevelSetType)];
        }

    }
}
