using System.Numerics;
using KeepLordWarriors;
using Schema;
using TestHelpers;

namespace Tests;

[TestClass]
public class SoldierTests
{
    [TestMethod]
    public void Soldier_WalksTowardsTarget()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep0 = game.Map.KeepAt(0);
        Keep keep1 = game.Map.KeepAt(1);
        TH.ErradicateAllArchers(game);
        var soldier = new Soldier(game, 0, SoldierType.Warrior, keep0.Id, keep1.Id, 0);
        var path = game.Map.GetPathBetweenKeeps(keep0.Id, keep1.Id)!;
        game.Map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        HashSet<Vector2Int?> visited = new();
        for (int i = 0; i < 200; i++)
        {
            visited.Add(game.Map.Grid.GetEntityGridPos(soldier.Id));
            TH.UpdateGame(game, .1f);
        }

        Console.WriteLine($"Expected path: {string.Join(", ", path)}");
        Console.WriteLine($"Visited path: {string.Join(", ", visited)}");
        CollectionAssert.IsSubsetOf(path, visited.ToArray());
    }

    [TestMethod]
    public void Soldier_BreachesTarget()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep0 = game.Map.KeepAt(0);
        Keep keep1 = game.Map.KeepAt(1);
        keep0.Capture(1);
        keep1.Capture(2);
        keep1.SetCount(archers: 2);
        var soldier = new Soldier(game, 1, SoldierType.Warrior, keep0.Id, keep1.Id, 0);
        var path = game.Map.GetPathBetweenKeeps(keep0.Id, keep1.Id)!;
        game.Map.AddSoldier(soldier, new Vector2(path[0].X + .5f, path[0].Y + .5f));

        Assert.AreEqual(2, keep1.Alliance);
        for (int i = 0; i < 200; i++)
        {
            TH.UpdateGame(game, .1f);
        }
        Assert.AreEqual(1, keep1.Alliance);
        Assert.AreEqual(1, keep1.WarriorCount);
        Assert.AreEqual(0, keep1.ArcherCount);
    }

    [TestMethod]
    public void Soldier_SendsMessageOnSpawn()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        var p = TH.AddPlayer(game);
        Keep keep0 = game.Map.KeepAt(0);
        Keep keep1 = game.Map.KeepAt(1);
        keep0.Capture(1);
        keep1.Capture(2);
        keep1.SetCount(archers: 2);

        TH.UpdateGame(game, Game.NetworkTickTime);
        var newSoldiers = p.MessageQueue.Where(m => m.NewSoldiers != null).ToList();
        Assert.AreEqual(0, newSoldiers.Count);

        keep0.SetDeploymentOrder(keep1.Id);

        TH.UpdateGame(game, Game.NetworkTickTime);
        newSoldiers = p.MessageQueue.Where(m => m.NewSoldiers != null).ToList();
        Assert.AreEqual(1, newSoldiers.Count);

        var msg = newSoldiers.First();
        var expectedSoldiers = game.Map.Soldiers.Values;
        Assert.AreEqual(expectedSoldiers.Count, msg.NewSoldiers.Soldiers.Count);
        Assert.IsTrue(msg.NewSoldiers.Soldiers.All(s => expectedSoldiers.Any(es => es.Id == s.Id)));

        foreach (NewSoldier newSoldier in msg.NewSoldiers.Soldiers)
        {
            Soldier corresponding = expectedSoldiers.First(s => s.Id == newSoldier.Id);
            Assert.AreEqual(corresponding.Id, newSoldier.Id);
            Assert.AreEqual(corresponding.MovementSpeed, newSoldier.MovementSpeed);
            Assert.AreEqual(corresponding.Type, newSoldier.Type);
            Assert.AreEqual(corresponding.SourceKeepId, newSoldier.SourceKeepId);
            Assert.AreEqual(corresponding.TargetKeepId, newSoldier.TargetKeepId);
            Assert.AreEqual(corresponding.RowOffset, newSoldier.RowOffset);
        }
    }
}