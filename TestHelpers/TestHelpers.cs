using System.Numerics;
using KeepLordWarriors;
using Schema;

namespace TestHelpers;

public static class TH
{
    public static GameSettings GetGameSettings(GenerationMode mode = GenerationMode.Word, string? map = null)
    {
        return new GameSettings()
        {
            Map = map ?? TestMaps.TenByFive,
            GenerationMode = mode,
            Seed = 12
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

    public static Soldier BuildEnemySoldier(SoldierType type, int ofAlliance, Game game)
    {
        return new Soldier(game, null, ofAlliance + 1, type, game.Map.KeepAt(0).Id, game.Map.KeepAt(1).Id, 0);
    }

    public static Soldier BuildAllySoldier(SoldierType type, int ofAlliance, Game game)
    {
        return new Soldier(game, null, ofAlliance, type, game.Map.KeepAt(0).Id, game.Map.KeepAt(1).Id, 0);
    }

    public static void ClearOutbox(Game game)
    {
        game.Outbox.Clear();
    }

    public static Player AddPlayer(Game game)
    {
        int playerCount = game.Players.Count;
        var player = new Player(name: $"test{playerCount}", id: $"plyr_000{playerCount}");
        game.JoinGame(player);
        return player;
    }

    public static Player AddAI(Game game)
    {
        int playerCount = game.Players.Count;
        var player = new Player(name: $"test_bot{playerCount}", id: $"plyr_000{playerCount}");
        player.AIConfig = new();
        game.JoinGame(player);
        return player;
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
        game.Update(game.Time.Now + deltaTime);
    }

    public static void ErradicateAllArchers(Game game)
    {
        foreach (var keep in game.Map.Keeps.Values)
        {
            keep.SetCount(archers: 0);
        }
    }


    public static void AssertIsApproximately(float f1, float f2, float tolerance = 0.1f)
    {
        float difference = f2 - f1;
        if (MathF.Abs(difference) > tolerance)
        {
            throw new Exception($"{f1} is not close enough to {f2}");
        }
    }

    public static void AssertIsApproximately(Vector3 v1, Vector3 v2, float tolerance = 0.1f)
    {
        Vector3 difference = v1 - v2;
        if (Math.Abs(v2.X - v1.X) >= tolerance)
        {
            throw new Exception($"{v1} is not close enough to {v2}");
        }

        if (Math.Abs(v2.Y - v1.Y) >= tolerance)
        {
            throw new Exception($"{v1} is not close enough to {v2}");
        }

        if (Math.Abs(v2.Z - v1.Z) >= tolerance)
        {
            throw new Exception($"{v1} is not close enough to {v2}");
        }
    }

    public static List<Field> OwnedAndGrownFields(Game game, string playerId)
    {
        return game.Map.Fields.Values.Where(
           field => field.RemainingGrowthTime == 0 &&
           game.Map.GetOwnerOf(field.Position).OwnerId == playerId)
           .ToList();
    }
}