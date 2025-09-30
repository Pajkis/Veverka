using UnityEngine;

/// <summary>
/// Static utility class for centralized debug logging
/// </summary>
public static class DebugLogger
{
    private static DebugLogConfig config;
    
    /// <summary>
    /// Initialize with debug config (call this once at startup)
    /// </summary>
    public static void Initialize(DebugLogConfig debugConfig)
    {
        config = debugConfig;
    }
    
    /// <summary>
    /// Log message if category is enabled
    /// </summary>
    public static void Log(DebugLogCategory category, string message, Object context = null)
    {
        if (config == null || !config.IsLogEnabled(category)) return;
        
        string categoryName = category.ToString();
        string colorCode = config.GetCategoryColor(category);
        
        if (!string.IsNullOrEmpty(colorCode))
        {
            Debug.Log($"<color={colorCode}>[{categoryName}]</color> {message}", context);
        }
        else
        {
            Debug.Log($"[{categoryName}] {message}", context);
        }
    }
    
    /// <summary>
    /// Log warning if category is enabled
    /// </summary>
    public static void LogWarning(DebugLogCategory category, string message, Object context = null)
    {
        if (config == null || !config.IsLogEnabled(category)) return;
        
        string categoryName = category.ToString();
        string colorCode = config.GetCategoryColor(category);
        
        if (!string.IsNullOrEmpty(colorCode))
        {
            Debug.LogWarning($"<color={colorCode}>[{categoryName} WARNING]</color> {message}", context);
        }
        else
        {
            Debug.LogWarning($"[{categoryName} WARNING] {message}", context);
        }
    }
    
    /// <summary>
    /// Log error if category is enabled
    /// </summary>
    public static void LogError(DebugLogCategory category, string message, Object context = null)
    {
        if (config == null || !config.IsLogEnabled(category)) return;
        
        string categoryName = category.ToString();
        string colorCode = config.GetCategoryColor(category);
        
        if (!string.IsNullOrEmpty(colorCode))
        {
            Debug.LogError($"<color={colorCode}>[{categoryName} ERROR]</color> {message}", context);
        }
        else
        {
            Debug.LogError($"[{categoryName} ERROR] {message}", context);
        }
    }
    
    /// <summary>
    /// Check if logging is enabled for category
    /// </summary>
    public static bool IsEnabled(DebugLogCategory category)
    {
        return config != null && config.IsLogEnabled(category);
    }
}