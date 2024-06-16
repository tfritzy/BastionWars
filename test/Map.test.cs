using System.Numerics;
using KeepLordWarriors;

namespace Tests;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void Map_PlacesBastions()
    {
        Map map = new(Maps.ThirtyByTwenty);
        Assert.IsTrue(map.Bastions.Count > 0);

        foreach (var bastion in map.Bastions)
        {
            Vector2 pos = map.Grid.GetEntityPosition(bastion.Id);
            Assert.IsTrue(map.Traversable[(int)pos.X, (int)pos.Y] == 0);
        }
    }

    [TestMethod]
    public void Map_AllBastionsCanBeNavigatedBetween()
    {
        Map map = new(Maps.TenByFive);
        foreach (var bastion in map.Bastions)
        {
            foreach (var other in map.Bastions)
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
        Map map = new(Maps.TenByFive);
        List<int> expectedAlliances = new() { 1, 0, 0, 0, 0, 2 };
        List<Vector2Int> expectedPositions = new()
        {
            new Vector2Int(0, 0),
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
        for (int i = 0; i < map.Bastions.Count; i++)
        {
            Assert.AreEqual(expectedAlliances[i], map.Bastions[i].Alliance);
            Assert.AreEqual(expectedTypes[i], map.Bastions[i].SoldierType);
            Assert.AreEqual(expectedPositions[i], map.Grid.GetEntityGridPos(map.Bastions[i].Id));
        }
    }
}