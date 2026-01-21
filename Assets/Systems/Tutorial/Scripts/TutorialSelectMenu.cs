using UnityEngine;


/// <summary>
/// Simple tutorial selector - provides public functions callable from button onClick events.
/// No complex UI management - just direct tutorial loading by set and tutorial number.
/// Example usage: Button onClick → TutorialSelectMenu.LoadTutorial(TutorialSetType.Basic, 0)
/// </summary>
public class TutorialSelectMenu : MonoBehaviour
{
    [Header("References")]    
    [SerializeField] private TutorialSelectData tutorialSelection;
    [SerializeField] private SceneNavigationEvents sceneNavigationEvents;
    [SerializeField] private UniversalTabManager tabManager;

    [Header("Tutorial set mapping")]
    [SerializeField] private TutorialSetMapping[] tutorialSetMappings;

    [System.Serializable]
    private class TutorialSetMapping
    {
        [Tooltip("Type of the tutorial set")]
        public TutorialSetType setType;
        
        [Tooltip("Tab value")]
        public int tabValue;

        [Tooltip("Description for debugging purposes")]
        public string description;
    }


    /// <summary>
    /// Set initial tab based on current tutorial set in database
    /// </summary>
    private void SetInitialTab()
    {
        if (tutorialSelection  != null && tabManager != null)
        {
            int tabValue = GetTabValueForTutorialSet(tutorialSelection.TutorialSetType);
            if (tabValue != -1)
            {
                tabManager.ShowTabByValue(tabValue);
                DebugLogger.Log(DebugLogCategory.SceneManager, $"TutorialSelectMenu: Initial tab set to tutorial set {tutorialSelection.TutorialSetType}", this);
            }
            else
            {
                DebugLogger.LogWarning(DebugLogCategory.SceneManager, $"No tab mapping found for tutorial set: {tutorialSelection.TutorialSetType}", this);
            }
        }
    }

    /// <summary>
    /// React to tab change events from tab manager
    /// </summary>
    /// <param name="tabIndex">Index of newly selected tab</param>
    private void OnTabChanged(int tabIndex)
    {
        if (tutorialSelection != null && tabManager != null)
        {
            int tabValue = tabManager.GetCurrentTabValue();
            TutorialSetType tutorialSetType = GetTutorialSetForTabValue(tabValue);

            if (tutorialSetType != tutorialSelection.TutorialSetType)
            {
                tutorialSelection.TutorialSetType = tutorialSetType;
                DebugLogger.Log(DebugLogCategory.SceneManager, $"TutorialSelectMenu: Tutorial set changed to {tutorialSetType}", this);
            }
        }
    }

    /// <summary>
    /// Get tab value for specific tutorial set type
    /// </summary>
    /// <param name="setType">Tutorial set type to find</param>
    /// <returns>Tab value or -1 if not found</returns>
    private int GetTabValueForTutorialSet(TutorialSetType setType)
    {
        foreach (var mapping in tutorialSetMappings)
        {
            if (mapping.setType == setType)
                return mapping.tabValue;
        }
        return -1;
    }

    /// <summary>
    /// Get tutorial set type for specific tab value
    /// </summary>
    /// <param name="tabValue">Tab value to find</param>
    /// <returns>Tutorial set type or Basic as fallback</returns>
    private TutorialSetType GetTutorialSetForTabValue(int tabValue)
    {
        foreach (var mapping in tutorialSetMappings)
        {
            if (mapping.tabValue == tabValue)
                return mapping.setType;
        }
        return TutorialSetType.Basic; // fallback
    }

    /// <summary>
    /// Handler for tutorial button clicks - called from Unity events
    /// </summary>
    /// <param name="tutorialNumber">Tutorial number to load (0-based index)</param>
    public void HandleTutorialButtonClick(int tutorialNumber)
    {
        if (tutorialSelection != null && sceneNavigationEvents != null)
        {
            // Update tutorial set type from current tab BEFORE opening overlay
            TutorialSetType currentSet = GetCurrentTutorialSetType();
            tutorialSelection.TutorialSetType = currentSet;
            tutorialSelection.TutorialIndex = tutorialNumber;

            DebugLogger.Log(DebugLogCategory.Tutorial, $"TutorialSelectMenu: Loading tutorial {tutorialNumber} from set {currentSet}", this);

            sceneNavigationEvents.Raise(new SceneNavigationEventPayload
            {
                EventType = SceneNavigationEventType.OpenOverlay,
                Overlay = OverlayType.Tutorial
            });
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial, "TutorialSelection or SceneNavigationEvent not assigned to TutorialSelectMenu!", this);
        }
    }

         /// <summary>
         /// Get currently active tutorial set type
         /// </summary>
         /// <returns>Current tutorial set type</returns>
    public TutorialSetType GetCurrentTutorialSetType()
    {
        if (tabManager != null)
        {
            int tabValue = tabManager.GetCurrentTabValue();
            return GetTutorialSetForTabValue(tabValue);
        }
        return TutorialSetType.Basic;
    }

    /// <summary>
    /// Get all available tutorial set types
    /// </summary>
    /// <returns>Array of available tutorial set types</returns>
    public TutorialSetType[] GetAvailableTutorialSets()
    {
        var availableSets = new System.Collections.Generic.List<TutorialSetType>();
        
        return availableSets.ToArray();
    }

    /// <summary>
    /// Force refresh of tab manager state based on current tutorial database
    /// Useful when tutorial database is changed externally
    /// </summary>
    public void RefreshTabState()
    {
        if (tutorialSelection != null && tabManager != null)
        {
            int targetTabValue = GetTabValueForTutorialSet(tutorialSelection.TutorialSetType);
            int currentTabValue = tabManager.GetCurrentTabValue();

            if (targetTabValue != currentTabValue && targetTabValue != -1)
            {
                tabManager.ShowTabByValue(targetTabValue);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug - Tutorial Set Mappings")]
    private void DebugTutorialSetMappings()
    {
        DebugLogger.Log(DebugLogCategory.UI, "=== LEVEL SET MAPPINGS ===", this);
        DebugLogger.Log(DebugLogCategory.UI, $"Current Tutorial Set: {(tutorialSelection != null ? tutorialSelection.TutorialSetType.ToString() : "NULL")}", this);
        DebugLogger.Log(DebugLogCategory.UI, $"Current Tab Value: {(tabManager != null ? tabManager.GetCurrentTabValue() : -1)}", this);

        for (int i = 0; i < tutorialSetMappings.Length; i++)
        {
            var mapping = tutorialSetMappings[i];          
        }
    }

    [ContextMenu("Test - Switch to Next Tutorial Set")]
    private void TestSwitchToNextTutorialSet()
    {
        if (tabManager != null)
        {
            tabManager.ShowNextTab();
        }
    }
#endif
}