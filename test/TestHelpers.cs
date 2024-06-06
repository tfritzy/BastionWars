using System.Numerics;
using BastionWars;

namespace Tests;

public static class TH
{
    public static Entity BuildEntity(float x, float y, float radius = .2f)
    {
        return new Entity(new Vector2(x, y), IdGenerator.NextId(), radius);
    }

    public static Entity AddNewEntity(Grid grid, float x, float y, float radius = .2f)
    {
        Entity entity = BuildEntity(x, y, radius);
        grid.AddEntity(entity);
        return entity;
    }
}