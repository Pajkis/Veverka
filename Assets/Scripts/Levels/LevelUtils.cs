using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Level utilities class
/// </summary>
public static class LevelUtils
{
    /// <summary>
    /// loaded/validated grid for building the level
    /// </summary>
    public static TileType[,] CachedGrid { get; private set; }
    public static NutType[,] CachedNutGrid { get; private set; }
    public static GoalType[,] CachedGoalGrid { get; private set; }
    public static WallType[,] CachedWallGrid { get; private set; }
    public static RoadType[,] CachedRoadGrid { get; private set; }
        
    /// <summary>
    /// Loads data from file into array
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static TileType[,] LoadGridFromCsv(string filename)
    {
        try
        {
            // get file into variable
            TextAsset levelData = Resources.Load<TextAsset>($"{filename}");

            if (levelData == null)
                throw new FileNotFoundException($"File not found: {filename}");

            // get size of the array
            string[] lines = levelData.text.Split('\n');
            int height = lines.Length;
            int width = lines[0].Split(',').Length;

            CachedNutGrid = null;
            CachedGoalGrid = null;
            CachedWallGrid = null;
            CachedRoadGrid = null;

            TileType[,] levelGrid = new TileType[width, height];
            NutType[,] nutGrid = new NutType[width, height];
            GoalType[,] goalGrid = new GoalType[width, height];
            WallType[,] wallGrid = new WallType[width, height];
            RoadType[,] roadGrid = new RoadType[width, height];

            Debug.Log("Size of layout is (width, height): " + width + ", " + height);
            // put data into grid
            for (int y = 0; y < height; y++)
            {
                string[] row = lines[height - 1 - y].Split(",");
                for (int x = 0; x < width; x++)
                {
                    ParsedTile parsed = TileParser.ParseTileType(row[x].Trim());
                    levelGrid[x, y] = parsed.TileType;
                    nutGrid[x, y] = parsed.NutType;
                    goalGrid[x, y] = parsed.GoalType;
                    wallGrid[x, y] = parsed.WallType;
                    roadGrid[x, y] = parsed.RoadType;

                    //Catch error types
                    if (parsed.TileType == TileType.ErrorTile)
                    {
                        Debug.LogWarning($"error tile on position[x,y]: {x}, {y}");
                    }

                }
            }

            CachedNutGrid = nutGrid;
            CachedGoalGrid = goalGrid;
            CachedWallGrid = wallGrid;
            CachedRoadGrid = roadGrid;
            return levelGrid;

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return null;
        }

    }

    /// <summary>
    /// Validates the grid loaded from file
    /// </summary>
    /// <param name="grid">loaded grid from file</param>
    /// <returns>grid is Valid <= true</returns>
    public static bool ValidateGrid(TileType[,] grid)
    {
        int veverkaCount = 0;
        int nutCount = 0;
        int goalCount = 0;
        int errorTileCount = 0;
        bool valid = true;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                switch (grid[x, y])
                {
                    case TileType.Veverka: veverkaCount++; break;
                    case TileType.Nut: nutCount++; break;
                    case TileType.Goal: goalCount++; break;
                    case TileType.ErrorTile: errorTileCount++; break;
                }
            }
        }

        // check veverka count
        if (veverkaCount != 1)
        {
            Debug.LogWarning($"Expected 1 veverka but found {veverkaCount}");
            valid = false;
        }

        // check nuts amount more than zero        
        if (nutCount < 1)
        {
            Debug.LogWarning($"Expected at least 1 nut but found {nutCount}");
            valid =  false;
        }

        // check goal amount more than zero        
        if (goalCount < 1)
        {
            Debug.LogWarning($"Expected at least 1 goal but found {goalCount}");
            valid =  false;
        }

        // check same amount of nut and goals
        if (nutCount != goalCount)
        {
            Debug.LogWarning($"The amount of goals and nuts does not match! Found goals: {goalCount}, found nuts {nutCount}");
           // valid =  false;
        }

        // Check for invalid tiles
        if (errorTileCount > 0)
        {
            Debug.LogWarning($"invalid tiles in input CSV file: Count {errorTileCount}");
            valid = false;
        }

        //Set up grid
        CachedGrid = null;
        CachedGrid = grid;
        return valid;
    }
}

