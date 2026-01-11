using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static helper class for spawning grid tiles.
/// Shared between GridBuilder (main game) and TutorialGridBuilder (tutorial system).
/// Extracts all spawn logic into one place to eliminate code duplication.
/// </summary>
public static class GridSpawner
{
    /// <summary>
    /// Configuration for spawning a grid. Contains all prefab references and context needed.
    /// </summary>
    public struct SpawnConfig
    {
        // Road prefabs
        public GameObject emptyPrefab;
        public GameObject roadPrefab;
        public GameObject stoneRoadPrefab;
        public GameObject stoneFilledHolePrefab;
        public GameObject roadGoldenGoalScoredPrefab;

        // Character prefab
        public GameObject veverkaPrefab;

        // Nut prefabs
        public GameObject nutPrefab;
        public GameObject nutStonePrefab;
        public GameObject nutWaterPrefab;
        public GameObject nutGoldenPrefab;

        // Obstacle prefabs
        public GameObject wallPrefab;
        public GameObject wallStonePrefab;
        public GameObject holePrefab;
        public GameObject waterHolePrefab;
        public GameObject goldenStatuePrefab;

        // Goal prefabs
        public GameObject goalPrefab;
        public GameObject goldenGoalPrefab;

        // Other prefabs
        public GameObject backgroundPrefab;

        // Context
        public Transform gridRoot;
        public LevelInitData levelInitData; // Nullable - null for tutorial
        public List<GameObject> backgroundPool;
        public List<GameObject> surroundingObstacles; // Nullable - null for tutorial
    }

