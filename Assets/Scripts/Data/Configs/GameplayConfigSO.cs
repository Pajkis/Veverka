
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/Gameplay Config")]
public class GameplayConfigSO : ScriptableObject
{
    [Header("Rules")]
    public int maxUndoSteps = 10;
    //Move time of objects -> move speed = tilesize / moveTime;
    public float moveTime = 0.3f;

    [Header("Scoring")]
    public int reduceGoalCount = 1;
   
}