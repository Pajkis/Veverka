using UnityEngine;

/// <summary>
/// Hides GameObject when specific tab is active
/// Shows it again when different tab is selected
/// </summary>
public class HideOnTabActive : MonoBehaviour
{
    [SerializeField] private UniversalTabManager tabManager;
    [SerializeField] private int hideOnTabValue = 667;

    void Start()
    {
        if (tabManager != null)
        {
            tabManager.OnTabChanged.AddListener(OnTabChanged);
            OnTabChanged(tabManager.GetCurrentTabIndex()); // Set initial state
        }
        else
        {
            DebugLogger.LogError(DebugLogCategory.UI, "HideOnTabActive: TabManager not assigned!", this);
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
        bool shouldBeActive = currentTabValue != hideOnTabValue;

        gameObject.SetActive(shouldBeActive);

        DebugLogger.Log(DebugLogCategory.UI,
            $"HideOnTabActive [{gameObject.name}]: Tab {currentTabValue} - {(shouldBeActive ? "Visible" : "Hidden")}",
            this);
    }
}
