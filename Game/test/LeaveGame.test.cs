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
        Assert.Fail("No soldiers should belong to the leaving player id");
    }
}