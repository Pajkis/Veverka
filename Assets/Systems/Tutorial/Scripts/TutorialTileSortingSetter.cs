using UnityEngine;

/// <summary>
/// Dynamicky nastavuje sorting layers pro tutorial tiles.
/// Movable tiles (Character, Nuts) dostanou Overlay3Movable (320).
/// Static tiles (Goals, Obstacles, Roads) dostanou Overlay3Static (310).
/// </summary>
public class TutorialTileSortingSetter : MonoBehaviour
{
    /// <summary>
    /// Nastaví správné sorting layers pro všechny tutorial tiles v gridRoot.
    /// </summary>
    public void SetTutorialSortingLayers(Transform tutorialRoot)
    {
        if (tutorialRoot == null)
        {
            DebugLogger.LogError(DebugLogCategory.Tutorial,
                "TutorialRoot is null, cannot set sorting layers", this);
            return;
        }

        int staticCount = 0;
        int movableCount = 0;

        foreach (SpriteRenderer renderer in tutorialRoot.GetComponentsInChildren<SpriteRenderer>())
        {
            // Movable tiles: Character, Nuts
            if (renderer.GetComponent<CharVeverka>() != null ||
                renderer.GetComponent<NutTile>() != null)
            {
                renderer.sortingLayerName = "Overlay3Movable";
                movableCount++;
            }
            // Static tiles: Goals, Obstacles, Roads, atd.
            else
            {
                renderer.sortingLayerName = "Overlay3Static";
                staticCount++;
            }
        }

        DebugLogger.Log(DebugLogCategory.Tutorial,
            $"Tutorial sorting layers set: {staticCount} static, {movableCount} movable", this);
    }
}
