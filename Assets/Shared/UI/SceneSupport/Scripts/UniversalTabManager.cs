using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Universal tab manager for GameHelp, LevelSet and other tab systems
/// Uses sprite swap instead of color tinting for better visual control
/// </summary>
public class UniversalTabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabInfo
    {
        [Tooltip("GameObject containing the tab content")]
        public GameObject content;

        [Tooltip("Button that activates this tab")]
        public Button button;

        [Tooltip("Integer value for this tab (used for external mapping)")]
        public int tabValue;

        [Tooltip("Is this tab currently available/unlocked?")]
        public bool isAvailable = true;
    }

    [Header("Tab Configuration")]
    [SerializeField] private TabInfo[] tabs;

    [Header("Basic Settings")]
    [SerializeField] private int defaultTabIndex = 0;

    [Header("Visual Feedback - Sprite Swap")]
    [Tooltip("Which sprite state to use for active tab (Selected, Pressed, Highlighted, or Disabled)")]
    [SerializeField] private SpriteStateType activeTabSpriteType = SpriteStateType.Selected;
    [SerializeField] private Sprite unavailableSprite; // Override for unavailable tabs

    [Header("Interactivity Settings")]
    [Tooltip("Allow hover effects on inactive tabs")]
    [SerializeField] private bool allowHoverOnInactive = false;

    [Header("Debug")]
    [SerializeField] private DebugLogConfig debugConfig;

    public enum SpriteStateType
    {
        Selected,
        Pressed,
        Highlighted,
        Disabled
    }

    [Header("Events")]
    [SerializeField] private UnityEvent<int> onTabChanged;
    [SerializeField] private UnityEvent<int> onTabClicked;
    [SerializeField] private UnityEvent<TabInfo> onTabChangedWithInfo;

    // Public properties for external access
    public UnityEvent<int> OnTabChanged => onTabChanged;
    public UnityEvent<int> OnTabClicked => onTabClicked;
    public UnityEvent<TabInfo> OnTabChangedWithInfo => onTabChangedWithInfo;

    private int currentTabIndex = -1;

    // Store original sprites and sprite states for each button
    private Sprite[] originalSprites;
    private SpriteState[] originalSpriteStates;

    // Identifier for debug logs
    private string TabManagerId => $"TabManager[{gameObject.name}]";

    /// <summary>
    /// Init and show default tab
    /// </summary>
    void Start()
    {
        DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} - Initializing with default tab: {defaultTabIndex}", this);
        InitializeTabs();

        // Add delay in case other components are not ready yet
        StartCoroutine(DelayedShowTab(defaultTabIndex));
    }

    /// <summary>
    /// Initialize tabs and connect button events
    /// </summary>
    private void InitializeTabs()
    {
        // Store original sprites and sprite states
        originalSprites = new Sprite[tabs.Length];
        originalSpriteStates = new SpriteState[tabs.Length];

        for (int i = 0; i < tabs.Length; i++)
        {
            int tabIndex = i; // Closure for lambda

            if (tabs[i].button != null)
            {
                tabs[i].button.onClick.AddListener(() => ShowTab(tabIndex));

                // Store original sprite (Source Image) and sprite state
                var buttonImage = tabs[i].button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    originalSprites[i] = buttonImage.sprite;
                }
                originalSpriteStates[i] = tabs[i].button.spriteState;
            }

            // Always deactivate other tabs initially
            if (tabs[i].content != null)
            {
                tabs[i].content.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Show tab after a short delay to ensure all components are ready
    /// </summary>
    /// <param name="tabIndex"></param>
    /// <returns></returns>
    private System.Collections.IEnumerator DelayedShowTab(int tabIndex)
    {
        yield return new WaitForEndOfFrame();
        DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} - Delayed showing tab: {tabIndex}", this);
        ShowTab(tabIndex);
    }

    /// <summary>
    /// Show tab by index
    /// </summary>
    /// <param name="tabIndex">Index of tab to show</param>
    public void ShowTab(int tabIndex)
    {
        // Validation checks
        if (tabIndex < 0 || tabIndex >= tabs.Length) return;
        if (!tabs[tabIndex].isAvailable) return;

        // Refresh detection
        bool isRefresh = (currentTabIndex == tabIndex);

        // Deactivate other tabs
        foreach (var tab in tabs)
        {
            if (tab.content != null)
                tab.content.SetActive(false);
        }

        // Activate selected tab 
        if (tabs[tabIndex].content != null)
        {
            tabs[tabIndex].content.SetActive(true);
        }

        currentTabIndex = tabIndex;
        UpdateButtonVisuals();

        // Fire events - only if it's not a refresh
        if (!isRefresh)
        {
            onTabClicked?.Invoke(tabIndex);
            onTabChanged?.Invoke(tabIndex);
            onTabChangedWithInfo?.Invoke(tabs[tabIndex]);
        }

        DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} - Tab activated: {tabIndex} {(isRefresh ? "[REFRESH]" : "")}", this);
    }

    /// <summary>
    /// Force refresh current tab
    /// </summary>
    public void RefreshCurrentTab()
    {
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
        {
            ShowTab(currentTabIndex);
        }
    }

    /// <summary>
    /// Show tab by its assigned value
    /// </summary>
    /// <param name="value">Tab value to find and show</param>
    public void ShowTabByValue(int value)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabValue == value)
            {
                ShowTab(i);
                return;
            }
        }
        DebugLogger.LogWarning(DebugLogCategory.UI, $"{TabManagerId} - No tab found with value: {value}", this);
    }

    /// <summary>
    /// Switch to next available tab (cyclically)
    /// </summary>
    public void ShowNextTab()
    {
        int nextIndex = (currentTabIndex + 1) % tabs.Length;
        while (!tabs[nextIndex].isAvailable && nextIndex != currentTabIndex)
        {
            nextIndex = (nextIndex + 1) % tabs.Length;
        }
        ShowTab(nextIndex);
    }

    /// <summary>
    /// Switch to previous available tab (cyclically)
    /// </summary>
    public void ShowPreviousTab()
    {
        int prevIndex = currentTabIndex - 1;
        if (prevIndex < 0) prevIndex = tabs.Length - 1;

        while (!tabs[prevIndex].isAvailable && prevIndex != currentTabIndex)
        {
            prevIndex--;
            if (prevIndex < 0) prevIndex = tabs.Length - 1;
        }
        ShowTab(prevIndex);
    }

    /// <summary>
    /// Set availability of a specific tab
    /// </summary>
    /// <param name="tabIndex">Index of tab to modify</param>
    /// <param name="isAvailable">New availability state</param>
    public void SetTabAvailability(int tabIndex, bool isAvailable)
    {
        if (tabIndex >= 0 && tabIndex < tabs.Length)
        {
            tabs[tabIndex].isAvailable = isAvailable;
            UpdateButtonVisuals();

            // If current tab becomes unavailable, switch to next available
            if (!isAvailable && currentTabIndex == tabIndex)
            {
                ShowNextTab();
            }
        }
    }

    /// <summary>
    /// Set availability by tab value
    /// </summary>
    /// <param name="value">Tab value to find</param>
    /// <param name="isAvailable">New availability state</param>
    public void SetTabAvailabilityByValue(int value, bool isAvailable)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabValue == value)
            {
                SetTabAvailability(i, isAvailable);
                return;
            }
        }
    }

    /// <summary>
    /// Update visual state of tab buttons using existing sprite states
    /// </summary>
    private void UpdateButtonVisuals()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].button == null) continue;

            var buttonImage = tabs[i].button.GetComponent<Image>();
            if (buttonImage == null || originalSprites == null || i >= originalSprites.Length) continue;

            // Always read from stored originals — never from current button state (may be stale)
            var storedState = originalSpriteStates[i];
            bool isActive = (i == currentTabIndex);
            bool isAvailable = tabs[i].isAvailable;

            if (!isAvailable)
            {
                // Unavailable: show disabled sprite, non-interactable
                SpriteState disabledState = storedState;
                if (unavailableSprite != null)
                    disabledState.disabledSprite = unavailableSprite;
                tabs[i].button.spriteState = disabledState;
                tabs[i].button.interactable = false;
            }
            else if (isActive)
            {
                // Active tab: set activeSprite as disabledSprite so Unity applies it on interactable=false
                Sprite activeSprite = GetSpriteFromState(storedState, activeTabSpriteType);
                SpriteState activeState = storedState;
                activeState.disabledSprite = activeSprite != null ? activeSprite : originalSprites[i];
                tabs[i].button.spriteState = activeState;
                tabs[i].button.interactable = false;
            }
            else
            {
                // Inactive available tab: restore original state, interactable — hover works normally
                SpriteState inactiveState = allowHoverOnInactive ? storedState : new SpriteState();
                tabs[i].button.spriteState = inactiveState;
                tabs[i].button.interactable = true;
                // Restore source image sprite in case it was overridden
                buttonImage.sprite = originalSprites[i];
            }
        }
    }

    /// <summary>
    /// Get sprite from button's sprite state based on type
    /// </summary>
    private Sprite GetSpriteFromState(SpriteState spriteState, SpriteStateType stateType)
    {
        switch (stateType)
        {
            case SpriteStateType.Selected:
                return spriteState.selectedSprite;
            case SpriteStateType.Pressed:
                return spriteState.pressedSprite;
            case SpriteStateType.Highlighted:
                return spriteState.highlightedSprite;
            case SpriteStateType.Disabled:
                return spriteState.disabledSprite;
            default:
                return spriteState.selectedSprite;
        }
    }

    // === GETTER METHODS ===

    /// <summary>
    /// Get current tab index
    /// </summary>
    /// <returns>Current tab index or -1 if none</returns>
    public int GetCurrentTabIndex() => currentTabIndex;

    /// <summary>
    /// Get current tab value
    /// </summary>
    /// <returns>Current tab value or -1 if none</returns>
    public int GetCurrentTabValue()
    {
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
            return tabs[currentTabIndex].tabValue;
        return -1;
    }

    /// <summary>
    /// Get current tab information
    /// </summary>
    /// <returns>Current TabInfo or null</returns>
    public TabInfo GetCurrentTabInfo()
    {
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
            return tabs[currentTabIndex];
        return null;
    }

    /// <summary>
    /// Check if specific tab is currently active
    /// </summary>
    /// <param name="tabIndex">Tab index to check</param>
    /// <returns>True if active</returns>
    public bool IsTabActive(int tabIndex) => currentTabIndex == tabIndex;

    /// <summary>
    /// Check if specific tab is available
    /// </summary>
    /// <param name="tabIndex">Tab index to check</param>
    /// <returns>True if available</returns>
    public bool IsTabAvailable(int tabIndex)
    {
        if (tabIndex >= 0 && tabIndex < tabs.Length)
            return tabs[tabIndex].isAvailable;
        return false;
    }

    /// <summary>
    /// Get total number of tabs
    /// </summary>
    /// <returns>Number of tabs</returns>
    public int GetTabCount() => tabs.Length;

    /// <summary>
    /// Hide all tabs
    /// </summary>
    public void HideAllTabs()
    {
        foreach (var tab in tabs)
        {
            if (tab.content != null)
                tab.content.SetActive(false);
        }
        currentTabIndex = -1;
        UpdateButtonVisuals();
    }

#if UNITY_EDITOR
    [ContextMenu("Debug - Tab Information")]
    private void DebugTabInfo()
    {
        DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} === TAB INFORMATION ===", this);
        DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} - Current Tab: {currentTabIndex}", this);

        for (int i = 0; i < tabs.Length; i++)
        {
            var tab = tabs[i];
            string status = i == currentTabIndex ? "[ACTIVE]" :
                           tab.isAvailable ? "[AVAILABLE]" : "[LOCKED]";
            DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} - Index {i}: Value {tab.tabValue} | {status}", this);
        }
    }

    [ContextMenu("Auto-find tabs")]
    private void AutoFindTabs()
    {
        // Helper method for editor - finds all direct children with "Tab" in name
        var children = new System.Collections.Generic.List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child.name.ToLower().Contains("tab"))
            {
                children.Add(child);
            }
        }

        DebugLogger.Log(DebugLogCategory.UI, $"{TabManagerId} - Found {children.Count} potential tabs", this);
    }
#endif
}