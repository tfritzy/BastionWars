using System.Numerics;
using Castle.Components.DictionaryAdapter;
using Schema;
using TestHelpers;
using Tests;

namespace KeepLordWarriors;

[TestClass]
public class GameTests
{
    [TestMethod]
    public void Game_AutoAccrues()
    {
        Game game = new(TH.GetGameSettings());

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
    public void Game_SendsInitialStateWhenPlayerJoins()
    {
        Game game = new(TH.GetGameSettings());
        TH.AddPlayer(game);
        var initialStates = TH.GetMessagesOfType(game, Oneof_GameServerToPlayer.MsgOneofCase.InitialState);
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
    public void Game_DisconnectPlayer()
    {
        Game game = new(TH.GetGameSettings());
        int initialPlayers = game.Players.Count;
        var player = TH.AddPlayer(game);
        Assert.AreEqual(initialPlayers + 1, game.Players.Count);
        game.DisconnectPlayer(player.Id);
        Assert.AreEqual(initialPlayers, game.Players.Count);
    }


    [TestMethod]
    public void Game_DoesntAccrueIfWordMode()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));

        game.Update(Game.AutoAccrualTime + .1f);
        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount, keep.GetCount(keep.SoldierType));
        }
    }

    [TestMethod]
    public void Game_PlacesWords()
    {
        Game game = new Game(TH.GetGameSettings(mode: GenerationMode.Word));

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
        Game game = new(TH.GetGameSettings());
        TH.AddPlayer(game);
        TH.AddPlayer(game);
        game.Update(Game.NetworkTickTime + .1f);
        var positionUpdate = game
            .Players.Values.First().MessageQueue
            .Where((m) => m.AllSoldierPositions != null)
            .First()
            .AllSoldierPositions;
        Assert.AreEqual(0, positionUpdate.SoldierPositions.Count);
        Soldier soldier = new(
            map: game.Map,
            alliance: 0,
            type: SoldierType.Warrior,
            source: game.Map.KeepAt(0).Id,
            target: game.Map.KeepAt(1).Id);
        game.Map.AddSoldier(soldier, game.Map.Grid.GetEntityPosition(game.Map.KeepAt(0).Id));
        game.Update(Game.NetworkTickTime + .1f);
        Vector2 newPos = game.Map.Grid.GetEntityPosition(soldier.Id);
        positionUpdate = game
            .Players.Values.First().MessageQueue
            .Where((m) => m.AllSoldierPositions != null)
            .Last()
            .AllSoldierPositions;
        Assert.AreEqual(1, positionUpdate.SoldierPositions.Count);
        Assert.AreEqual(soldier.Id, positionUpdate.SoldierPositions[0].Id);
        Assert.AreEqual(newPos.X, positionUpdate.SoldierPositions[0].Pos.X);
        Assert.AreEqual(newPos.Y, positionUpdate.SoldierPositions[0].Pos.Y);
    }

    [TestMethod]
    public void Game_IncrementsWordProgress()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        for (int i = 0; i < 10; i++)
            game.Update(Game.WordPlacementTime + .1f);

        Word firstWord = game.Map.Words.Values.First(w => w != null)!;
        uint ownerId = game.Map.KeepLands[firstWord.Position];
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
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        Keep allyKeep = game.Map.Keeps.Values.First(b => b.Alliance == 1);
        allyKeep.SetCount(archers: 0, warriors: 0);

        // Fill in words
        foreach (Vector2Int pos in game.Map.Words.Keys)
            game.Map.Words[pos] = new Word("a", pos);

        int numWordsOwned = game.Map.Words.Values.Count(w => w != null && game.Map.KeepLands[w.Position] == allyKeep.Id);
        game.HandleKeystroke('a', allyKeep.Alliance);
        Assert.AreEqual(numWordsOwned, game.Map.Words.Values.Count(w => w == null));
        Assert.AreEqual(numWordsOwned, allyKeep.GetCount(allyKeep.SoldierType));
    }

    [TestMethod]
    public void Map_AttackingDeploysSoldiers()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        Map map = game.Map;

        map.KeepAt(0).Capture(1);
        map.KeepAt(1).Capture(1);
        map.KeepAt(0).SetCount(archers: 2, warriors: 0);
        map.KeepAt(1).SetCount(archers: 2, warriors: 0);
        game.AttackBastion(map.KeepAt(0).Id, map.KeepAt(1).Id);
        game.Update(.0001f);

        Assert.AreEqual(0, map.KeepAt(0).ArcherCount);
        Assert.AreEqual(2, map.Soldiers.Count);
        Assert.AreEqual(0, map.KeepAt(0).DeploymentOrders.Count);

        foreach (var soldier in map.Soldiers)
        {
            Assert.AreEqual(map.KeepAt(0).Id, soldier.SourceBastionId);
            Assert.AreEqual(map.KeepAt(1).Id, soldier.TargetBastionId);
            Assert.AreEqual(SoldierType.Archer, soldier.Type);
            Assert.AreEqual(1, soldier.Alliance);
            Assert.AreEqual(0, soldier.PathProgress);
            Vector2Int? gridPos = map.Grid.GetEntityGridPos(map.KeepAt(0).Id);
            Assert.AreEqual(gridPos, map.Grid.GetEntityGridPos(soldier.Id));
        }

        for (int i = 0; i < 100; i++)
        {
            game.Update(.1f);
        }

        Assert.AreEqual(0, map.Soldiers.Count);

        game.AttackBastion(map.KeepAt(1).Id, map.KeepAt(0).Id);
        for (int i = 0; i < 100; i++)
            game.Update(.1f);
        Assert.AreEqual(4, map.KeepAt(0).ArcherCount);
        Assert.AreEqual(0, map.KeepAt(1).ArcherCount);

    }

    [TestMethod]
    public void Game_HandleMessage_AttackingDeploysSoldiers()
    {
        Game game = new(TH.GetGameSettings());
        Map map = game.Map;
        map.KeepAt(0).Capture(1);
        map.KeepAt(1).Capture(2);
        map.KeepAt(0).SetCount(archers: 10, warriors: 5);
        game.HandleCommand(new Oneof_PlayerToGameServer()
        {
            IssueDeploymentOrder = new IssueDeploymentOrder()
            {
                SourceKeep = map.KeepAt(0).Id,
                TargetKeep = map.KeepAt(1).Id,
                Percent = .5f,
                SoldierType = SoldierType.Archer
            }
        });
        game.Update(.01);

        Assert.AreEqual(5, map.KeepAt(0).ArcherCount);
        Assert.AreEqual(5, map.Soldiers.Count);

        foreach (var soldier in map.Soldiers)
        {
            Assert.AreEqual(map.KeepAt(0).Id, soldier.SourceBastionId);
            Assert.AreEqual(map.KeepAt(1).Id, soldier.TargetBastionId);
            Assert.AreEqual(SoldierType.Archer, soldier.Type);
            Assert.AreEqual(1, soldier.Alliance);
            Assert.AreEqual(0, soldier.PathProgress);
            Vector2Int? gridPos = map.Grid.GetEntityGridPos(map.KeepAt(0).Id);
            Assert.AreEqual(gridPos, map.Grid.GetEntityGridPos(soldier.Id));
        }
    }

    [TestMethod]
    public void Game_SendsKeepOccupancyChanges()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        Map map = game.Map;
        TH.AddPlayer(game);

        game.Update(1f);

        var keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(0, keepUpdates.Count);
        game.Map.KeepAt(0).SetCount(archers: 4, warriors: 1);
        game.Map.KeepAt(1).SetCount(warriors: 6, archers: 3);
        keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(0, keepUpdates.Count);
        game.Update(1f);
        keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(1, keepUpdates.Count);
        Assert.AreEqual(2, keepUpdates.First().KeepUpdates.Count);

        var firstKeepUpdate = keepUpdates.First().KeepUpdates.First(u => u.Id == game.Map.KeepAt(0).Id);
        var secondKeepUpdate = keepUpdates.First().KeepUpdates.First(u => u.Id == game.Map.KeepAt(1).Id);
        Assert.AreEqual(4, firstKeepUpdate.ArcherCount);
        Assert.AreEqual(1, firstKeepUpdate.WarriorCount);
        Assert.AreEqual(6, secondKeepUpdate.WarriorCount);
        Assert.AreEqual(3, secondKeepUpdate.ArcherCount);

        // doesn't double send message
        game.Update(1f);
        keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(1, keepUpdates.Count);
    }
}