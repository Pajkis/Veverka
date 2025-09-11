using UnityEngine;
using UnityEditor;
using NUnit.Framework;

/// <summary>
/// Unit tests for GridUtils - umístěné v Assets/Editor/Tests/
/// Funguje bez Assembly Definitions
/// </summary>
public class GridUtilsEditorTests
{
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
        Assert.AreEqual(TileType.Wall, result.TileType);
        Assert.AreEqual(WallType.BasicWall, result.WallType);
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
        Assert.AreEqual(TileType.Wall, result.TileType);
        Assert.AreEqual(WallType.StoneWall, result.WallType);
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
    public void ParseTileType_HoleGoal_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType("H");

        // Assert
        Assert.AreEqual(TileType.Goal, result.TileType);
        Assert.AreEqual(GoalType.HoleGoal, result.GoalType);
    }

    [Test]
    public void ParseTileType_Empty_ReturnsCorrectTile()
    {
        // Act
        var result = GridUtils.ParseTileType(".");

        // Assert
        Assert.AreEqual(TileType.Empty, result.TileType);
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

    [Test]
    public void ValidateGrid_ValidBasicGrid_ReturnsTrue()
    {
        // Arrange - simple 3x3 grid with 1 veverka, 1 nut, 1 goal
        TileType[,] grid = new TileType[3, 3] {
            { TileType.Wall, TileType.Wall, TileType.Wall },
            { TileType.Wall, TileType.Veverka, TileType.Nut },
            { TileType.Wall, TileType.Goal, TileType.Wall }
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
            { TileType.Wall, TileType.Wall, TileType.Wall },
            { TileType.Wall, TileType.Empty, TileType.Nut },
            { TileType.Wall, TileType.Goal, TileType.Wall }
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
            { TileType.Wall, TileType.Veverka, TileType.Wall },
            { TileType.Wall, TileType.Veverka, TileType.Nut },
            { TileType.Wall, TileType.Goal, TileType.Wall }
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
            { TileType.Wall, TileType.Wall, TileType.Wall },
            { TileType.Wall, TileType.Veverka, TileType.Empty },
            { TileType.Wall, TileType.Goal, TileType.Wall }
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
            { TileType.Wall, TileType.Wall, TileType.Wall },
            { TileType.Wall, TileType.Veverka, TileType.Nut },
            { TileType.Wall, TileType.Empty, TileType.Wall }
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
            { TileType.Wall, TileType.Wall, TileType.Wall },
            { TileType.Wall, TileType.Veverka, TileType.Nut },
            { TileType.ErrorTile, TileType.Goal, TileType.Wall }
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
            { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
            { TileType.Wall, TileType.Veverka, TileType.Nut, TileType.Wall },
            { TileType.Wall, TileType.Nut, TileType.Goal, TileType.Wall },
            { TileType.Wall, TileType.Goal, TileType.Wall, TileType.Wall }
        };

        // Act
        bool result = GridUtils.ValidateGrid(grid);

        // Assert
        Assert.IsTrue(result, "Valid grid with multiple nuts and goals should pass validation");
    }
}