using System.Numerics;
using System.Reflection;
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
        Game game = new(TH.GetGameSettings(mode: GenerationMode.AutoAccrue));

        TH.UpdateGame(game, Game.AutoAccrualTime - .1f);
        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount, keep.GetCount(keep.SoldierType));
        }
        TH.UpdateGame(game, .2f);
        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount + 1, keep.GetCount(keep.SoldierType));
        }
        TH.UpdateGame(game, Game.AutoAccrualTime - .1f);
        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount + 1, keep.GetCount(keep.SoldierType));
        }
    }

    [TestMethod]
    public void Game_JoinGame_SendsInitialState()
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
    public void Game_JoinGame_ReturnsFalseIfFull()
    {
        Assert.Fail("Join game should return false if there's nowhere to put someone");
    }

    [TestMethod]
    public void Game_InitialStateHasPathBetweenAllKeeps()
    {
        Game game = new(TH.GetGameSettings());
        TH.AddPlayer(game);
        var initialStates = TH.GetMessagesOfType(game, Oneof_GameServerToPlayer.MsgOneofCase.InitialState);
        var s = initialStates[0].InitialState;

        for (int i = 0; i < game.Map.Keeps.Count; i++)
        {
            var source = s.Keeps.Where(k => k.Id == game.Map.KeepAt(i).Id).First();
            for (int j = 0; j < game.Map.Keeps.Count; j++)
            {
                if (i == j)
                    continue;

                var target = s.Keeps.Where(k => k.Id == game.Map.KeepAt(j).Id).First();
                var targetPaths = source.Paths.Where(p => p.TargetId == game.Map.KeepAt(j).Id).ToList();
                Assert.AreEqual(1, targetPaths.Count);
                var path = targetPaths.First();
                Assert.IsNotNull(path);
                Assert.AreEqual(new Vector2Int(source.Pos).ToSchema(), path.Path.First());
                Assert.AreEqual(new Vector2Int(target.Pos).ToSchema(), path.Path.Last());
            }
        }
    }

    [TestMethod]
    public void Game_InitialStateHasInitialGrownFields()
    {
        Game game = new(TH.GetGameSettings());
        Field f1 = game.Map.Fields.Values.First();
        f1.RemainingGrowthTime = 1f;
        Field f2 = game.Map.Fields.Values.Last();
        f2.HandleKeystroke(f2.Text[0]);
        TH.AddPlayer(game);
        var initialStates = TH.GetMessagesOfType(game, Oneof_GameServerToPlayer.MsgOneofCase.InitialState);
        var state = initialStates[0].InitialState;

        Assert.IsTrue(state.GrownFields.Count > 0);
        foreach (Vector2Int pos in game.Map.Fields.Keys)
        {
            if (game.Map.Fields.TryGetValue(pos, out Field? field))
            {
                GrownField? stateField = state.GrownFields.FirstOrDefault(
                    f => f.GridPos.X == pos.X && f.GridPos.Y == pos.Y);

                if (field.RemainingGrowthTime > 0)
                {
                    Assert.IsNull(stateField);
                }
                else
                {
                    Assert.AreEqual(field!.Text, stateField!.Text);
                    Assert.AreEqual(field.TypedIndex, stateField.Progress);
                }
            }
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

        TH.UpdateGame(game, Game.AutoAccrualTime + .1f);
        foreach (Keep keep in game.Map.Keeps.Values)
        {
            Assert.AreEqual(Keep.StartTroopCount, keep.GetCount(keep.SoldierType));
        }
    }

    [TestMethod]
    public void Game_FieldsStartGrown()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        foreach (Field f in game.Map.Fields.Values)
        {
            Assert.AreEqual(0, f.RemainingGrowthTime);
        }
    }

    [TestMethod]
    public void Game_IncrementsWordProgress()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        var p = TH.AddPlayer(game);
        Keep k = game.Map.Keeps.Values.First(k => k.OwnerId == p.Id);
        Field field = game.Map.Fields.Values.First(f => game.Map.KeepLands[f.Position] == k.Id)!;
        int lastCount = k.GetCount(k.SoldierType);
        string text = field.Text;

        for (int i = 0; i < field.Text.Length; i++)
        {
            game.HandleKeystroke(field.Text[i], k.OwnerId!);
            Assert.IsTrue(k.GetCount(k.SoldierType) > lastCount);

            if (i != text.Length - 1)
                Assert.AreEqual(0, field.RemainingGrowthTime);
        }

        Assert.AreNotEqual(text, field.Text);
        Assert.AreEqual(0, field.TypedIndex);
        Assert.AreEqual(Field.GROWTH_TIME, field.RemainingGrowthTime);

        TH.UpdateGame(game, Game.NetworkTickTime);

        var harvestedFieldMsg = p.MessageQueue.Where(m => m.RemovedWords != null).ToList();
        Assert.AreEqual(1, harvestedFieldMsg.Count);
        Assert.AreEqual(field.Position.ToSchema(), harvestedFieldMsg.First().RemovedWords.Positions.First());
    }

    [TestMethod]
    public void Game_DoestCompleteNonOwnedWords()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        var player = TH.AddPlayer(game);
        Keep allyKeep = game.Map.Keeps.Values.First(b => b.OwnerId == player.Id);
        allyKeep.SetCount(archers: 0, warriors: 0);

        foreach (Field f in game.Map.Fields.Values)
            f.TestSetText("a");

        int numWordsOwned = game.Map.Fields.Values.Count(
            w => w != null && game.Map.KeepLands[w.Position] == allyKeep.Id);
        game.HandleKeystroke('a', allyKeep.OwnerId!);
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
        game.AttackKeep(map.KeepAt(0).Id, map.KeepAt(1).Id);
        TH.UpdateGame(game, .0001f);

        Assert.AreEqual(0, map.KeepAt(0).ArcherCount);
        Assert.AreEqual(2, map.Soldiers.Count);
        Assert.AreEqual(0, map.KeepAt(0).DeploymentOrders.Count);

        foreach (var soldier in map.Soldiers.Values)
        {
            Assert.AreEqual(map.KeepAt(0).Id, soldier.SourceKeepId);
            Assert.AreEqual(map.KeepAt(1).Id, soldier.TargetKeepId);
            Assert.AreEqual(SoldierType.Archer, soldier.Type);
            Assert.AreEqual(1, soldier.Alliance);
            Assert.AreEqual(0, soldier.PathIndex);
            Vector2Int? gridPos = map.Grid.GetEntityGridPos(map.KeepAt(0).Id);
            Assert.AreEqual(gridPos, map.Grid.GetEntityGridPos(soldier.Id));
        }

        for (int i = 0; i < 100; i++)
        {
            TH.UpdateGame(game, .1f);
        }

        Assert.AreEqual(0, map.Soldiers.Count);

        game.AttackKeep(map.KeepAt(1).Id, map.KeepAt(0).Id);
        for (int i = 0; i < 100; i++)
            TH.UpdateGame(game, .1f);
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
        TH.UpdateGame(game, .01f);

        Assert.AreEqual(10 - Keep.MaxTroopsPerWave, map.KeepAt(0).ArcherCount);
        Assert.AreEqual(Keep.MaxTroopsPerWave, map.Soldiers.Count);

        foreach (var soldier in map.Soldiers.Values)
        {
            Assert.AreEqual(map.KeepAt(0).Id, soldier.SourceKeepId);
            Assert.AreEqual(map.KeepAt(1).Id, soldier.TargetKeepId);
            Assert.AreEqual(SoldierType.Archer, soldier.Type);
            Assert.AreEqual(1, soldier.Alliance);
            Assert.AreEqual(0, soldier.PathIndex);
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

        TH.UpdateGame(game, 1f);

        var keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(0, keepUpdates.Count);
        game.Map.KeepAt(0).SetCount(archers: 4, warriors: 1);
        game.Map.KeepAt(1).SetCount(warriors: 6, archers: 3);
        keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(0, keepUpdates.Count);
        TH.UpdateGame(game, 1f);
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
        TH.UpdateGame(game, 1f);
        keepUpdates = TH.GetKeepUpdateMessages(game.Players.Values.First());
        Assert.AreEqual(1, keepUpdates.Count);
    }
}