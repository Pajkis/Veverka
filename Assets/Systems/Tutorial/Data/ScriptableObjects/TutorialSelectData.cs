
using UnityEngine;

/// <summary>
/// Represents configuration data for selecting a specific tutorial, including the tutorial set and its index.
/// </summary>
/// <remarks>This class is intended to be used as a Unity <see cref="ScriptableObject"/> asset for configuring
/// tutorial selection in the editor.</remarks>
[CreateAssetMenu(menuName = "Data/Tutorial/TutorialSelectData")]
public class TutorialSelectData : ScriptableObject
{
    [Header("Tutorial Selection Data")]
    [Tooltip("Tutorial Set and tutorial index")]
    public TutorialSetType TutorialSetType;
    public int TutorialIndex;
}
