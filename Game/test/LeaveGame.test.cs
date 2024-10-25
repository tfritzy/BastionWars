using System.Numerics;
using System.Reflection;
using Castle.Components.DictionaryAdapter;
using Schema;
using TestHelpers;
using Tests;

namespace KeepLordWarriors;

[TestClass]
public class LeaveGameTests
{
    [TestMethod]
    public void Game_LeaveGame_ResetsAllKeeps()
    {
        Game game = new(TH.GetGameSettings());
        var p = TH.AddPlayer(game);

        for (int i = 0; i < 3; i++)
        {
            game.Map.KeepAt(i).Capture(p.Alliance, p.Id);
        }

        game.DisconnectPlayer(p.Id);

        Assert.IsFalse(game.PlayerIds.Contains(p.Id));
        Assert.IsFalse(game.Players.ContainsKey(p.Id));
        Assert.IsFalse(game.Map.Keeps.Values.Any(k => k.OwnerId == p.Id));
        Assert.IsFalse(game.Map.Keeps.Values.Any(k => k.Alliance == p.Alliance));
    }

    [TestMethod]
    public void Game_LeaveGame_ResetsAllSoldiers()
    {
        Game game = new(TH.GetGameSettings());
        var p = TH.AddPlayer(game);
        var keep = game.Map.Keeps.Values.First(k => k.OwnerId == p.Id);
        var target = game.Map.Keeps.Values.First(k => k.OwnerId != p.Id);

        for (int i = 0; i < 3; i++)
        {
            game.Map.KeepAt(i).Capture(p.Alliance, p.Id);
        }

        game.AttackKeep(keep.Id, target.Id);
        TH.UpdateGame(game, 1f);
        TH.UpdateGame(game, 1f);
        TH.UpdateGame(game, 1f);

        Assert.IsTrue(game.Map.Soldiers.Values.Any(k => k.OwnerId == p.Id));
        game.DisconnectPlayer(p.Id);
        Assert.IsFalse(game.Map.Soldiers.Values.Any(k => k.OwnerId == p.Id));
    }
}