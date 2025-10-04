using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;

/// <summary>
/// Kompletní testy pro GridUtils - unit testy i testy s reálnými daty
/// Každý reálný level je testován všemi individuálními testy místo ValidateGrid
/// Umístěné v Assets/Editor/Tests/
/// </summary>
public class GridUtilsTests
{
    #region Unit Tests - Basic GridUtils Functions

    [Test]
    public void GridToWorld_Origin_ReturnsZero()
    {
        // Arrange
        Vector2Int gridPosition = Vector2Int.zero;

        // Act
        Vector3 result = GridUtils.GridToWorld(gridPosition);

        // Assert
        Assert.AreEqual(Vector3.zero, result, "Grid origin should convert to world origin");
    }

    [Test]
    public void GetPositionInDir_Up_ReturnsCorrectPosition()
    {
        // Arrange
        Vector2Int startPosition = new Vector2Int(5, 5);
        Direction direction = Direction.Up;
        int distance = 1;

        // Act
        Vector2Int result = GridUtils.GetPositionInDir(startPosition, direction, distance);

        // Assert
        Vector2Int expected = new Vector2Int(5, 6);
        Assert.AreEqual(expected, result, "Moving up should increase Y coordinate by 1");
    }

    [Test]
    public void ParseTileType_Wall_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("W");

