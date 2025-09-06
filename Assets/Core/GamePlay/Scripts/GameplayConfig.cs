
using UnityEngine;
/// <summary>
/// Gameplay configuration settings for the game.   
/// </summary>
[CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/GameplayConfig")]
public class GameplayConfig : ScriptableObject
{
    [Header("Rules")]
    public int maxUndoSteps = 10;
    //Move time of objects -> move speed = tilesize / moveTime;
    public float moveTime = 0.2f;

    [Header("Scoring")]
    public int reduceGoalCount = 1;
   
}