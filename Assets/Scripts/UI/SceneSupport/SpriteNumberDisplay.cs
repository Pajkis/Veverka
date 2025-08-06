using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteNumberDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] numberSprites; // sprites numbers from 0-9
    [SerializeField] private GameObject digitPrefab; // prefab with Image
    [SerializeField] private Vector2 imageSize = new Vector2(50, 100); // image size, spacing by x coordinate

    private List<Image> digitImages = new();

    public void SetNumber(int number)
    {
        string numberStr = Mathf.Max(0, number).ToString();

        // Expand if needed
        while (digitImages.Count < numberStr.Length)
        {           
            GameObject digit = Instantiate(digitPrefab, transform);               
            digitImages.Add(digit.GetComponent<Image>());          
        }

        // Update digit sprites
        for (int i = 0; i < digitImages.Count; i++)
        {
            if (i < numberStr.Length)
            {
                int digit = numberStr[i] - '0';
                digitImages[i].sprite = numberSprites[digit];
                digitImages[i].gameObject.SetActive(true);
            }
            else
            {
                digitImages[i].gameObject.SetActive(false);
            }

            // Spacing of numbers - adding higher grade to the left
            if (digitImages[i] != null)
            {
                digitImages[i].rectTransform.sizeDelta = imageSize;
                digitImages[i].rectTransform.anchoredPosition = new Vector2((i - numberStr.Length) * imageSize.x, 0f);
            }
        }
    }
}
