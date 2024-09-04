using System.Numerics;
using System.Text;
using KeepLordWarriors;
using Navigation;
using TestHelpers;

namespace Tests;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void Map_PlacesBastions()
    {
        KeepLordWarriors.Map map = new(TestMaps.ThirtyByTwenty);
        Assert.IsTrue(map.Keeps.Count > 0);

        foreach (var bastion in map.Keeps.Values)
        {
            Vector2 pos = map.Grid.GetEntityPosition(bastion.Id);
            Assert.IsTrue(map.Traversable[(int)pos.X, (int)pos.Y] == 0);
        }
    }

    [TestMethod]
    public void Map_AllBastionsCanBeNavigatedBetween()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        foreach (var bastion in map.Keeps.Values)
        {
            foreach (var other in map.Keeps.Values)
            {
                if (bastion == other)
                {
                    continue;
                }

                List<Vector2Int>? path = map.GetPathBetweenBastions(bastion.Id, other.Id);
                Assert.IsTrue(path?.Count > 0);
                Assert.AreEqual(Vector2Int.From(map.Grid.GetEntityPosition(bastion.Id)), path[0]);
                Assert.AreEqual(Vector2Int.From(map.Grid.GetEntityPosition(other.Id)), path[^1]);
            }
        }
    }

    [TestMethod]
    public void Map_ReadsBastionsCorrectly()
    {
        Map map = new(TestMaps.TenByFive);
        List<int> expectedAlliances = new() { 1, 0, 0, 0, 0, 2 };
        List<Vector2Int> expectedPositions = new()
        {
            new Vector2Int(1, 0),
            new Vector2Int(3, 0),
            new Vector2Int(2, 2),
            new Vector2Int(0, 3),
            new Vector2Int(2, 4),
            new Vector2Int(4, 4),
        };
        List<SoldierType> expectedTypes = new()
        {
            SoldierType.Warrior,
            SoldierType.Archer,
            SoldierType.Warrior,
            SoldierType.Archer,
            SoldierType.Archer,
            SoldierType.Warrior
        };

        for (int i = 0; i < map.Keeps.Count; i++)
        {
            Assert.AreEqual(expectedAlliances[i], map.Keeps.Values.ToList()[i].Alliance);
            Assert.AreEqual(expectedTypes[i], map.Keeps.Values.ToList()[i].SoldierType);
            Assert.AreEqual(expectedPositions[i], map.Grid.GetEntityGridPos(map.Keeps.Values.ToList()[i].Id));
        }
    }

    [TestMethod]
    public void Map_CalculatesBastionLands()
    {
        string rawMap = @"
            W . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . W . W . . . . .
            </>
            1 . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . 0 . 2 . . . . .
            </>
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0";
        KeepLordWarriors.Map map = new(rawMap);
        uint b0 = map.KeepAt(0).Id;
        uint b1 = map.KeepAt(1).Id;
        uint b2 = map.KeepAt(2).Id;

        StringBuilder actualOwnership = new();
        Dictionary<uint, string> lookup = new() {
            {b0, "0"}, {b1, "1"}, {b2, "2"},
        };

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                actualOwnership.Append($"{lookup[map.KeepLands[new Vector2Int(x, y)]]}");
                if (x != map.Width - 1)
                    actualOwnership.Append(" ");
            }
            if (y != map.Height - 1)
                actualOwnership.Append($"\n");
        }

        Console.WriteLine(actualOwnership);

        string expectedOwnership =
            "0 0 0 0 0 1 1 2 2 2\n" +
            "0 0 0 0 1 1 2 2 2 2\n" +
            "0 0 0 1 1 2 2 2 2 2\n" +
            "1 1 1 1 2 2 2 2 2 2\n" +
            "1 1 1 1 2 2 2 2 2 2";
        Assert.AreEqual(expectedOwnership, actualOwnership.ToString());
    }

    [TestMethod]
    public void Map_PlacesWords()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        int numAvailableSpots = 0;
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                if (x % 2 == 0 && y % 2 == 0)
                {
                    numAvailableSpots += map.Traversable[x, y] == Navigation.Constants.TRAVERSABLE ? 1 : 0;
                }
            }
        }
        Assert.AreEqual(numAvailableSpots, map.Words.Keys.Count);
        Assert.AreEqual(0, map.Words.Values.Count(w => w != null));
        map.PlaceWord();
        Assert.AreEqual(1, map.Words.Values.Count(w => w != null));
        map.PlaceWord();
        Assert.AreEqual(2, map.Words.Values.Count(w => w != null));

        for (int i = 0; i < 100; i++)
        {
            map.PlaceWord();
        }

        Assert.AreEqual(numAvailableSpots, map.Words.Values.Count(w => w != null));
    }

    [TestMethod]
    public void Map_RenderTiles()
    {
        KeepLordWarriors.Map map = new(TestMaps.ThreeByThree);
        RenderTType[,] expected = new RenderTType[,]
        {
            {RenderTType.FullWater, RenderTType.FullWater, RenderTType.FullWater, RenderTType.FullWater},
            {RenderTType.L_0001, RenderTType.L_0010, RenderTType.L_0001, RenderTType.L_0010},
            {RenderTType.L_0101, RenderTType.L_1011, RenderTType.L_0110, RenderTType.L_1000},
            {RenderTType.L_0100, RenderTType.L_1100, RenderTType.L_1000, RenderTType.FullWater},
        };

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Assert.AreEqual(
                    expected[y, x],
                    map.RenderTiles[x, y],
                    $"Index ({x}, {y}) incorrect. Expected {expected[y, x]}, actual{map.RenderTiles[x, y]}");
            }
        }
    }
}