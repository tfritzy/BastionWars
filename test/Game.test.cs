using System.Numerics;
using Tests;

namespace KeepLordWarriors;

[TestClass]
public class GameTests
{
    [TestMethod]
    public void Game_AutoAccrues()
    {
        Game game = new(new GameSettings(GenerationMode.AutoAccrue, TestMaps.TenByFive));

        game.Update(Game.AutoAccrualTime - .1f);
        foreach (Keep bastion in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount, bastion.GetCount(bastion.SoldierType));
        }
        game.Update(.2f);
        foreach (Keep bastion in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount + 1, bastion.GetCount(bastion.SoldierType));
        }
        game.Update(Game.AutoAccrualTime - .1f);
        foreach (Keep bastion in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount + 1, bastion.GetCount(bastion.SoldierType));
        }
    }

    [TestMethod]
    public void Game_DoesntAccrueIfWordMode()
    {
        Game game = new(new GameSettings(GenerationMode.Word, TestMaps.TenByFive));

        game.Update(Game.AutoAccrualTime + .1f);
        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount, keep.GetCount(keep.SoldierType));
        }
    }

    [TestMethod]
    public void Game_PlacesWords()
    {
        Game game = new Game(new GameSettings(GenerationMode.Word, TestMaps.TenByFive));

        Assert.AreEqual(Game.InitialWordCount, game.Map.Words.Values.Count(w => w != null));
        game.Update(Game.AutoAccrualTime + .1f);
        Assert.AreEqual(Game.InitialWordCount + 1, game.Map.Words.Values.Count(w => w != null));
        game.Update(.1f);
        Assert.AreEqual(Game.InitialWordCount + 1, game.Map.Words.Values.Count(w => w != null));
        game.Update(Game.WordPlacementTime + .1f);
        Assert.AreEqual(Game.InitialWordCount + 2, game.Map.Words.Values.Count(w => w != null));
    }

    [TestMethod]
    public void Game_ReportsSoldierPositions()
    {
        Game game = new(new GameSettings(GenerationMode.AutoAccrue, TestMaps.TenByFive));
        game.Update(Game.NetworkTickTime + .1f);
        var positionUpdate = game.outbox
            .First((u) => u.UpdateCase == Schema.OneofUpdate.UpdateOneofCase.AllSoldierPositions);
        Assert.AreEqual(0, positionUpdate.AllSoldierPositions.SoldierPositions.Count);
        Soldier soldier =
            new Soldier(
                map: game.Map,
                alliance: 0,
                type: SoldierType.Warrior,
                source: game.Map.KeepAt(0).Id,
                target: game.Map.KeepAt(1).Id);
        game.Map.AddSoldier(soldier, game.Map.Grid.GetEntityPosition(game.Map.KeepAt(0).Id));
        TH.ClearOutbox(game);
        game.Update(Game.NetworkTickTime + .1f);
        Vector2 newPos = game.Map.Grid.GetEntityPosition(soldier.Id);
        positionUpdate = game.outbox
            .First((u) => u.UpdateCase == Schema.OneofUpdate.UpdateOneofCase.AllSoldierPositions);
        Assert.AreEqual(1, positionUpdate.AllSoldierPositions.SoldierPositions.Count);
        Assert.AreEqual(soldier.Id, positionUpdate.AllSoldierPositions.SoldierPositions[0].Id);
        Assert.AreEqual(newPos.X, positionUpdate.AllSoldierPositions.SoldierPositions[0].Pos.X);
        Assert.AreEqual(newPos.Y, positionUpdate.AllSoldierPositions.SoldierPositions[0].Pos.Y);
    }

    [TestMethod]
    public void Game_IncrementsWordProgress()
    {
        Game game = new(new GameSettings(GenerationMode.Word, TestMaps.TenByFive));
        for (int i = 0; i < 10; i++)
            game.Update(Game.WordPlacementTime + .1f);

        Word firstWord = game.Map.Words.Values.First(w => w != null)!;
        ulong ownerId = game.Map.KeepLands[firstWord.Position];
        Keep keep = game.Map.Keeps[ownerId];

        for (int i = 0; i < firstWord.Text.Length; i++)
        {
            game.HandleKeystroke(firstWord.Text[i], keep.Alliance);
        }

        // Multiple words could have been completed.
        Assert.IsTrue(keep.GetCount(keep.SoldierType) > 0);
    }

    [TestMethod]
    public void Game_DoestCompleteNonOwnedWords()
    {
        Game game = new(new GameSettings(GenerationMode.Word, TestMaps.TenByFive));
        Keep allyKeep = game.Map.Keeps.Values.First(b => b.Alliance == 1);
        allyKeep.SetCount(archers: 0, warriors: 0);

        // Fill in words
        foreach (V2Int pos in game.Map.Words.Keys)
            game.Map.Words[pos] = new Word("a", pos);

        int numWordsOwned = game.Map.Words.Values.Count(w => w != null && game.Map.KeepLands[w.Position] == allyKeep.Id);
        game.HandleKeystroke('a', allyKeep.Alliance);
        Assert.AreEqual(numWordsOwned, game.Map.Words.Values.Count(w => w == null));
        Assert.AreEqual(numWordsOwned, allyKeep.GetCount(allyKeep.SoldierType));
    }


    [TestMethod]
    public void Map_AttackingDeploysSoldiers()
    {
        Game game = new(new GameSettings(GenerationMode.AutoAccrue, TestMaps.TenByFive));
        Map map = game.Map;

        map.KeepAt(0).Capture(1);
        map.KeepAt(1).Capture(2);
        map.KeepAt(0).SetCount(archers: 2, warriors: 0);
        game.AttackBastion(map.KeepAt(0).Id, map.KeepAt(1).Id);

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