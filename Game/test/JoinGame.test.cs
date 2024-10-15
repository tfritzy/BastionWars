using System.Numerics;
using System.Reflection;
using Castle.Components.DictionaryAdapter;
using Schema;
using TestHelpers;
using Tests;

namespace KeepLordWarriors;

[TestClass]
public class JoinGameTests
{
    [TestMethod]
    public void Game_JoinGame_SendsInitialState()
    {
        Game game = new(TH.GetGameSettings());
        var p = TH.AddPlayer(game);
        var initialStates = p.MessageQueue.Where(m => m.InitialState != null).ToList();
        Assert.AreEqual(1, initialStates.Count);
        var s = initialStates[0].InitialState;
        Assert.AreEqual(game.Map.Width, s.MapWidth);
        Assert.AreEqual(game.Map.Height, s.MapHeight);
        for (int x = 0; x < game.Map.Width; x++)
        {
            for (int y = 0; y < game.Map.Height; y++)
            {
                Assert.AreEqual(game.Map.Tiles[x, y], s.Tiles[y * game.Map.Width + x]);
            }
        }

        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(1, s.Keeps.Count(k => k.Id == keep.Id));
            Schema.KeepState keepState = s.Keeps.First(k => k.Id == keep.Id);
            Assert.AreEqual(keep.ArcherCount, keepState.ArcherCount);
            Assert.AreEqual(keep.WarriorCount, keepState.WarriorCount);
            Assert.AreEqual(keep.Alliance, keepState.Alliance);
            Assert.AreEqual(keep.Name, keepState.Name);
            Assert.AreEqual(keep.Id, keepState.Id);
            Vector2 keepPos = game.Map.Grid.GetEntityPosition(keep.Id);
            Assert.AreEqual(keepPos.X, keepState.Pos.X);
            Assert.AreEqual(keepPos.Y, keepState.Pos.Y);
        }
    }

    [TestMethod]
    public void Game_JoinGame_IsGrantedAnEmptyKeep()
    {
        Game game = new Game(TH.GetGameSettings());
        var p = TH.AddPlayer(game);
        Assert.AreEqual(1, game.Map.Keeps.Values.Count(k => k.OwnerId == p.Id));
        Keep k = game.Map.Keeps.Values.First(k => k.OwnerId == p.Id);
        HashSet<int> otherAlliances = game.Map.Keeps.Values
            .Where(k => k.OwnerId != p.Id)
            .Select(k => k.Alliance)
            .ToHashSet();
        Assert.IsFalse(otherAlliances.Contains(k.Alliance));
        Assert.AreEqual(p.Alliance, k.Alliance);
    }

    [TestMethod]
    public void Game_JoinGame_DontGiveAnotherKeepOnRejoin()
    {
        Game game = new Game(TH.GetGameSettings());
        var p = TH.AddPlayer(game);
        game.JoinGame(p);
        Assert.AreEqual(1, game.Map.Keeps.Values.Count(k => k.OwnerId == p.Id));
    }

    [TestMethod]
    public void Game_JoinGame_ReturnsFalseIfFull()
    {
        Assert.Fail("Join game should return false if there's nowhere to put someone");
    }
}