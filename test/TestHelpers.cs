using System.Numerics;
using KeepLordWarriors;

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
        return new Soldier(map, ofAlliance + 1, type, map.Keeps[0].Id, map.Keeps[1].Id);
    }

    public static Soldier BuildAllySoldier(SoldierType type, int ofAlliance, KeepLordWarriors.Map map)
    {
        return new Soldier(map, ofAlliance, type, map.Keeps[0].Id, map.Keeps[1].Id);
    }

    public static void ClearOutbox(Game game)
    {
        game.outbox.Clear();
    }
}