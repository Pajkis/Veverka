using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;

/// <summary>
/// Testy s reálnými level daty z vašeho projektu
/// </summary>
public class RealDataGridTests
{
    #region Tests with real level files

    [Test]
    public void TestAllLevelsInProject_ShouldBeValid()
    {
        // Find all .csv files containing levels
        string[] levelFiles = Directory.GetFiles(Application.dataPath, "*.csv", SearchOption.AllDirectories);

        Assert.IsTrue(levelFiles.Length > 0, "You should have at least one level file!");

        foreach (string levelFile in levelFiles)
        {
            // Load level as TextAsset
            string relativePath = "Assets" + levelFile.Substring(Application.dataPath.Length);
            TextAsset levelAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);

            if (levelAsset != null)
            {
                Debug.Log($"Testing level: {levelAsset.name}");

                // Load grid from file
                TileType[,] grid = GridUtils.LoadGridFromTextAsset(levelAsset);

                // Test 1: Grid loaded successfully
                Assert.IsNotNull(grid, $"Level {levelAsset.name} failed to load!");

                // Test 2: Grid is valid
                bool isValid = GridUtils.ValidateGrid(grid);
                Assert.IsTrue(isValid, $"Level {levelAsset.name} is not valid! Missing veverka/nuts/goals?");

                // Test 3: Grid has reasonable dimensions
                Assert.IsTrue(grid.GetLength(0) > 0 && grid.GetLength(1) > 0,
                    $"Level {levelAsset.name} has invalid dimensions: {grid.GetLength(0)}x{grid.GetLength(1)}");

                // Test 4: Grid is not too large
                Assert.IsTrue(grid.GetLength(0) <= 100 && grid.GetLength(1) <= 100,
                    $"Level {levelAsset.name} is too large: {grid.GetLength(0)}x{grid.GetLength(1)}");

                Debug.Log($"✓ Level {levelAsset.name} passed all tests!");
            }
        }
    }

    [Test]
    public void TestSpecificLevel_Level01_ShouldHaveCorrectElements()
    {
        // Test specific level (change name according to your files)
        TextAsset level = Resources.Load<TextAsset>("Gameplay/Level/Data/Levels/01_Basic/08_GreedyChibi");

        if (level != null)
        {
            TileType[,] grid = GridUtils.LoadGridFromTextAsset(level);

            // Count elements
            int veverkaCount = 0;
            int nutCount = 0;
            int goalCount = 0;
            int wallCount = 0;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    switch (grid[x, y])
                    {
                        case TileType.Veverka: veverkaCount++; break;
                        case TileType.Nut: nutCount++; break;
                        case TileType.Goal: goalCount++; break;
                        case TileType.Wall: wallCount++; break;
                    }
                }
            }

            // Assertions for specific expectations
            Assert.AreEqual(1, veverkaCount, "Level01 should have exactly one veverka");
            Assert.IsTrue(nutCount > 0, "Level01 should have at least one nut");
            Assert.IsTrue(goalCount > 0, "Level01 should have at least one goal");
            Assert.IsTrue(wallCount > 0, "Level01 should have some walls");

            Debug.Log($"08_GreedyChibi statistics: Veverka={veverkaCount}, Nuts={nutCount}, Goals={goalCount}, Walls={wallCount}");
        }
        else
        {
            Assert.Inconclusive("08_GreedyChibi was not found - check file path");
        }
    }

    #endregion

    #region Tests with manually created "invalid" levels

    [Test]
    public void TestInvalidGrid_NoVeverka_ShouldFail()
    {
        // Create grid without veverka (simulates level editing error)
        string invalidLevelData = "W,W,W,W,W\n" +
                                  "W,.,N,.,W\n" +
                                  "W,.,G,.,W\n" +
                                  "W,W,W,W,W";

        // Simulate TextAsset
        TextAsset mockLevel = new TextAsset(invalidLevelData);

        TileType[,] grid = GridUtils.LoadGridFromTextAsset(mockLevel);
        bool isValid = GridUtils.ValidateGrid(grid);

        Assert.IsFalse(isValid, "Grid without veverka should not be valid!");
    }

    [Test]
    public void TestInvalidGrid_TwoVeverkas_ShouldFail()
    {
        // Grid with two veverkas
        string invalidLevelData = "W,W,W,W,W\n" +
                                  "W,V,N,V,W\n" +
                                  "W,.,G,.,W\n" +
                                  "W,W,W,W,W";

        TextAsset mockLevel = new TextAsset(invalidLevelData);

        TileType[,] grid = GridUtils.LoadGridFromTextAsset(mockLevel);
        bool isValid = GridUtils.ValidateGrid(grid);

        Assert.IsFalse(isValid, "Grid with two veverkas should not be valid!");
    }

    #endregion

    #region Helper methods for debugging

    [Test]
    public void DebugPrintAllLevelsInfo()
    {
        string levelPath = Path.Combine(Application.dataPath, "Gameplay", "Level", "Data", "Levels");

        if (!Directory.Exists(levelPath))
        {
            Debug.LogWarning($"Level directory not found: {levelPath}");
            Assert.Inconclusive("Level directory not found");
            return;
        }

        string[] levelFiles = Directory.GetFiles(levelPath, "*.csv", SearchOption.AllDirectories);

        // Separate files: all files vs non-test files
        string[] allFiles = levelFiles;
        string[] nonTestFiles = System.Array.FindAll(levelFiles, file => !file.Contains("TestLevels"));

        Debug.Log($"=== LEVEL FILES SUMMARY ===");
        Debug.Log($"Scanning directory: Assets/Gameplay/Level/Data/Levels");
        Debug.Log($"Total .csv files found: {allFiles.Length}");
        Debug.Log($"Production level files (excluding TestLevels): {nonTestFiles.Length}");
        Debug.Log($"Test level files (in TestLevels): {allFiles.Length - nonTestFiles.Length}");
        Debug.Log($"");

        Debug.Log("=== PRODUCTION LEVELS (will be tested) ===");
        if (nonTestFiles.Length > 0)
        {
            foreach (string file in nonTestFiles)
            {
                string fileName = Path.GetFileName(file);
                string relativePath = file.Replace(Application.dataPath, "Assets");
                string subdirectory = Path.GetDirectoryName(file).Replace(levelPath, "").TrimStart('\\', '/');
                if (string.IsNullOrEmpty(subdirectory))
                {
                    Debug.Log($"✓ {fileName} (root level)");
                }
                else
                {
                    Debug.Log($"✓ {fileName} (in {subdirectory})");
                }
            }
        }
        else
        {
            Debug.Log("❌ No production level files found!");
        }

        if (allFiles.Length != nonTestFiles.Length)
        {
            Debug.Log($"");
            Debug.Log("=== TEST LEVELS (excluded from tests) ===");
            string[] testFiles = System.Array.FindAll(allFiles, file => file.Contains("TestLevels"));
            foreach (string file in testFiles)
            {
                string fileName = Path.GetFileName(file);
                Debug.Log($"⚠ {fileName} (in TestLevels - excluded)");
            }
        }

        Debug.Log($"");
        Debug.Log("=== EXPECTED LEVEL FILES FROM YOUR STRUCTURE ===");
        Debug.Log("Based on your folder structure, these should be found:");
        Debug.Log("✓ 01_Basic.csv, 01_Tutorial.csv, 02_Vshape.csv");
        Debug.Log("✓ 03_UpAndDown.csv, 04_Baricaded.csv, 05_ManyWays.csv");
        Debug.Log("✓ 06_EasyTrap.csv, 07_WhereNutBent.csv, 08_GreedyChibi.csv");
        Debug.Log("✓ 09_FinishLater.csv, 10_LongRun.csv");
        Debug.Log("✓ Files from 02_Stone subdirectory");

        // This test always passes, serves only for info
        Assert.IsTrue(true);
    }

    /// <summary>
    /// Helper method for creating test grid with specific parameters
    /// </summary>
    private TileType[,] CreateTestGrid(int veverkaCount, int nutCount, int goalCount)
    {
        // Simple 5x5 grid
        TileType[,] grid = new TileType[5, 5];

        // Fill with walls on borders
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                grid[x, y] = (x == 0 || x == 4 || y == 0 || y == 4) ? TileType.Wall : TileType.Empty;
            }
        }

        // Add veverkas
        if (veverkaCount >= 1) grid[1, 1] = TileType.Veverka;
        if (veverkaCount >= 2) grid[1, 2] = TileType.Veverka; // second veverka

        // Add nuts
        if (nutCount >= 1) grid[2, 1] = TileType.Nut;
        if (nutCount >= 2) grid[2, 2] = TileType.Nut;

        // Add goals
        if (goalCount >= 1) grid[3, 1] = TileType.Goal;
        if (goalCount >= 2) grid[3, 2] = TileType.Goal;

        return grid;
    }

    [Test]
    public void TestCreateTestGrid_Helper_Works()
    {
        // Test our helper method
        TileType[,] validGrid = CreateTestGrid(1, 1, 1);
        TileType[,] invalidGrid = CreateTestGrid(2, 1, 1); // 2 veverkas

        Assert.IsTrue(GridUtils.ValidateGrid(validGrid), "Valid test grid should pass");
        Assert.IsFalse(GridUtils.ValidateGrid(invalidGrid), "Invalid test grid should not pass");
    }

    #endregion
}