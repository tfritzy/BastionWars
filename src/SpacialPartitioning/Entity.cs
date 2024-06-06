using System.Numerics;

namespace SpacialPartitioning;

public class Entity
{
    public Vector2 Position;
    public ulong Id;
    public float Radius { get; private set; }
    public float RadiusSqr { get; private set; }

    public Entity(Vector2 pos, ulong id, float radius)
    {
        Position = pos;
        Id = id;
        Radius = radius;
        RadiusSqr = radius * radius;
    }
}
