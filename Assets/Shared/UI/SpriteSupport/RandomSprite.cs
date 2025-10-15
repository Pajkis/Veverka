
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] possibleSprites;

    void Start()
    {
        if (possibleSprites.Length == 0) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        }
    }
}
