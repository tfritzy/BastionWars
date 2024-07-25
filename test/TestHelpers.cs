using System.Numerics;
using KeepLordWarriors;
using Schema;

namespace Tests;

public static class TH
{
    public static SpacialPartitioning.Entity BuildEntity(float x, float y, float radius = .2f)
    {
        return new SpacialPartitioning.Entity(new Vector2(x, y), KeepLordWarriors.IdGenerator.NextId(), radius);
    }

    public static SpacialPartitioning.Entity AddNewEntity(SpacialPartitioning.Grid grid, float x, float y, float radius = .2f)
    {
        SpacialPartitioning.Entity entity = BuildEntity(x, y, radius);
        grid.AddEntity(entity);
        return entity;
    }

    public static Soldier BuildEnemySoldier(SoldierType type, int ofAlliance, KeepLordWarriors.Map map)
    {
        return new Soldier(map, ofAlliance + 1, type, map.KeepAt(0).Id, map.KeepAt(1).Id);
    }

    public static Soldier BuildAllySoldier(SoldierType type, int ofAlliance, KeepLordWarriors.Map map)
    {
        return new Soldier(map, ofAlliance, type, map.KeepAt(0).Id, map.KeepAt(1).Id);
    }

    public static void ClearOutbox(Game game)
    {
        game.Outbox.Clear();
    }

    public static void AddPlayer(Game game)
    {
        int playerCount = game.Players.Count;
        game.JoinGame(new Player(name: $"test{playerCount}", id: playerCount.ToString()));
    }

    public static List<Schema.OneofUpdate> GetMessagesOfType(Game game, Schema.OneofUpdate.UpdateOneofCase type)
    {
        return game.Outbox.Where((u) => u.UpdateCase == type).ToList();
    }

    // public static List<Schema.OneofUpdate> GetMessagesSentToPlayerOfType(
    //     Game game,
    //     string playerId,
    //     Schema.OneofUpdate.UpdateOneofCase type)
    // {
    //     Player player = game.Players[playerId];
    //     var packets = player.PendingPackets;

    //     var unchunked = MessageChunker.ExtractFullUpdate(ref packets);
    // }
}