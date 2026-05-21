using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for handling all tutorial sets
/// </summary>
[CreateAssetMenu(menuName = "Tutorial/Config/Manager")]
public class TutorialSetManager : ScriptableObject
{
    [Header("Available Tutorial Sets")]
    [SerializeField] private List<TutorialSet> tutorialSets = new List<TutorialSet>();

    // Dictionary for faster lookups
    private Dictionary<TutorialSetType, TutorialSet> setLookup;

    /// <summary>
    /// Initialize the lookup dictionary
    /// </summary>
    private void OnEnable()
    {
        InitializeLookup();
    }

    /// <summary>
    /// Initialize the lookup dictionary for faster access
    /// </summary>
    private void InitializeLookup()
    {
        setLookup = new Dictionary<TutorialSetType, TutorialSet>();

        foreach (var tutorialSet in tutorialSets)
        {
            if (tutorialSet != null)
            {
                if (setLookup.ContainsKey(tutorialSet.setType))
                {
                   // DebugLogger.LogWarning(DebugLogCategory.TutorialSystem, $"{nameof(TutorialSetManager)}: Duplicate tutorial set type found: {tutorialSet.setType}");
                    continue;
                }
                setLookup[tutorialSet.setType] = tutorialSet;
            }
        }
    }

    /// <summary>
    /// Get a specific tutorial set by type
    /// </summary>
    /// <param name="setType">Type of the set to retrieve</param>
    /// <returns>TutorialSet or null if not found</returns>
    public TutorialSet GetTutorialSet(TutorialSetType setType)
    {  

        // Ensure lookup is initialized
        if (setLookup == null)
        {
            InitializeLookup();
        }

        setLookup.TryGetValue(setType, out TutorialSet tutorialSet);

        if (tutorialSet == null)
        {
          //  DebugLogger.LogError(DebugLogCategory.TutorialSystem, $"{nameof(TutorialSetManager)}: Tutorial set of type {setType} not found!");
        }

        return tutorialSet;
    }

    /// <summary>
    /// Get CSV data for a specific tutorial
    /// </summary>
    /// <param name="setType">Type of the tutorial set</param>
    /// <param name="tutorialNumber">Tutorial number within the set</param>
    /// <returns>TextAsset containing CSV data or null if not found</returns>
    public TextAsset GetTutorialCsv(TutorialSetType setType, int tutorialNumber)
    {
        var tutorialSet = GetTutorialSet(setType);
        if (tutorialSet == null)
        {
            return null;
        }

        return tutorialSet.GetTutorialCsv(tutorialNumber);
    }

    /// <summary>
    /// Check if a tutorial exists
    /// </summary>
    /// <param name="setType">Type of the tutorial set</param>
    /// <param name="tutorialNumber">Tutorial number within the set</param>
    /// <returns>True if tutorial exists</returns>
    public bool HasTutorial(TutorialSetType setType, int tutorialNumber)
    {
        var tutorialSet = GetTutorialSet(setType);
        return tutorialSet != null && tutorialSet.HasTutorial(tutorialNumber);
    }

    /// <summary>
    /// Get the total number of tutorials in a specific set
    /// </summary>
    /// <param name="setType">Type of the tutorial set</param>
    /// <returns>Number of tutorials in the set, or 0 if set not found</returns>
    public int GetTutorialCount(TutorialSetType setType)
    {
        var tutorialSet = GetTutorialSet(setType);
        return tutorialSet?.TutorialCount ?? 0;
    }

    /// <summary>
    /// Get all available tutorial sets
    /// </summary>
    /// <returns>List of all available tutorial sets</returns>
    public List<TutorialSet> GetAllTutorialSets()
    {
        return new List<TutorialSet>(tutorialSets);
    }

    /// <summary>
    /// Get sequence data for a specific tutorial
    /// </summary>
    /// <param name="setType">Type of the tutorial set</param>
    /// <param name="tutorialNumber">Tutorial number within the set</param>
    /// <returns>TutorialSequenceData or null if not found</returns>
    public TutorialSequenceData GetTutorialSequence(TutorialSetType setType, int tutorialNumber)
    {
        var tutorialSet = GetTutorialSet(setType);
        if (tutorialSet == null)
        {
            return null;
        }
        return tutorialSet.GetTutorialSequence(tutorialNumber);
    }
}