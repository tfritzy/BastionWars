using System.Numerics;
using KeepLordWarriors;

namespace Tests;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void Map_PlacesBastions()
    {
        Map map = new(50, 30);
        Assert.IsTrue(map.Bastions.Count > 0);

        foreach (var bastion in map.Bastions)
        {
            Vector2 pos = map.Grid.GetEntityPosition(bastion.Id);
            Assert.IsTrue(map.Travelable[(int)pos.X, (int)pos.Y] == 0);
        }
    }

    [TestMethod]
    public void Map_AllBastionsCanBeNavigatedBetween()
    {
        Map map = new(5, 5);
        foreach (var bastion in map.Bastions)
        {
            foreach (var other in map.Bastions)
            {
                if (bastion == other)
                {
                    continue;
                }

                List<Vector2Int> path = map.GetPathBetweenBastions(bastion.Id, other.Id);
                Assert.IsTrue(path.Count > 0);
                Assert.AreEqual(Vector2Int.From(map.Grid.GetEntityPosition(bastion.Id)), path[0]);
                Assert.AreEqual(Vector2Int.From(map.Grid.GetEntityPosition(other.Id)), path[^1]);
            }
        }
    }

    [TestMethod]
    public void Map_BastionAttackDeploysTroopsOverTime()
    {
        Map map = new(5, 5);
        Bastion bastion = map.Bastions[0];
        bastion.SetCount(archers: 30);

        Assert.AreEqual(30, bastion.GetCount(SoldierType.Archer));
        map.AttackBastion(bastion.Id, map.Bastions[1].Id);
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Archer));
    }
}