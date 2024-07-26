using System.Numerics;
using KeepLordWarriors;

namespace Tests;

[TestClass]
public class SoldierTests
{
    [TestMethod]
    public void Soldier_WalksTowardsTarget()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep keep0 = map.KeepAt(0);
        Keep keep1 = map.KeepAt(1);
        var soldier = new Soldier(map, 0, SoldierType.Warrior, keep0.Id, keep1.Id);
        var path = map.GetPathBetweenBastions(keep0.Id, keep1.Id)!;
        map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        HashSet<Vector2Int?> visited = new();
        for (int i = 0; i < 200; i++)
        {
            visited.Add(map.Grid.GetEntityGridPos(soldier.Id));
            map.Update(.1f);
        }

        Console.WriteLine($"Expected path: {string.Join(", ", path)}");
        Console.WriteLine($"Visited path: {string.Join(", ", visited)}");
        CollectionAssert.IsSubsetOf(path, visited.ToArray());
    }

    [TestMethod]
    public void Soldier_BreachesTarget()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep keep0 = map.KeepAt(0);
        Keep keep1 = map.KeepAt(1);
        keep0.Capture(1);
        keep1.Capture(2);
        keep1.SetCount(archers: 2);
        var soldier = new Soldier(map, 1, SoldierType.Warrior, keep0.Id, keep1.Id);
        var path = map.GetPathBetweenBastions(keep0.Id, keep1.Id)!;
        map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        Assert.AreEqual(2, keep1.Alliance);
        for (int i = 0; i < 100; i++)
        {
            map.Update(.1f);
        }
        Assert.AreEqual(1, keep1.Alliance);
        Assert.AreEqual(1, keep1.WarriorCount);
        Assert.AreEqual(0, keep1.ArcherCount);
    }
}