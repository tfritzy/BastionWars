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

    public static Player AddPlayer(Game game)
    {
        int playerCount = game.Players.Count;
        var player = new Player(name: $"test{playerCount}", id: playerCount.ToString());
        game.JoinGame(player);
        return player;
    }

    public static List<Oneof_GameServerToPlayer> GetMessagesOfType(Game game, Oneof_GameServerToPlayer.MsgOneofCase type)
    {
        return game.Outbox.Where((u) => u.MsgCase == type).ToList();
    }

    public static List<AllKeepUpdates> GetKeepUpdateMessages(Player player)
    {
        return player.MessageQueue
            .Select(m => m.KeepUpdates != null ? m.KeepUpdates : null)
            .Where(m => m != null)
            .ToList()!;
    }

    public static void UpdateGame(Game game, float deltaTime)
    {
        Time.Update(Time.Now + deltaTime);
        game.Update();
    }

    public static void UpdateMap(Map map, float deltaTime)
    {
        Time.Update(Time.Now + deltaTime);
        map.Update();
    }

    public static void UpdateKeep(Keep keep, float deltaTime)
    {
        Time.Update(Time.Now + deltaTime);
        keep.Update();
    }

    public static bool AssertIsApproximately(Vector3 v1, Vector3 v2, float tolerance = 0.1f)
    {
        Vector3 difference = v1 - v2;
        return difference.Length() <= tolerance;
    }
}