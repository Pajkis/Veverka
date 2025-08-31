using UnityEngine;

/// <summary>
/// Adapter for connecting AnimatedUniversalTabManager with level set system
/// Handles communication between tab manager and level data
/// </summary>
public class LevelSetAdapter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private UniversalTabManager tabManager;
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private GoToSceneEvent goToSceneEvent;

    [Header("Level Set Mapping")]
    [SerializeField] private LevelSetMapping[] levelSetMappings;

    [System.Serializable]
    public class LevelSetMapping
    {
        [Tooltip("Type of the level set")]
        public LevelSetType setType;

        [Tooltip("Tab value in tab manager (must match tabValue in TabInfo)")]
        public int tabValue;

        [Tooltip("Description for debugging purposes")]
        public string description;
    }

    void Start()
    {
        // Subscribe to tab manager events
        if (tabManager != null)
        {
            tabManager.OnTabChanged.AddListener(OnTabChanged);
        }
        else
        {
            Debug.LogError("UniversalTabManager not assigned to LevelSetAdapter!");
        }

        // Set initial tab based on level database
        SetInitialTab();
    }

    /// <summary>
    /// Set initial tab based on current level set in database
    /// </summary>
    private void SetInitialTab()
    {
        if (levelDatabase != null && tabManager != null)
        {
            int tabValue = GetTabValueForLevelSet(levelDatabase.LevelSetType);
            if (tabValue != -1)
            {
                tabManager.ShowTabByValue(tabValue);
                Debug.Log($"Initial tab set to level set: {levelDatabase.LevelSetType}");
            }
            else
            {
                Debug.LogWarning($"No tab mapping found for level set: {levelDatabase.LevelSetType}");
            }
        }
    }

    /// <summary>
    /// React to tab change events from tab manager
    /// </summary>
    /// <param name="tabIndex">Index of newly selected tab</param>
    private void OnTabChanged(int tabIndex)
    {
        if (levelDatabase != null && tabManager != null)
        {
            int tabValue = tabManager.GetCurrentTabValue();
            LevelSetType levelSetType = GetLevelSetForTabValue(tabValue);

            if (levelSetType != levelDatabase.LevelSetType)
            {
                levelDatabase.LevelSetType = levelSetType;
                Debug.Log($"Level set changed to: {levelSetType}");
            }
        }
    }

    /// <summary>
    /// Get tab value for specific level set type
    /// </summary>
    /// <param name="setType">Level set type to find</param>
    /// <returns>Tab value or -1 if not found</returns>
    private int GetTabValueForLevelSet(LevelSetType setType)
    {
        foreach (var mapping in levelSetMappings)
        {
            if (mapping.setType == setType)
                return mapping.tabValue;
        }
        return -1;
    }

    /// <summary>
    /// Get level set type for specific tab value
    /// </summary>
    /// <param name="tabValue">Tab value to find</param>
    /// <returns>Level set type or Basic as fallback</returns>
    private LevelSetType GetLevelSetForTabValue(int tabValue)
    {
        foreach (var mapping in levelSetMappings)
        {
            if (mapping.tabValue == tabValue)
                return mapping.setType;
        }
        return LevelSetType.Basic; // fallback
    }

    /// <summary>
    /// Handler for level button clicks - called from Unity events
    /// </summary>
    /// <param name="levelNumber">Level number to load (0-based index)</param>
    public void HandleLevelButtonClick(int levelNumber)
    {
        if (levelDatabase != null && goToSceneEvent != null)
        {
            levelDatabase.CurrentLevelIndex = levelNumber;
            goToSceneEvent.Raise(SceneType.LoadLevel);

            Debug.Log($"Loading level {levelNumber} from set {levelDatabase.LevelSetType}");
        }
        else
        {
            Debug.LogError("LevelDatabase or GoToSceneEvent not assigned to LevelSetAdapter!");
        }
    }

    /// <summary>
    /// Handler for back button clicks
    /// </summary>
    public void HandleBackButton()
    {
        if (goToSceneEvent != null)
        {
            goToSceneEvent.Raise(SceneType.MainMenu);
        }
        else
        {
            Debug.LogError("GoToSceneEvent not assigned to LevelSetAdapter!");
        }
    }

    /// <summary>
    /// Unlock/lock specific level set
    /// </summary>
    /// <param name="setType">Level set type to modify</param>
    /// <param name="unlocked">New unlock state</param>
    public void SetLevelSetUnlocked(LevelSetType setType, bool unlocked)
    {
        if (tabManager != null)
        {
            int tabValue = GetTabValueForLevelSet(setType);
            if (tabValue != -1)
            {
                tabManager.SetTabAvailabilityByValue(tabValue, unlocked);
                Debug.Log($"Level set {setType} unlock state changed to: {unlocked}");
            }
            else
            {
                Debug.LogWarning($"No tab mapping found for level set: {setType}");
            }
        }
    }

    /// <summary>
    /// Get currently active level set type
    /// </summary>
    /// <returns>Current level set type</returns>
    public LevelSetType GetCurrentLevelSetType()
    {
        if (tabManager != null)
        {
            int tabValue = tabManager.GetCurrentTabValue();
            return GetLevelSetForTabValue(tabValue);
        }
        return LevelSetType.Basic;
    }

    /// <summary>
    /// Check if specific level set is currently unlocked
    /// </summary>
    /// <param name="setType">Level set type to check</param>
    /// <returns>True if unlocked, false otherwise</returns>
    public bool IsLevelSetUnlocked(LevelSetType setType)
    {
        if (tabManager == null) return false;

        int tabValue = GetTabValueForLevelSet(setType);
        if (tabValue == -1) return false;

        // Find the corresponding tab and check its availability
        for (int i = 0; i < tabManager.GetTabCount(); i++)
        {
            var tabInfo = tabManager.GetCurrentTabInfo();
            if (tabInfo != null && tabInfo.tabValue == tabValue)
            {
                return tabManager.IsTabAvailable(i);
            }
        }

        return false;
    }

    /// <summary>
    /// Get all available level set types
    /// </summary>
    /// <returns>Array of available level set types</returns>
    public LevelSetType[] GetAvailableLevelSets()
    {
        var availableSets = new System.Collections.Generic.List<LevelSetType>();

        if (tabManager != null)
        {
            foreach (var mapping in levelSetMappings)
            {
                if (IsLevelSetUnlocked(mapping.setType))
                {
                    availableSets.Add(mapping.setType);
                }
            }
        }

        return availableSets.ToArray();
    }

    /// <summary>
    /// Force refresh of tab manager state based on current level database
    /// Useful when level database is changed externally
    /// </summary>
    public void RefreshTabState()
    {
        if (levelDatabase != null && tabManager != null)
        {
            int targetTabValue = GetTabValueForLevelSet(levelDatabase.LevelSetType);
            int currentTabValue = tabManager.GetCurrentTabValue();

            if (targetTabValue != currentTabValue && targetTabValue != -1)
            {
                tabManager.ShowTabByValue(targetTabValue);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug - Level Set Mappings")]
    private void DebugLevelSetMappings()
    {
        Debug.Log("=== LEVEL SET MAPPINGS ===");
        Debug.Log($"Current Level Set: {(levelDatabase != null ? levelDatabase.LevelSetType.ToString() : "NULL")}");
        Debug.Log($"Current Tab Value: {(tabManager != null ? tabManager.GetCurrentTabValue() : -1)}");

        for (int i = 0; i < levelSetMappings.Length; i++)
        {
            var mapping = levelSetMappings[i];
            bool isUnlocked = IsLevelSetUnlocked(mapping.setType);
            Debug.Log($"Mapping {i}: {mapping.setType} → Tab Value {mapping.tabValue} | Unlocked: {isUnlocked} | {mapping.description}");
        }
    }

    [ContextMenu("Test - Switch to Next Level Set")]
    private void TestSwitchToNextLevelSet()
    {
        if (tabManager != null)
        {
            tabManager.ShowNextTab();
        }
    }
#endif
}