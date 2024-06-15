using System.Numerics;
using KeepLordWarriors;

namespace Tests;

[TestClass]
public class SoldierTests
{
    [TestMethod]
    public void Soldier_WalksTowardsTarget()
    {
        Map map = new(10, 10);
        var soldier = new Soldier(map, 0, SoldierType.Warrior, map.Bastions[0].Id, map.Bastions[1].Id);
        var path = map.GetPathBetweenBastions(map.Bastions[0].Id, map.Bastions[1].Id)!;
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
        Map map = new(10, 10);
        map.Bastions[0].Capture(1);
        map.Bastions[1].Capture(2);
        map.Bastions[1].SetCount(archers: 2);
        var soldier = new Soldier(map, 1, SoldierType.Warrior, map.Bastions[0].Id, map.Bastions[1].Id);
        var path = map.GetPathBetweenBastions(map.Bastions[0].Id, map.Bastions[1].Id)!;
        map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        Assert.AreEqual(2, map.Bastions[1].Alliance);
        HashSet<Vector2Int> visited = new();
        for (int i = 0; i < 100; i++)
        {
            map.Update(.1f);
        }
        Assert.AreEqual(1, map.Bastions[1].Alliance);
        Assert.AreEqual(1, map.Bastions[1].WarriorCount);
        Assert.AreEqual(0, map.Bastions[1].ArcherCount);
    }
}