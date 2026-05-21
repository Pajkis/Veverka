using UnityEngine;

/// <summary>
/// ScriptableObject representing a set of tutorials (e.g., Stone set, Basic set)
/// </summary>
[CreateAssetMenu(menuName = "Tutorial/Config/Set")]
public class TutorialSet : ScriptableObject
{
    [Header("Set Information")]
    public TutorialSetType setType;
    public string setDisplayName;

    [Header("Tutorial CSV Files")]
    [SerializeField] private TextAsset[] tutorialCsvFiles;

    [Header("Tutorial Sequence Data")]
    [SerializeField] private TutorialSequenceData[] tutorialSequences;

    /// <summary>
    /// Get the CSV file for a specific tutorial number
    /// </summary>
    /// <param name="tutorialNumber">Tutorial number (0-based index)</param>
    /// <returns>TextAsset containing the CSV data, or null if not found</returns>
    public virtual TextAsset GetTutorialCsv(int tutorialNumber)
    {
        if (tutorialNumber < 0 || tutorialNumber >= tutorialCsvFiles.Length)
        {
          //  DebugLogger.LogError(DebugLogCategory.TutorialSystem, $"TutorialSet: Tutorial {tutorialNumber} not found in set {setType}. Available tutorials: 0-{tutorialCsvFiles.Length - 1}");
            return null;
        }

        return tutorialCsvFiles[tutorialNumber];
    }

    /// <summary>
    /// Get the total number of tutorials in this set
    /// </summary>
    public virtual int TutorialCount => tutorialCsvFiles.Length;

    /// <summary>
    /// Check if a tutorial number exists in this set
    /// </summary>
    /// <param name="tutorialNumber">Tutorial number to check</param>
    /// <returns>True if tutorial exists</returns>
    public virtual bool HasTutorial(int tutorialNumber)
    {
        return tutorialNumber >= 0 && tutorialNumber < tutorialCsvFiles.Length;
    }

    /// <summary>
    /// Get the sequence data for a specific tutorial number
    /// </summary>
    /// <param name="tutorialNumber">Tutorial number (0-based index)</param>
    /// <returns>TutorialSequenceData or null if not found</returns>
    public virtual TutorialSequenceData GetTutorialSequence(int tutorialNumber)
    {
        if (tutorialSequences == null || tutorialNumber < 0 || tutorialNumber >= tutorialSequences.Length)
        {
            return null;
        }
        return tutorialSequences[tutorialNumber];
    }
}