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
        var soldier = new Soldier(map, 0, SoldierType.Warrior, map.Keeps[0].Id, map.Keeps[1].Id);
        var path = map.GetPathBetweenBastions(map.Keeps[0].Id, map.Keeps[1].Id)!;
        map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        HashSet<V2Int?> visited = new();
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
        map.Keeps[0].Capture(1);
        map.Keeps[1].Capture(2);
        map.Keeps[1].SetCount(archers: 2);
        var soldier = new Soldier(map, 1, SoldierType.Warrior, map.Keeps[0].Id, map.Keeps[1].Id);
        var path = map.GetPathBetweenBastions(map.Keeps[0].Id, map.Keeps[1].Id)!;
        map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        Assert.AreEqual(2, map.Keeps[1].Alliance);
        HashSet<V2Int> visited = new();
        for (int i = 0; i < 100; i++)
        {
            map.Update(.1f);
        }
        Assert.AreEqual(1, map.Keeps[1].Alliance);
        Assert.AreEqual(1, map.Keeps[1].WarriorCount);
        Assert.AreEqual(0, map.Keeps[1].ArcherCount);
    }
}