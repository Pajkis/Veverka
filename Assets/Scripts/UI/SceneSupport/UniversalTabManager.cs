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

        [Tooltip("Tab name for debugging purposes")]
        public string tabName;

        [Tooltip("Integer value for this tab (optional, used for external mapping)")]
        public int tabValue;

        [Tooltip("Is this tab currently available/unlocked?")]
        public bool isAvailable = true;

        [Tooltip("Custom data - can be any ScriptableObject or component")]
        public UnityEngine.Object customData;
    }

    [Header("Tab Configuration")]
    [SerializeField] private TabInfo[] tabs;

    [Header("Basic Settings")]
    [SerializeField] private int defaultTabIndex = 0;
    [SerializeField] private bool deactivateOtherTabs = true;

    [Header("Visual Feedback - Sprite Swap")]
    [Tooltip("Which sprite state to use for active tab (Selected, Pressed, Highlighted, or Disabled)")]
    [SerializeField] private SpriteStateType activeTabSpriteType = SpriteStateType.Selected;
    [SerializeField] private Sprite unavailableSprite; // Optional override for unavailable tabs

    [Header("Interactivity Settings")]
    [Tooltip("Allow hover effects on inactive tabs")]
    [SerializeField] private bool allowHoverOnInactive = false;

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

    // Store original sprites for each button
    private Sprite[] originalSprites;

    void Start()
    {
        InitializeTabs();
        ShowTab(defaultTabIndex);
    }

    /// <summary>
    /// Initialize tabs and connect button events
    /// </summary>
    private void InitializeTabs()
    {
        // Store original sprites
        originalSprites = new Sprite[tabs.Length];

        for (int i = 0; i < tabs.Length; i++)
        {
            int tabIndex = i; // Closure for lambda

            if (tabs[i].button != null)
            {
                tabs[i].button.onClick.AddListener(() => ShowTab(tabIndex));

                // Store original sprite (Source Image)
                var buttonImage = tabs[i].button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    originalSprites[i] = buttonImage.sprite;
                }
            }

            if (tabs[i].content != null && deactivateOtherTabs)
            {
                tabs[i].content.SetActive(false);
            }
        }
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
        if (currentTabIndex == tabIndex) return;

        // Deactivate other tabs
        if (deactivateOtherTabs)
        {
            foreach (var tab in tabs)
            {
                if (tab.content != null)
                    tab.content.SetActive(false);
            }
        }

        // Activate selected tab
        if (tabs[tabIndex].content != null)
        {
            tabs[tabIndex].content.SetActive(true);
        }

        currentTabIndex = tabIndex;
        UpdateButtonVisuals();

        // Fire events
        onTabClicked?.Invoke(tabIndex);
        onTabChanged?.Invoke(tabIndex);
        onTabChangedWithInfo?.Invoke(tabs[tabIndex]);

        Debug.Log($"Tab activated: {tabs[tabIndex].tabName} (index: {tabIndex})");
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
        Debug.LogWarning($"No tab found with value: {value}");
    }

    /// <summary>
    /// Show tab by its name (case insensitive)
    /// </summary>
    /// <param name="name">Tab name to find and show</param>
    public void ShowTabByName(string name)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                ShowTab(i);
                return;
            }
        }
        Debug.LogWarning($"No tab found with name: '{name}'");
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
            if (tabs[i].button != null)
            {
                var buttonImage = tabs[i].button.GetComponent<Image>();
                if (buttonImage != null && originalSprites != null && i < originalSprites.Length)
                {
                    var spriteState = tabs[i].button.spriteState;

                    // Debug log
                    Debug.Log($"Updating button {i}: {tabs[i].tabName}, Current tab: {currentTabIndex}, Available: {tabs[i].isAvailable}");

                    // Set the main sprite based on state
                    if (!tabs[i].isAvailable)
                    {
                        // Use unavailableSprite override or disabled sprite from button
                        if (unavailableSprite != null)
                        {
                            buttonImage.sprite = unavailableSprite;
                            Debug.Log($"Button {i} set to unavailable sprite (override)");
                        }
                        else if (spriteState.disabledSprite != null)
                        {
                            buttonImage.sprite = spriteState.disabledSprite;
                            Debug.Log($"Button {i} set to disabled sprite from button");
                        }
                        else
                        {
                            buttonImage.sprite = originalSprites[i];
                            Debug.Log($"Button {i} set to original sprite (unavailable fallback)");
                        }
                    }
                    else if (i == currentTabIndex)
                    {
                        // Active tab uses selected sprite state
                        Sprite activeSprite = GetSpriteFromState(spriteState, activeTabSpriteType);
                        if (activeSprite != null)
                        {
                            buttonImage.sprite = activeSprite;
                            Debug.Log($"Button {i} set to active sprite ({activeTabSpriteType})");
                        }
                        else
                        {
                            // Fallback to original if selected sprite not set
                            buttonImage.sprite = originalSprites[i];
                            Debug.Log($"Button {i} set to original sprite (active fallback - no {activeTabSpriteType} sprite set)");
                        }
                    }
                    else
                    {
                        // Inactive tab uses original Source Image sprite
                        buttonImage.sprite = originalSprites[i];
                        Debug.Log($"Button {i} set to original sprite (inactive)");
                    }

                    // Handle sprite state for interactions
                    if (i == currentTabIndex)
                    {
                        // Active tab keeps its sprite swap interactions
                        tabs[i].button.spriteState = spriteState;
                    }
                    else if (!allowHoverOnInactive)
                    {
                        // Inactive tabs have no sprite swap interactions
                        var emptyState = new SpriteState();
                        tabs[i].button.spriteState = emptyState;
                    }
                }

                // Set interactability
                tabs[i].button.interactable = tabs[i].isAvailable && (i != currentTabIndex);
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
    /// Get current tab name
    /// </summary>
    /// <returns>Current tab name or empty string</returns>
    public string GetCurrentTabName()
    {
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
            return tabs[currentTabIndex].tabName;
        return "";
    }

    /// <summary>
    /// Get current tab custom data of specified type
    /// </summary>
    /// <typeparam name="T">Type of custom data</typeparam>
    /// <returns>Custom data or null</returns>
    public T GetCurrentTabData<T>() where T : UnityEngine.Object
    {
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
            return tabs[currentTabIndex].customData as T;
        return null;
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
        Debug.Log("=== TAB INFORMATION ===");
        Debug.Log($"Current Tab: {currentTabIndex}");

        for (int i = 0; i < tabs.Length; i++)
        {
            var tab = tabs[i];
            string status = i == currentTabIndex ? "[ACTIVE]" :
                           tab.isAvailable ? "[AVAILABLE]" : "[LOCKED]";
            Debug.Log($"Index {i}: {tab.tabName} | Value: {tab.tabValue} | {status}");
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

        // This would need to be done in editor script to actually modify the serialized array
        Debug.Log($"Found {children.Count} potential tabs");
    }
#endif
}