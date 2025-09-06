using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Universal text display component
/// </summary>
public class UniversalTextDisplay : MonoBehaviour
{
    [Header("Text Component")]
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string fallbackText;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
        else
            textComponent.text = fallbackText;
    }

    // Display a number
    public void DisplayNumber(int number)
    {
        if (textComponent != null)
            textComponent.text = number.ToString();
        else
            textComponent.text = fallbackText;
    }

    // Display a string
    public void DisplayString(string text)
    {
        if (textComponent != null)
            textComponent.text = text;
        else
            textComponent.text = fallbackText;

    }

    // Display an enum
    public void DisplayEnum<T>(T enumValue) where T : Enum
    {
        if (textComponent != null)
            textComponent.text = enumValue.ToString();
        else
            textComponent.text = fallbackText;
    }
}