    /// <summary>
    /// Ensures that the background pool contains at least the specified number of objects.
    /// </summary>
    private static void EnsureBackgroundPool(int requiredCount, SpawnConfig config)
    {
        int currentCount = config.backgroundPool.Count;
        if (requiredCount > currentCount)
        {
            DebugLogger.Log(DebugLogCategory.GridSystem, $"Expanding background pool from {currentCount} to {requiredCount} objects", null);
        }

        for (int i = config.backgroundPool.Count; i < requiredCount; i++)
        {
            var background = Object.Instantiate(config.backgroundPrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
            background.SetActive(false);
            config.backgroundPool.Add(background);
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Background pool ready with {config.backgroundPool.Count} objects", null);
    }

    /// <summary>
    /// Spawns a grid using the provided GridData and SpawnConfig.
    /// This is the core spawn logic shared between main game and tutorial.
    /// </summary>
    public static void SpawnGrid(GridData gridData, SpawnConfig config)
    {
        Vector2Int gridSize = gridData.GridSize;
        DebugLogger.Log(DebugLogCategory.GridSystem, $"Building level from grid - Processing {gridSize.x * gridSize.y} tiles", null);
        EnsureBackgroundPool(gridSize.x * gridSize.y, config);

        int bgIndex = 0;

        // For cycle over grid width
        for (int x = 0; x < gridSize.x; x++)
        {
            // For cycle over grid height
            for (int y = 0; y < gridSize.y; y++)
            {
                // Prepare tile from the grid
                Vector2Int tilePos = new(x, y);
                Vector3 worldTilePos = GridUtils.GridToWorld(tilePos);
                TileType tileType = gridData.GetTileType(tilePos);

                var background = config.backgroundPool[bgIndex++];
                background.transform.SetParent(config.gridRoot, false);
                background.transform.localPosition = worldTilePos;
                background.SetActive(true);

                // Insert specific tiles
                switch (tileType)
                {
                    case TileType.Obstacle:
                        ObstacleType obstacleType = gridData.GetObstacleType(tilePos);
                        if (obstacleType == ObstacleType.Hole)
                        {
                            HoleTile holeTile = Object.Instantiate(config.holePrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<HoleTile>();
                            holeTile.transform.localPosition = worldTilePos;
                            holeTile.Init(tileType, tilePos, ObstacleType.Hole);
                        }
                        else if (obstacleType == ObstacleType.WaterHole)
                        {
                            WaterHoleTile waterHoleTile = Object.Instantiate(config.waterHolePrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<WaterHoleTile>();
                            waterHoleTile.transform.localPosition = worldTilePos;
                            waterHoleTile.Init(tileType, tilePos, ObstacleType.WaterHole);
                        }
                        else if (obstacleType == ObstacleType.StoneWall)
                        {
                            GameObject tile = Object.Instantiate(config.wallStonePrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            tile.transform.localPosition = worldTilePos;
                        }
                        else if (obstacleType == ObstacleType.BasicWall)
                        {
                            GameObject tile = Object.Instantiate(config.wallPrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            tile.transform.localPosition = worldTilePos;
                        }
                        else if (obstacleType == ObstacleType.GoldenStatue)
                        {
                            GameObject tile = Object.Instantiate(config.goldenStatuePrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            tile.transform.localPosition = worldTilePos;
                        }
                        gridData.SetTileType(tilePos, TileType.Obstacle);
                        break;

                    case TileType.Veverka:
                        gridData.SetTileType(tilePos, TileType.Road);
                        gridData.SetRoadType(tilePos, RoadType.Empty);

                        // Generate veverka
                        CharVeverka veverka = Object.Instantiate(config.veverkaPrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<CharVeverka>();
                        veverka.transform.localPosition = worldTilePos;
                        veverka.Init(tileType, CharacterType.BasicVeverka, tilePos);

                        // Center grid according to veverka (only for main game, not tutorial)
                        if (config.levelInitData != null)
                        {
                            config.levelInitData.gridCenterStartTarget = veverka.transform;
                            DebugLogger.Log(DebugLogCategory.Gameplay, $"Player character (Veverka) placed at {tilePos} and set as camera target", null);
                        }
                        break;

                    case TileType.Nut:
                        NutType nutType = gridData.GetNutType(tilePos);
                        if (nutType == NutType.StoneNut)
                        {
                            StoneNutTile stoneNutTile = Object.Instantiate(config.nutStonePrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<StoneNutTile>();
                            stoneNutTile.transform.localPosition = worldTilePos;
                            stoneNutTile.Init(tileType, tilePos, NutType.StoneNut);
                        }
                        else if (nutType == NutType.BasicNut)
                        {
                            BasicNutTile nutTile = Object.Instantiate(config.nutPrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<BasicNutTile>();
                            nutTile.transform.localPosition = worldTilePos;
                            nutTile.Init(tileType, tilePos, NutType.BasicNut);
                        }
                        else if (nutType == NutType.WaterNut)
                        {
                            WaterNutTile waterNutTile = Object.Instantiate(config.nutWaterPrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<WaterNutTile>();
                            waterNutTile.transform.localPosition = worldTilePos;
                            waterNutTile.Init(tileType, tilePos, NutType.WaterNut);
                        }
                        else if (nutType == NutType.GoldenNut)
                        {
                            GoldenNutTile goldenNutTile = Object.Instantiate(config.nutGoldenPrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<GoldenNutTile>();
                            goldenNutTile.transform.localPosition = worldTilePos;
                            goldenNutTile.Init(tileType, tilePos, NutType.GoldenNut);
                        }
                        break;

                    case TileType.Goal:
                        GoalType goalType = gridData.GetGoalType(tilePos);
                        if (goalType == GoalType.BasicGoal)
                        {
                            GoalTile goalTile = Object.Instantiate(config.goalPrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<GoalTile>();
                            goalTile.transform.localPosition = worldTilePos;
                            goalTile.Init(tileType, tilePos, GoalType.BasicGoal);
                        }
                        else if (goalType == GoalType.GoldenGoal)
                        {
                            GoldenGoalTile goldenGoalTile = Object.Instantiate(config.goldenGoalPrefab, Vector3.zero, Quaternion.identity, config.gridRoot).GetComponent<GoldenGoalTile>();
                            goldenGoalTile.transform.localPosition = worldTilePos;
                            goldenGoalTile.Init(tileType, tilePos, GoalType.GoldenGoal);
                        }
                        break;

                    case TileType.Road:
                        RoadType roadType = gridData.GetRoadType(tilePos);
                        if (roadType == RoadType.Empty)
                        {
                            // Empty road - just background, no prefab needed
                            gridData.SetTileType(tilePos, TileType.Road);
                        }
                        else if (roadType == RoadType.StoneFilledHole)
                        {
                            GameObject roadTile = Object.Instantiate(config.stoneFilledHolePrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            roadTile.transform.localPosition = worldTilePos;
                            gridData.SetTileType(tilePos, TileType.Road);
                        }
                        else if (roadType == RoadType.StoneRoad)
                        {
                            GameObject roadTile = Object.Instantiate(config.stoneRoadPrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            roadTile.transform.localPosition = worldTilePos;
                            gridData.SetTileType(tilePos, TileType.Road);
                        }
                        else if (roadType == RoadType.BasicRoad)
                        {
                            GameObject roadTile = Object.Instantiate(config.roadPrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            roadTile.transform.localPosition = worldTilePos;
                            gridData.SetTileType(tilePos, TileType.Road);
                        }
                        else if (roadType == RoadType.RoadGoldenGoalScored)
                        {
                            GameObject roadTile = Object.Instantiate(config.roadGoldenGoalScoredPrefab, Vector3.zero, Quaternion.identity, config.gridRoot);
                            roadTile.transform.localPosition = worldTilePos;
                            gridData.SetTileType(tilePos, TileType.Road);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        for (int i = bgIndex; i < config.backgroundPool.Count; i++)
        {
            config.backgroundPool[i].SetActive(false);
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, $"Level build complete - {bgIndex} tiles processed, {config.backgroundPool.Count - bgIndex} background objects deactivated", null);
    }

    /// <summary>
    /// Clears all non-pooled objects from the grid.
    /// Used for both main game reset and tutorial cleanup.
    /// </summary>
    public static void ClearNonPooledObjects(Transform gridRoot, List<GameObject> backgroundPool, List<GameObject> surroundingObstacles)
    {
        // Destroy old grid except pooled backgrounds
        foreach (Transform child in gridRoot.transform)
        {
            // Pool of surrounding obstacles (main game only)
            if (surroundingObstacles != null && surroundingObstacles.Contains(child.gameObject))
            {
                child.gameObject.SetActive(false);
                continue;
            }

            // Pool of in-game backgrounds
            if (!backgroundPool.Contains(child.gameObject))
            {
                Object.Destroy(child.gameObject);
            }
        }

        foreach (var background in backgroundPool)
        {
            background.SetActive(false);
        }

        DebugLogger.Log(DebugLogCategory.GridSystem, "Grid cleared - non-pooled objects destroyed, pooled objects deactivated", null);
    }
}
