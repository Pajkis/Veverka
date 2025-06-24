using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// Level utilities class
/// </summary>
public static class LevelUtils
{
    /// <summary>
    /// loaded/validated grid for building the level
    /// </summary>
    public static TileType[,] CachedGrid { get; private set; }
        
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

            TileType[,] levelGrid = new TileType[width, height];

            Debug.Log("Size of layout is (height, width): " + height + ", " + width);
            // put data into grid
            for (int y = 0; y < height; y++)
            {
                string[] row = lines[y].Split(",");
                for (int x = 0; x < width; x++)
                {
                    levelGrid[x, height - 1 - y] = TileTypeExtensions.ParseTileType(row[x].Trim());
                }
            }

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

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                switch (grid[x, y])
                {
                    case TileType.Veverka: veverkaCount++; break;
                    case TileType.Nut: nutCount++; break;
                    case TileType.Goal: goalCount++; break;
                }
            }
        }

        // check veverka count
        if (veverkaCount != 1)
        {
            Debug.Log($"Expected 1 veverka but found {veverkaCount}");
            return false;
        }

        // check nuts amount more than zero        
        if (nutCount < 1)
        {
            Debug.Log($"Expected at least 1 nut but found {nutCount}");
            return false;
        }

        // check goal amount more than zero        
        if (goalCount < 1)
        {
            Debug.Log($"Expected at least 1 goal but found {goalCount}");
            return false;
        }

        // check same amount of nut and goals
        if (nutCount != goalCount)
        {
            Debug.Log($"The amount of goals and nuts does not match! Found goals: {goalCount}, found nuts {nutCount}");
            return false;
        }

        //Set up grid
        CachedGrid = null;
        CachedGrid = grid;
        return true;
    }
}

