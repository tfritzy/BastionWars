using System.Numerics;
using BastionWars;

namespace Tests;

public static class TH
{
    public static Entity BuildEntity(float x, float y, float radius = .2f)
    {
        return new Entity(new Vector2(x, y), IdGenerator.NextId(), radius);
    }
}