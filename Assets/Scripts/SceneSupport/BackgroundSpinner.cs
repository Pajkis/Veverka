using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// rotates the spinner durring loading
/// </summary>
public class BackgroundSpinner : MonoBehaviour
{
    float rotationSpeed = -180f; // degrees per second

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
