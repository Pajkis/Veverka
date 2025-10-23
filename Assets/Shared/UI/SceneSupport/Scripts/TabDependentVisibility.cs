using UnityEngine;

/// <summary>
/// Controls GameObject visibility based on active tab
/// Can hide or show object when specific tab is active
/// </summary>
public class TabDependentVisibility : MonoBehaviour
{
    [SerializeField] private UniversalTabManager tabManager;
    [SerializeField] private int targetTabValue = 667;
    [SerializeField] private TabVisibilityMode visibilityMode = TabVisibilityMode.HideOnTab;

    void Start()
    {
        if (tabManager != null)
        {
            tabManager.OnTabChanged.AddListener(OnTabChanged);
            OnTabChanged(tabManager.GetCurrentTabIndex()); // Set initial state
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.UI, "TabDependentVisibility: TabManager not assigned!", this);
        }
    }

    void OnDestroy()
    {
        if (tabManager != null)
        {
            tabManager.OnTabChanged.RemoveListener(OnTabChanged);
        }
    }

    void OnTabChanged(int tabIndex)
    {
        int currentTabValue = tabManager.GetCurrentTabValue();
        bool isTargetTab = (currentTabValue == targetTabValue);

        // Determine visibility based on mode
        bool shouldBeActive;

        if (visibilityMode == TabVisibilityMode.HideOnTab)
        {
            // HideOnTab: visible when NOT on target tab, hidden when ON target tab
            shouldBeActive = !isTargetTab;
        }
        else // ShowOnTab
        {
            // ShowOnTab: visible ONLY when ON target tab, hidden otherwise
            shouldBeActive = isTargetTab;
        }

        gameObject.SetActive(shouldBeActive);

        DebugLogger.Log(DebugLogCategory.UI,
            $"TabDependentVisibility [{gameObject.name}]: Tab {currentTabValue}, Target {targetTabValue}, Mode: {visibilityMode} - {(shouldBeActive ? "Visible" : "Hidden")}",
            this);
    }
}


