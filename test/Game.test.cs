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
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(0, bastion.GetCount(bastion.SoldierType));
        }
        game.Update(.2f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(1, bastion.GetCount(bastion.SoldierType));
        }
        game.Update(Game.AutoAccrualTime - .1f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(1, bastion.GetCount(bastion.SoldierType));
        }
    }

    [TestMethod]
    public void Game_DoesntAccrueIfWordMode()
    {
        Game game = new(new GameSettings(GenerationMode.Word, TestMaps.TenByFive));

        game.Update(Game.AutoAccrualTime + .1f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(0, bastion.GetCount(bastion.SoldierType));
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
                source: game.Map.Bastions[0].Id,
                target: game.Map.Bastions[1].Id);
        game.Map.AddSoldier(soldier, game.Map.Grid.GetEntityPosition(game.Map.Bastions[0].Id));
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
}