/// <summary>
/// Defines how GameObject visibility behaves based on tab state
/// </summary>
public enum TabVisibilityMode
{
    /// <summary>
    /// GameObject is hidden when target tab is active, visible otherwise
    /// </summary>
    HideOnTab = 0,

    /// <summary>
    /// GameObject is visible only when target tab is active, hidden otherwise
    /// </summary>
    ShowOnTab = 1
}