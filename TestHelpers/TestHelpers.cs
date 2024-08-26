using System.Numerics;
using KeepLordWarriors;
using Schema;

namespace TestHelpers;

public static class TH
{
    public static GameSettings GetGameSettings(GenerationMode mode = GenerationMode.AutoAccrue, string? map = null)
    {
        return new GameSettings()
        {
            Map = map ?? TestMaps.TenByFive,
            GenerationMode = mode,
        };
    }

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

    public static List<Oneof_GameServerToPlayer> GetMessagesOfType(Game game, Oneof_GameServerToPlayer.MsgOneofCase type)
    {
        return game.Outbox.Where((u) => u.MsgCase == type).ToList();
    }

    public static Oneof_GameServerToPlayer? GetMessageSentToPlayerOfType(
        Game game,
        string playerId,
        Oneof_GameServerToPlayer.MsgOneofCase type)
    {
        Player player = game.Players[playerId];
        var packets = player.PendingPackets;

        Oneof_GameServerToPlayer? unchunked = null;
        do
        {
            unchunked = MessageChunker.ExtractFullUpdate(ref packets);
            if (unchunked?.MsgCase == type)
            {
                return unchunked;
            }
        } while (unchunked != null);

        return null;
    }
}