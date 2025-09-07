using UnityEngine;

/// <summary>
/// Level start event payload structure
/// </summary>
[System.Serializable]
public struct LevelSelectPayload
{
    public int levelNumber;
    public bool resetRequested;
}
