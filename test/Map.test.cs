using System.Numerics;
using System.Text;
using KeepLordWarriors;
using Navigation;

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

                List<V2Int>? path = map.GetPathBetweenBastions(bastion.Id, other.Id);
                Assert.IsTrue(path?.Count > 0);
                Assert.AreEqual(V2Int.From(map.Grid.GetEntityPosition(bastion.Id)), path[0]);
                Assert.AreEqual(V2Int.From(map.Grid.GetEntityPosition(other.Id)), path[^1]);
            }
        }
    }

    [TestMethod]
    public void Map_ReadsBastionsCorrectly()
    {
        Map map = new(TestMaps.TenByFive);
        List<int> expectedAlliances = new() { 1, 0, 0, 0, 0, 2 };
        List<V2Int> expectedPositions = new()
        {
            new V2Int(1, 0),
            new V2Int(3, 0),
            new V2Int(2, 2),
            new V2Int(0, 3),
            new V2Int(2, 4),
            new V2Int(4, 4),
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
        ulong b0 = map.KeepAt(0).Id;
        ulong b1 = map.KeepAt(1).Id;
        ulong b2 = map.KeepAt(2).Id;

        StringBuilder actualOwnership = new();
        Dictionary<ulong, string> lookup = new() {
            {b0, "0"}, {b1, "1"}, {b2, "2"},
        };

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                actualOwnership.Append($"{lookup[map.BastionLands[new V2Int(x, y)]]}");
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
            for (int y = 0; y < map.Height; y++)
                numAvailableSpots += map.Traversable[x, y] == Constants.TRAVERSABLE ? 1 : 0;

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
    public void Map_AttackingDeploysSoldiers()
    {
        Map map = new(TestMaps.TenByFive);

        map.KeepAt(0).Capture(1);
        map.KeepAt(1).Capture(2);
        map.KeepAt(0).SetCount(archers: 2);
        map.AttackBastion(map.KeepAt(0).Id, map.KeepAt(1).Id);

        Assert.AreEqual(0, map.KeepAt(0).ArcherCount);
        Assert.AreEqual(2, map.Soldiers.Count);

        foreach (var soldier in map.Soldiers)
        {
            Assert.AreEqual(map.KeepAt(0).Id, soldier.SourceBastionId);
            Assert.AreEqual(map.KeepAt(1).Id, soldier.TargetBastionId);
            Assert.AreEqual(SoldierType.Archer, soldier.Type);
            Assert.AreEqual(1, soldier.Alliance);
            Assert.AreEqual(0, soldier.PathProgress);
            V2Int? gridPos = map.Grid.GetEntityGridPos(map.KeepAt(0).Id);
            Assert.AreEqual(gridPos, map.Grid.GetEntityGridPos(soldier.Id));
        }
    }
}