        // Assert
        Assert.AreEqual(TileType.Obstacle, result.TileType);
        Assert.AreEqual(ObstacleType.BasicWall, result.ObstacleType);
    }

    [Test]
    public void GetPositionInDir_Down_ReturnsCorrectPosition()
    {
        // Arrange
        Vector2Int startPosition = new Vector2Int(5, 5);
        Direction direction = Direction.Down;
        int distance = 1;

        // Act
        Vector2Int result = GridUtils.GetPositionInDir(startPosition, direction, distance);

        // Assert
        Vector2Int expected = new Vector2Int(5, 4);
        Assert.AreEqual(expected, result, "Moving down should decrease Y coordinate by 1");
    }

    [Test]
    public void GetPositionInDir_Left_ReturnsCorrectPosition()
    {
        // Arrange
        Vector2Int startPosition = new Vector2Int(5, 5);
        Direction direction = Direction.Left;
        int distance = 1;

        // Act
        Vector2Int result = GridUtils.GetPositionInDir(startPosition, direction, distance);

        // Assert
        Vector2Int expected = new Vector2Int(4, 5);
        Assert.AreEqual(expected, result, "Moving left should decrease X coordinate by 1");
    }

    [Test]
    public void GetPositionInDir_Right_ReturnsCorrectPosition()
    {
        // Arrange
        Vector2Int startPosition = new Vector2Int(5, 5);
        Direction direction = Direction.Right;
        int distance = 1;

        // Act
        Vector2Int result = GridUtils.GetPositionInDir(startPosition, direction, distance);

        // Assert
        Vector2Int expected = new Vector2Int(6, 5);
        Assert.AreEqual(expected, result, "Moving right should increase X coordinate by 1");
    }

    [Test]
    public void GetPositionInDir_MultipleDistance_ReturnsCorrectPosition()
    {
        // Arrange
        Vector2Int startPosition = new Vector2Int(5, 5);
        Direction direction = Direction.Up;
        int distance = 3;

        // Act
        Vector2Int result = GridUtils.GetPositionInDir(startPosition, direction, distance);

        // Assert
        Vector2Int expected = new Vector2Int(5, 8);
        Assert.AreEqual(expected, result, "Moving up with distance 3 should increase Y by 3");
    }

    [Test]
    public void GetPositionInDir_ZeroDistance_TreatsAsOne()
    {
        // Arrange
        Vector2Int startPosition = new Vector2Int(5, 5);
        Direction direction = Direction.Right;
        int distance = 0;

        // Act
        Vector2Int result = GridUtils.GetPositionInDir(startPosition, direction, distance);

        // Assert
        Vector2Int expected = new Vector2Int(6, 5);
        Assert.AreEqual(expected, result, "Zero distance should be treated as distance 1");
    }

    [Test]
    public void GridToWorld_PositiveCoordinates_ReturnsCorrectWorldPosition()
    {
        // Arrange
        Vector2Int gridPosition = new Vector2Int(3, 4);

        // Act
        Vector3 result = GridUtils.GridToWorld(gridPosition);

        // Assert
        Vector3 expected = new Vector3(3f, 4f, 0f);
        Assert.AreEqual(expected, result, "Positive grid coordinates should convert correctly");
    }

    [Test]
    public void GridToWorld_NegativeCoordinates_ReturnsCorrectWorldPosition()
    {
        // Arrange
        Vector2Int gridPosition = new Vector2Int(-2, -3);

        // Act
        Vector3 result = GridUtils.GridToWorld(gridPosition);

        // Assert
        Vector3 expected = new Vector3(-2f, -3f, 0f);
        Assert.AreEqual(expected, result, "Negative grid coordinates should convert correctly");
    }

    [Test]
    public void GridToWorld_CustomTileSize_ReturnsScaledPosition()
    {
        // Arrange
        Vector2Int gridPosition = new Vector2Int(2, 3);
        float tileSize = 2.5f;

        // Act
        Vector3 result = GridUtils.GridToWorld(gridPosition, tileSize);

        // Assert
        Vector3 expected = new Vector3(5f, 7.5f, 0f);
        Assert.AreEqual(expected, result, "Custom tile size should scale world position correctly");
    }

    [Test]
    public void ParseTileType_StoneWall_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("WS");

        // Assert
        Assert.AreEqual(TileType.Obstacle, result.TileType);
        Assert.AreEqual(ObstacleType.StoneWall, result.ObstacleType);
    }

    [Test]
    public void ParseTileType_Veverka_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("V");

        // Assert
        Assert.AreEqual(TileType.Veverka, result.TileType);
    }

    [Test]
    public void ParseTileType_BasicNut_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("N");

        // Assert
        Assert.AreEqual(TileType.Nut, result.TileType);
        Assert.AreEqual(NutType.BasicNut, result.NutType);
    }

    [Test]
    public void ParseTileType_StoneNut_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("NS");

        // Assert
        Assert.AreEqual(TileType.Nut, result.TileType);
        Assert.AreEqual(NutType.StoneNut, result.NutType);
    }

    [Test]
    public void ParseTileType_BasicGoal_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("G");

        // Assert
        Assert.AreEqual(TileType.Goal, result.TileType);
        Assert.AreEqual(GoalType.BasicGoal, result.GoalType);
    }

    [Test]
    public void ParseTileType_HoleObstacle_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("H");

        // Assert
        Assert.AreEqual(TileType.Obstacle, result.TileType);
        Assert.AreEqual(ObstacleType.Hole, result.ObstacleType);
    }

    [Test]
    public void ParseTileType_Empty_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType(".");

        // Assert
        Assert.AreEqual(TileType.Road, result.TileType);
        Assert.AreEqual(RoadType.Empty, result.RoadType);
    }

    [Test]
    public void ParseTileType_Road_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("R");

        // Assert
        Assert.AreEqual(TileType.Road, result.TileType);
        Assert.AreEqual(RoadType.BasicRoad, result.RoadType);
    }

    [Test]
    public void ParseTileType_InvalidSymbol_ReturnsErrorTile()
    {
        // Act
        var result = GridUtils.ParseTileType("X");

        // Assert
        Assert.AreEqual(TileType.ErrorTile, result.TileType, "Invalid symbol should return ErrorTile");
    }

    [Test]
    public void ParseTileType_EmptyString_ReturnsErrorTile()
    {
        // Act
        var result = GridUtils.ParseTileType("");

        // Assert
        Assert.AreEqual(TileType.ErrorTile, result.TileType, "Empty string should return ErrorTile");
    }

    [Test]
    public void ParseTileType_NullString_ReturnsErrorTile()
    {
        // Act
        var result = GridUtils.ParseTileType(null);

        // Assert
        Assert.AreEqual(TileType.ErrorTile, result.TileType, "Null string should return ErrorTile");
    }

    #endregion

    #region Unit Tests - Validation with Mock Data

    [Test]
    public void ValidateGrid_ValidBasicGrid_ReturnsTrue()
    {
        // Arrange - simple 3x3 grid with 1 veverka, 1 nut, 1 goal
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Obstacle, TileType.Obstacle, TileType.Obstacle },
            { TileType.Obstacle, TileType.Veverka, TileType.Nut },
            { TileType.Obstacle, TileType.Goal, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsTrue(result, "Valid basic grid should pass validation");
        Assert.AreEqual(grid, GridUtils.CachedGrid, "CachedGrid should be set after validation");
    }

    [Test]
    public void ValidateGrid_NoVeverka_ReturnsFalse()
    {
        // Arrange - grid without veverka
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Obstacle, TileType.Obstacle, TileType.Obstacle },
            { TileType.Obstacle, TileType.Road, TileType.Nut },
            { TileType.Obstacle, TileType.Goal, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsFalse(result, "Grid without veverka should fail validation");
    }

    [Test]
    public void ValidateGrid_MultipleVeverkas_ReturnsFalse()
    {
        // Arrange - grid with 2 veverkas
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Obstacle, TileType.Veverka, TileType.Obstacle },
            { TileType.Obstacle, TileType.Veverka, TileType.Nut },
            { TileType.Obstacle, TileType.Goal, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsFalse(result, "Grid with multiple veverkas should fail validation");
    }

    [Test]
    public void ValidateGrid_NoNuts_ReturnsFalse()
    {
        // Arrange - grid without nuts
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Obstacle, TileType.Obstacle, TileType.Obstacle },
            { TileType.Obstacle, TileType.Veverka, TileType.Road },
            { TileType.Obstacle, TileType.Goal, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsFalse(result, "Grid without nuts should fail validation");
    }

    [Test]
    public void ValidateGrid_NoGoals_ReturnsFalse()
    {
        // Arrange - grid without goals
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Obstacle, TileType.Obstacle, TileType.Obstacle },
            { TileType.Obstacle, TileType.Veverka, TileType.Nut },
            { TileType.Obstacle, TileType.Road, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsFalse(result, "Grid without goals should fail validation");
    }

    [Test]
    public void ValidateGrid_ErrorTiles_ReturnsFalse()
    {
        // Arrange - grid with error tile
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Obstacle, TileType.Obstacle, TileType.Obstacle },
            { TileType.Obstacle, TileType.Veverka, TileType.Nut },
            { TileType.ErrorTile, TileType.Goal, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsFalse(result, "Grid with error tiles should fail validation");
    }

    [Test]
    public void ValidateGrid_MultipleNutsAndGoals_ReturnsTrue()
    {
        // Arrange - valid grid with 2 nuts and 2 goals
        TileType[,] grid = new TileType[4, 4] {
            { TileType.Obstacle, TileType.Obstacle, TileType.Obstacle, TileType.Obstacle },
            { TileType.Obstacle, TileType.Veverka, TileType.Nut, TileType.Obstacle },
            { TileType.Obstacle, TileType.Nut, TileType.Goal, TileType.Obstacle },
            { TileType.Obstacle, TileType.Goal, TileType.Obstacle, TileType.Obstacle }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsTrue(result, "Valid grid with multiple nuts and goals should pass validation");
    }

    #endregion

    #region Real Data Tests - Testing Actual Level Files with ALL Individual GridUtils Functions

    [Test]
    public void TestAllRealLevels_WithEveryIndividualGridUtilsTest()
    {
        // Find all production .csv files (exclude TestLevels)
        string[] levelFiles = Directory.GetFiles(Application.dataPath, "*.csv", SearchOption.AllDirectories);
        string[] productionLevels = System.Array.FindAll(levelFiles, file => !file.Contains("TestLevels"));

        Assert.IsTrue(productionLevels.Length > 0, "No production level files found! Check if levels are in correct directory.");

        Debug.Log($"=== Testing {productionLevels.Length} production levels with EVERY individual GridUtils test ===");

        foreach (string levelFile in productionLevels)
        {
            // Load level as TextAsset
            string relativePath = "Assets" + levelFile.Substring(Application.dataPath.Length);
            TextAsset levelAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);

            if (levelAsset != null)
            {
                Debug.Log($"Testing level: {levelAsset.name}");

                // Load grid from file
                TileType[,] grid = GridUtils.LoadGridFromTextAsset(levelAsset);
                Assert.IsNotNull(grid, $"GridUtils.LoadGridFromTextAsset failed for level {levelAsset.name}");

                // Now run EVERY individual test that was originally in GridUtilsTests on this real level
                RunEveryIndividualTestOnRealLevel(grid, levelAsset.name);

                Debug.Log($"✓ Level {levelAsset.name} passed all individual GridUtils tests!");
            }
            else
            {
                Assert.Fail($"Could not load level file as TextAsset: {relativePath}");
            }
        }

        Debug.Log($"=== All {productionLevels.Length} production levels tested with every individual GridUtils test! ===");
    }

    /// <summary>
    /// Runs EVERY individual test from the original GridUtilsTests on real level data
    /// This will reveal issues that ValidateGrid bypasses (like stone levels having different nut/goal counts)
    /// </summary>
    private void RunEveryIndividualTestOnRealLevel(TileType[,] grid, string levelName)
    {
        // Find real coordinates from this level to use for testing
        Vector2Int veverkaPos = FindTilePosition(grid, TileType.Veverka);
        Vector2Int nutPos = FindTilePosition(grid, TileType.Nut);
        Vector2Int goalPos = FindTilePosition(grid, TileType.Goal);
        Vector2Int wallPos = FindTilePosition(grid, TileType.Obstacle);
        Vector2Int roadPos = FindTilePosition(grid, TileType.Road);

        // Run GridToWorld tests equivalent to original unit tests
        Test_GridToWorld_Origin_ReturnsZero_OnRealLevel(levelName);
        Test_GridToWorld_PositiveCoordinates_ReturnsCorrectWorldPosition_OnRealLevel(veverkaPos, levelName);
        Test_GridToWorld_NegativeCoordinates_ReturnsCorrectWorldPosition_OnRealLevel(levelName);
        Test_GridToWorld_CustomTileSize_ReturnsScaledPosition_OnRealLevel(nutPos, levelName);

        // Run GetPositionInDir tests equivalent to original unit tests
        Test_GetPositionInDir_Up_ReturnsCorrectPosition_OnRealLevel(veverkaPos, levelName);
        Test_GetPositionInDir_Down_ReturnsCorrectPosition_OnRealLevel(veverkaPos, levelName);
        Test_GetPositionInDir_Left_ReturnsCorrectPosition_OnRealLevel(veverkaPos, levelName);
        Test_GetPositionInDir_Right_ReturnsCorrectPosition_OnRealLevel(veverkaPos, levelName);
        Test_GetPositionInDir_MultipleDistance_ReturnsCorrectPosition_OnRealLevel(veverkaPos, levelName);
        Test_GetPositionInDir_ZeroDistance_TreatsAsOne_OnRealLevel(nutPos, levelName);

        // Run ParseTileType tests equivalent to original unit tests
        Test_ParseTileType_AllVariants_OnRealLevel(levelName);

        // Run ValidateGrid logic tests equivalent to original unit tests - THIS IS THE KEY PART
        Test_ValidateGrid_ValidBasicGrid_ReturnsTrue_OnRealLevel(grid, levelName);
        Test_ValidateGrid_ExactlyOneVeverka_OnRealLevel(grid, levelName);
        Test_ValidateGrid_HasNuts_OnRealLevel(grid, levelName);
        Test_ValidateGrid_HasGoals_OnRealLevel(grid, levelName);
        Test_ValidateGrid_NoErrorTiles_OnRealLevel(grid, levelName);
        Test_ValidateGrid_NutsAndGoalsMatch_OnRealLevel(grid, levelName); // THIS will fail for stone levels!
    }

    // Helper method to find first occurrence of tile type
    private Vector2Int FindTilePosition(TileType[,] grid, TileType tileType)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == tileType)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(1, 1); // fallback if not found
    }

    #region Individual Tests on Real Level Data

    // GridToWorld tests on real data
    private void Test_GridToWorld_Origin_ReturnsZero_OnRealLevel(string levelName)
    {
        Vector3 result = GridUtils.GridToWorld(Vector2Int.zero);
        Assert.AreEqual(Vector3.zero, result, $"[{levelName}] GridToWorld origin test failed");
    }

    private void Test_GridToWorld_PositiveCoordinates_ReturnsCorrectWorldPosition_OnRealLevel(Vector2Int pos, string levelName)
    {
        Vector3 result = GridUtils.GridToWorld(pos);
        Vector3 expected = new Vector3(pos.x, pos.y, 0f);
        Assert.AreEqual(expected, result, $"[{levelName}] GridToWorld positive coordinates test failed");
    }

    private void Test_GridToWorld_NegativeCoordinates_ReturnsCorrectWorldPosition_OnRealLevel(string levelName)
    {
        Vector2Int negativePos = new Vector2Int(-2, -3);
        Vector3 result = GridUtils.GridToWorld(negativePos);
        Vector3 expected = new Vector3(-2f, -3f, 0f);
        Assert.AreEqual(expected, result, $"[{levelName}] GridToWorld negative coordinates test failed");
    }

    private void Test_GridToWorld_CustomTileSize_ReturnsScaledPosition_OnRealLevel(Vector2Int pos, string levelName)
    {
        float tileSize = 2.5f;
        Vector3 result = GridUtils.GridToWorld(pos, tileSize);
        Vector3 expected = new Vector3(pos.x * tileSize, pos.y * tileSize, 0f);
        Assert.AreEqual(expected, result, $"[{levelName}] GridToWorld custom tile size test failed");
    }

    // GetPositionInDir tests on real data
    private void Test_GetPositionInDir_Up_ReturnsCorrectPosition_OnRealLevel(Vector2Int startPos, string levelName)
    {
        Vector2Int result = GridUtils.GetPositionInDir(startPos, Direction.Up, 1);
        Vector2Int expected = startPos + Vector2Int.up;
        Assert.AreEqual(expected, result, $"[{levelName}] GetPositionInDir Up test failed");
    }

    private void Test_GetPositionInDir_Down_ReturnsCorrectPosition_OnRealLevel(Vector2Int startPos, string levelName)
    {
        Vector2Int result = GridUtils.GetPositionInDir(startPos, Direction.Down, 1);
        Vector2Int expected = startPos + Vector2Int.down;
        Assert.AreEqual(expected, result, $"[{levelName}] GetPositionInDir Down test failed");
    }

    private void Test_GetPositionInDir_Left_ReturnsCorrectPosition_OnRealLevel(Vector2Int startPos, string levelName)
    {
        Vector2Int result = GridUtils.GetPositionInDir(startPos, Direction.Left, 1);
        Vector2Int expected = startPos + Vector2Int.left;
        Assert.AreEqual(expected, result, $"[{levelName}] GetPositionInDir Left test failed");
    }

    private void Test_GetPositionInDir_Right_ReturnsCorrectPosition_OnRealLevel(Vector2Int startPos, string levelName)
    {
        Vector2Int result = GridUtils.GetPositionInDir(startPos, Direction.Right, 1);
        Vector2Int expected = startPos + Vector2Int.right;
        Assert.AreEqual(expected, result, $"[{levelName}] GetPositionInDir Right test failed");
    }

    private void Test_GetPositionInDir_MultipleDistance_ReturnsCorrectPosition_OnRealLevel(Vector2Int startPos, string levelName)
    {
        Vector2Int result = GridUtils.GetPositionInDir(startPos, Direction.Up, 3);
        Vector2Int expected = startPos + Vector2Int.up * 3;
        Assert.AreEqual(expected, result, $"[{levelName}] GetPositionInDir multiple distance test failed");
    }

    private void Test_GetPositionInDir_ZeroDistance_TreatsAsOne_OnRealLevel(Vector2Int startPos, string levelName)
    {
        Vector2Int result = GridUtils.GetPositionInDir(startPos, Direction.Right, 0);
        Vector2Int expected = startPos + Vector2Int.right; // should treat 0 as 1
        Assert.AreEqual(expected, result, $"[{levelName}] GetPositionInDir zero distance test failed");
    }

    // ParseTileType tests on real data
    private void Test_ParseTileType_AllVariants_OnRealLevel(string levelName)
    {
        // Test all the parsing variants like in the original unit tests
        var wallResult = GridUtils.ParseTileType("W");
        Assert.AreEqual(TileType.Obstacle, wallResult.TileType, $"[{levelName}] ParseTileType Wall test failed");
        Assert.AreEqual(ObstacleType.BasicWall, wallResult.ObstacleType, $"[{levelName}] ParseTileType Wall ObstacleType test failed");

        var stoneWallResult = GridUtils.ParseTileType("WS");
        Assert.AreEqual(TileType.Obstacle, stoneWallResult.TileType, $"[{levelName}] ParseTileType StoneWall test failed");
        Assert.AreEqual(ObstacleType.StoneWall, stoneWallResult.ObstacleType, $"[{levelName}] ParseTileType StoneWall ObstacleType test failed");

        var veverkaResult = GridUtils.ParseTileType("V");
        Assert.AreEqual(TileType.Veverka, veverkaResult.TileType, $"[{levelName}] ParseTileType Veverka test failed");

        var nutResult = GridUtils.ParseTileType("N");
        Assert.AreEqual(TileType.Nut, nutResult.TileType, $"[{levelName}] ParseTileType BasicNut test failed");
        Assert.AreEqual(NutType.BasicNut, nutResult.NutType, $"[{levelName}] ParseTileType BasicNut NutType test failed");

        var stoneNutResult = GridUtils.ParseTileType("NS");
        Assert.AreEqual(TileType.Nut, stoneNutResult.TileType, $"[{levelName}] ParseTileType StoneNut test failed");
        Assert.AreEqual(NutType.StoneNut, stoneNutResult.NutType, $"[{levelName}] ParseTileType StoneNut NutType test failed");

        var goalResult = GridUtils.ParseTileType("G");
        Assert.AreEqual(TileType.Goal, goalResult.TileType, $"[{levelName}] ParseTileType BasicGoal test failed");
        Assert.AreEqual(GoalType.BasicGoal, goalResult.GoalType, $"[{levelName}] ParseTileType BasicGoal GoalType test failed");

        var holeResult = GridUtils.ParseTileType("H");
        Assert.AreEqual(TileType.Obstacle, holeResult.TileType, $"[{levelName}] ParseTileType Hole test failed");
        Assert.AreEqual(ObstacleType.Hole, holeResult.ObstacleType, $"[{levelName}] ParseTileType Hole ObstacleType test failed");

        var emptyResult = GridUtils.ParseTileType(".");
        Assert.AreEqual(TileType.Road, emptyResult.TileType, $"[{levelName}] ParseTileType Empty test failed");
        Assert.AreEqual(RoadType.Empty, emptyResult.RoadType, $"[{levelName}] ParseTileType Empty RoadType test failed");

        var roadResult = GridUtils.ParseTileType("R");
        Assert.AreEqual(TileType.Road, roadResult.TileType, $"[{levelName}] ParseTileType Road test failed");
        Assert.AreEqual(RoadType.BasicRoad, roadResult.RoadType, $"[{levelName}] ParseTileType Road RoadType test failed");

        var invalidResult = GridUtils.ParseTileType("X");
        Assert.AreEqual(TileType.ErrorTile, invalidResult.TileType, $"[{levelName}] ParseTileType Invalid test failed");

        var emptyStringResult = GridUtils.ParseTileType("");
        Assert.AreEqual(TileType.ErrorTile, emptyStringResult.TileType, $"[{levelName}] ParseTileType EmptyString test failed");

        var nullResult = GridUtils.ParseTileType(null);
        Assert.AreEqual(TileType.ErrorTile, nullResult.TileType, $"[{levelName}] ParseTileType Null test failed");
    }

    // ValidateGrid logic tests - these are the KEY tests that will reveal stone level issues!
    private void Test_ValidateGrid_ValidBasicGrid_ReturnsTrue_OnRealLevel(TileType[,] grid, string levelName)
    {
        // Count elements like ValidateGrid does
        int veverkaCount = 0, nutCount = 0, goalCount = 0, errorCount = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                switch (grid[x, y])
                {
                    case TileType.Veverka: veverkaCount++; break;
                    case TileType.Nut: nutCount++; break;
                    case TileType.Goal: goalCount++; break;
                    case TileType.ErrorTile: errorCount++; break;
                }
            }
        }

        // This simulates ValidateGrid_ValidBasicGrid_ReturnsTrue test
        bool shouldBeValid = (veverkaCount == 1 && nutCount > 0 && goalCount > 0 && errorCount == 0);
        Assert.IsTrue(shouldBeValid, $"[{levelName}] ValidBasicGrid test failed: Veverka={veverkaCount}, Nuts={nutCount}, Goals={goalCount}, Errors={errorCount}");

        Debug.Log($"[{levelName}] Grid stats: Veverka={veverkaCount}, Nuts={nutCount}, Goals={goalCount}, Errors={errorCount}");
    }

    private void Test_ValidateGrid_ExactlyOneVeverka_OnRealLevel(TileType[,] grid, string levelName)
    {
        int veverkaCount = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == TileType.Veverka) veverkaCount++;
            }
        }

        Assert.AreEqual(1, veverkaCount, $"[{levelName}] Should have exactly 1 veverka, but found {veverkaCount}");
    }

    private void Test_ValidateGrid_HasNuts_OnRealLevel(TileType[,] grid, string levelName)
    {
        int nutCount = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == TileType.Nut) nutCount++;
            }
        }

        Assert.IsTrue(nutCount > 0, $"[{levelName}] Should have at least 1 nut, but found {nutCount}");
    }

    private void Test_ValidateGrid_HasGoals_OnRealLevel(TileType[,] grid, string levelName)
    {
        int goalCount = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == TileType.Goal) goalCount++;
            }
        }

        Assert.IsTrue(goalCount > 0, $"[{levelName}] Should have at least 1 goal, but found {goalCount}");
    }

    private void Test_ValidateGrid_NoErrorTiles_OnRealLevel(TileType[,] grid, string levelName)
    {
        int errorCount = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == TileType.ErrorTile) errorCount++;
            }
        }

        Assert.AreEqual(0, errorCount, $"[{levelName}] Should have no error tiles, but found {errorCount}");
    }

    private void Test_ValidateGrid_NutsAndGoalsMatch_OnRealLevel(TileType[,] grid, string levelName)
    {
       
        // This test implements the logic that ValidateGrid has bypassed/commented out
        int nutCount = 0, goalCount = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                switch (grid[x, y])
                {
                    case TileType.Nut: nutCount++; break;
                    case TileType.Goal: goalCount++; break;
                }
            }
        }
       
        // ValidateGrid bypasses this check, but this individual test will catch the mismatch
        Assert.AreEqual(nutCount, goalCount,
            $"[{levelName}] NUTS/GOALS MISMATCH: Found {nutCount} nuts but {goalCount} goals. " +
            $"Some Stone and other levels have different counts which ValidateGrid ignores!");

        Debug.Log($"[{levelName}] Nuts/Goals check: {nutCount} nuts = {goalCount} goals ✓");
    }

    #endregion

    #endregion


    #region Manual Invalid Grid Test - Updated to use individual tests instead of ValidateGrid

    [Test]
    public void TestInvalidGridCreatedManually_WithIndividualGridUtilsTests()
    {
        // Create invalid grid manually to test individual GridUtils functions handle it properly
        string invalidLevelData = "W,W,W,W,W\n" +
                                  "W,.,N,.,W\n" +  // No Veverka
                                  "W,.,G,.,W\n" +
                                  "W,W,W,W,W";

        // Create a temporary TextAsset for testing
        TextAsset mockLevel = new TextAsset(invalidLevelData);

        // Test loading
        TileType[,] grid = GridUtils.LoadGridFromTextAsset(mockLevel);
        Assert.IsNotNull(grid, "LoadGridFromTextAsset should work even with invalid data");

        // Test individual validation functions (not ValidateGrid)
        try
        {
            Test_ValidateGrid_ExactlyOneVeverka_OnRealLevel(grid, "ManualInvalidTest");
            Assert.Fail("Should have failed because no Veverka found!");
        }
        catch (System.Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Should have exactly 1 veverka, but found 0"),
                "Exception should mention missing Veverka");
            Debug.Log($"✓ Individual test correctly caught missing Veverka: {ex.Message}");
        }

        // GridToWorld and GetPositionInDir should still work with coordinates that exist
        Vector2Int testPos = new Vector2Int(2, 2); // center of our 5x5 grid
        Vector3 worldPos = GridUtils.GridToWorld(testPos);
        Assert.AreEqual(new Vector3(2, 2, 0), worldPos, "GridToWorld should work even with invalid grid data");

        Vector2Int dirPos = GridUtils.GetPositionInDir(testPos, Direction.Up);
        Assert.AreEqual(new Vector2Int(2, 3), dirPos, "GetPositionInDir should work even with invalid grid data");

        Debug.Log("✓ Manual invalid grid test completed - individual tests work as expected");
    }

    #endregion

    #region Debug Information Test

    [Test]
    public void DebugPrintAllLevelsInfo_ForGridUtilsTesting()
    {
        // This test serves as debugging info and always passes
        string levelPath = Path.Combine(Application.dataPath, "Gameplay", "Level", "Data", "Levels");

        if (!Directory.Exists(levelPath))
        {
            Debug.LogWarning($"Level directory not found: {levelPath}");
            Assert.Inconclusive("Level directory not found - tests will be skipped");
            return;
        }

        string[] levelFiles = Directory.GetFiles(levelPath, "*.csv", SearchOption.AllDirectories);
        string[] productionFiles = System.Array.FindAll(levelFiles, file => !file.Contains("TestLevels"));

        Debug.Log($"=== LEVEL FILES INFO FOR INDIVIDUAL GRIDUTILS TESTS ===");
        Debug.Log($"Scanning directory: {levelPath}");
        Debug.Log($"Total .csv files found: {levelFiles.Length}");
        Debug.Log($"Production levels (will be tested): {productionFiles.Length}");
        Debug.Log($"Test levels (excluded): {levelFiles.Length - productionFiles.Length}");
        Debug.Log($"");

        Debug.Log("=== PRODUCTION LEVELS (tested by individual GridUtils functions) ===");
        if (productionFiles.Length > 0)
        {
            foreach (string file in productionFiles)
            {
                string fileName = Path.GetFileName(file);
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

        Debug.Log($"");
        Debug.Log("These levels will be tested with EVERY individual test from original GridUtilsTests:");
        Debug.Log("• All GridToWorld tests (origin, positive, negative, custom tile size)");
        Debug.Log("• All GetPositionInDir tests (up, down, left, right, multiple distance, zero distance)");
        Debug.Log("• All ParseTileType tests (all tile variants, invalid inputs)");
        Debug.Log("• All ValidateGrid logic tests (exactly one veverka, has nuts, has goals, no errors)");
        Debug.Log("• WARNING: Test_ValidateGrid_NutsAndGoalsMatch - THIS WILL FAIL FOR SOME STONE LEVELS!");
        Debug.Log(">>> This approach will reveal issues that ValidateGrid bypasses! <<<");

        // Always pass
        Assert.IsTrue(true);
    }

    #endregion
}