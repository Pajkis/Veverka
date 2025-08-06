using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Debug tools 
/// </summary>
public static class DebugTools
{
#if UNITY_EDITOR
    /// <summary>
    /// Reset player prefs registers
    /// </summary>
    [MenuItem("Tools/Reset PlayerPrefs")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("✅ PlayerPrefs cleared.");
    }
#endif
}