using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sequence of actions for a tutorial. Linked to TutorialSet via TutorialSetType and index.
/// </summary>
[CreateAssetMenu(fileName = "TutorialSequence", menuName = "Tutorial/Config/Sequence")]
public class TutorialSequenceData : ScriptableObject
{
    [Header("Identity")]
    public string tutorialId;
    public string titleText;

    [Header("Action Sequence")]
    public List<TutorialAction> actions;
}
